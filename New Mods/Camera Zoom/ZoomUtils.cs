using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Camera_Zoom
{
    public static class ZoomUtils
    {
        private static readonly float maxBlackOverlayAlpha = 1.0f;
        private static readonly Dictionary<string, CanvasGroup> overlays = new Dictionary<string, CanvasGroup>();
        private static readonly Dictionary<string, bool> activeCoroutines = new Dictionary<string, bool>();

        // Create or get black overlay for screen transitions
        public static CanvasGroup GetBlackOverlay(string name)
        {
            if (overlays.TryGetValue(name, out CanvasGroup overlay) && overlay != null)
                return overlay;

            GameObject overlayObj = new GameObject($"{name}BlackOverlay");
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
        public static bool HandleGradualZoom(KeyCode zoomInKey, KeyCode zoomOutKey, float zoomSpeed, float minFOV, float maxFOV)
        {
            int zoomDirection = 0;
            if (Input.GetKey(zoomInKey))
                zoomDirection = -1; // Zoom in
            else if (Input.GetKey(zoomOutKey))
                zoomDirection = 1; // Zoom out

            if (zoomDirection == 0)
                return false;

            Camera camera = SNCameraRoot.main?.mainCamera;
            if (camera == null)
                return false;

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

        // Reset all zoom-related state
        public static void ResetZoomState(float targetFOV, ref int currentStep, string overlayName, ref Coroutine coroutineRef)
        {
            string coroutineKey = $"{overlayName}_blink";

            ApplyFOV(targetFOV);
            currentStep = 0;

            if (overlays.TryGetValue(overlayName, out CanvasGroup overlay) && overlay != null)
                overlay.alpha = 0f;

            if (coroutineRef != null)
            {
                UWE.CoroutineHost.StopCoroutine(coroutineRef);
                coroutineRef = null;
            }

            activeCoroutines[coroutineKey] = false;
        }
    }
}