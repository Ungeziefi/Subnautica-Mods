using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Camera_Zoom
{
    public static class ZoomUtils
    {
        private static readonly float maxBlackOverlayAlpha = 1.0f;
        private static readonly Dictionary<string, CanvasGroup> overlays = new();
        private static readonly Dictionary<string, bool> activeCoroutines = new();
        private static readonly Dictionary<string, float> cameraDefaultFOVs = new();

        // Create or get black overlay for screen transitions
        public static CanvasGroup GetBlackOverlay(string name)
        {
            if (overlays.TryGetValue(name, out CanvasGroup overlay) && overlay != null)
                return overlay;

            GameObject overlayObj = new($"{name}BlackOverlay");
            overlayObj.transform.SetParent(uGUI.main.transform, false);

            Canvas canvas = overlayObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999; // Force to the top

            CanvasGroup blackOverlay = overlayObj.AddComponent<CanvasGroup>();
            blackOverlay.alpha = 0f;
            blackOverlay.blocksRaycasts = false;

            RectTransform rectTransform = overlayObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            Image image = overlayObj.AddComponent<Image>();
            image.color = Color.black;

            overlays[name] = blackOverlay;
            return blackOverlay;
        }

        // Stop a coroutine and mark it as inactive
        public static void StopCoroutine(Coroutine coroutine, string key)
        {
            if (coroutine != null)
                UWE.CoroutineHost.StopCoroutine(coroutine);

            activeCoroutines[key] = false;
        }

        // Calculate FOV for a specific zoom step
        public static float CalculateStepFOV(int currentStep, float totalSteps, float minFOV, float maxFOV)
        {
            float stepSize = (maxFOV - minFOV) / totalSteps;
            return maxFOV - (currentStep * stepSize);
        }

        // Apply FOV changes
        public static void ApplyFOV(float fov)
        {
            if (SNCameraRoot.main == null)
                return;

            MiscSettings.fieldOfView = fov;
            SNCameraRoot.main.SyncFieldOfView(fov);
        }

        // Handle stepped zoom inputs and effects
        public static bool HandleSteppedZoom(
            bool zoomInPressed,
            bool zoomOutPressed,
            ref int currentStep,
            float maxSteps,
            bool useBlinkEffect,
            float blinkSpeed,
            float minFOV,
            float maxFOV,
            string overlayName,
            ref Coroutine coroutineRef)
        {
            string coroutineKey = $"{overlayName}_blink";

            // Check if a blink effect is in progress
            if (activeCoroutines.TryGetValue(coroutineKey, out bool isActive) && isActive)
                return false;

            bool zoomChanged = false;

            if (zoomInPressed && currentStep < maxSteps)
            {
                currentStep++;
                zoomChanged = true;
            }
            else if (zoomOutPressed && currentStep > 0)
            {
                currentStep--;
                zoomChanged = true;
            }

            if (zoomChanged)
            {
                float targetFOV = CalculateStepFOV(currentStep, maxSteps, minFOV, maxFOV);

                if (useBlinkEffect)
                {
                    activeCoroutines[coroutineKey] = true;
                    coroutineRef = UWE.CoroutineHost.StartCoroutine(BlinkEffect(overlayName, blinkSpeed, targetFOV, coroutineKey));
                }
                else
                {
                    // Apply FOV immediately
                    ApplyFOV(targetFOV);
                }
            }

            return zoomChanged;
        }

        // Create blink transition effect
        private static IEnumerator BlinkEffect(string overlayName, float blinkSpeed, float targetFOV, string coroutineKey)
        {
            CanvasGroup blackOverlay = GetBlackOverlay(overlayName);

            // Blink closing effect
            float time = 0f;
            while (time < 1f)
            {
                time += Time.deltaTime * blinkSpeed;
                blackOverlay.alpha = Mathf.Lerp(0f, maxBlackOverlayAlpha, time);
                yield return null;
            }
            blackOverlay.alpha = maxBlackOverlayAlpha;

            // Change FOV while screen is black
            ApplyFOV(targetFOV);

            // Very short pause at full black
            yield return new WaitForSeconds(0.05f);

            // Blink opening effect
            time = 0f;
            while (time < 1f)
            {
                time += Time.deltaTime * blinkSpeed;
                blackOverlay.alpha = Mathf.Lerp(maxBlackOverlayAlpha, 0f, time);
                yield return null;
            }
            blackOverlay.alpha = 0f;

            // Mark this coroutine as completed
            activeCoroutines[coroutineKey] = false;
        }

        // Handle gradual zoom inputs and effects
        public static bool HandleGradualZoom(GameInput.Button zoomInKey, GameInput.Button zoomOutKey, float zoomSpeed, float minFOV, float maxFOV)
        {
            int zoomDirection = GameInput.GetButtonHeld(zoomInKey) ? -1 :
                               GameInput.GetButtonHeld(zoomOutKey) ? 1 : 0;

            if (zoomDirection == 0) return false;

            Camera camera = SNCameraRoot.main.mainCamera;
            if (camera == null) return false;

            float currentFOV = camera.fieldOfView;
            float newFOV = Mathf.Clamp(
                currentFOV + (zoomDirection * zoomSpeed * Time.deltaTime),
                minFOV,
                maxFOV
            );

            if (newFOV != currentFOV)
            {
                ApplyFOV(newFOV);
                return true;
            }

            return false;
        }

        // Initialize camera mode
        public static void InitializeCameraMode(string cameraType, ref float previousFOV, ref int currentZoomStep, bool useBlinkEffect)
        {
            if (SNCameraRoot.main.mainCamera == null)
                return;

            // Store current FOV as previous (for restoration later)
            previousFOV = SNCameraRoot.main.mainCamera.fieldOfView;

            // Store as default for this camera type if not already saved
            if (!cameraDefaultFOVs.ContainsKey(cameraType))
                cameraDefaultFOVs[cameraType] = previousFOV;

            // Reset zoom step
            currentZoomStep = 0;

            // Initialize black overlay if needed
            if (useBlinkEffect)
                GetBlackOverlay(cameraType);
        }

        // Handle camera switch (maintaining FOV settings)
        public static void SwitchCamera(string cameraType, bool useDefault = false)
        {
            if (SNCameraRoot.main.mainCamera == null)
                return;

            // When switching cameras, we should maintain current FOV unless useDefault is true
            if (useDefault && cameraDefaultFOVs.TryGetValue(cameraType, out float defaultFOV))
                ApplyFOV(defaultFOV);

            // No FOV change otherwise - preserve current camera zoom settings
        }

        // Deactivate camera and restore settings
        public static void DeactivateCamera(string cameraType, float previousFOV, ref int currentZoomStep, ref Coroutine coroutineRef)
        {
            string coroutineKey = $"{cameraType}_blink";

            // Restore original FOV
            ApplyFOV(previousFOV);

            // Reset zoom step
            currentZoomStep = 0;

            // Clear any overlay effects
            if (overlays.TryGetValue(cameraType, out CanvasGroup overlay) && overlay != null)
                overlay.alpha = 0f;

            // Stop any active coroutines
            if (coroutineRef != null)
            {
                UWE.CoroutineHost.StopCoroutine(coroutineRef);
                coroutineRef = null;
            }

            // Mark coroutine as inactive
            activeCoroutines[coroutineKey] = false;
        }

        // Reset all zoom-related state (previously ResetZoomState)
        public static void ResetZoomState(float targetFOV, ref int currentStep, string overlayName, ref Coroutine coroutineRef)
        {
            DeactivateCamera(overlayName, targetFOV, ref currentStep, ref coroutineRef);
        }
    }
}