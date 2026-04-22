using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UWE;

namespace Ungeziefi.Camera_Zoom;

public static class ZoomUtils
{
    private static CanvasGroup blackOverlay;
    private static bool isBlinking;
    public static float DefaultFOV { get; private set; }

    private static float lastUIRefreshTime;
    private const float UI_REFRESH_INTERVAL = 0.1f;

    public static void ApplyFOV(float fov)
    {
        if (SNCameraRoot.main == null) return;
        MiscSettings.fieldOfView = fov;
        SNCameraRoot.main.SyncFieldOfView(fov);

        // Limit the use of FindObjectsOfTypeAll
        if (Time.unscaledTime >= lastUIRefreshTime + UI_REFRESH_INTERVAL)
        {
            lastUIRefreshTime = Time.unscaledTime;
            foreach (var t in Resources.FindObjectsOfTypeAll<TextMeshProUGUI>()) t.SetScaleDirty();
        }
    }

    public static void PrepareForCameraMode()
    {
        PlayerCamera.ForceReset();
        DefaultFOV = MiscSettings.fieldOfView;
    }

    public static void HandleSteppedZoom(bool zoomIn, bool zoomOut, ref int currentStep, float maxSteps, bool useBlink,
        float blinkSpeed, float min, float max)
    {
        if (isBlinking) return;

        var direction = (zoomIn ? 1 : 0) - (zoomOut ? 1 : 0);
        if (direction == 0) return;

        var nextStep = Mathf.Clamp(currentStep + direction, 0, (int)maxSteps);
        if (nextStep == currentStep) return;

        currentStep = nextStep;
        var targetFOV = max - currentStep * ((max - min) / maxSteps);

        if (useBlink) CoroutineHost.StartCoroutine(BlinkEffect(blinkSpeed, targetFOV));
        else ApplyFOV(targetFOV);
    }

    public static void HandleGradualZoom(GameInput.Button inKey, GameInput.Button outKey, float speed, float min,
        float max)
    {
        float dir = GameInput.GetButtonHeld(inKey) ? -1 : GameInput.GetButtonHeld(outKey) ? 1 : 0;
        if (dir == 0) return;
        var newFOV = Mathf.Clamp(SNCameraRoot.main.mainCamera.fieldOfView + dir * speed * Time.deltaTime, min, max);
        ApplyFOV(newFOV);
    }

    private static IEnumerator BlinkEffect(float speed, float targetFOV)
    {
        isBlinking = true;
        var overlay = GetOverlay();
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            overlay.alpha = t;
            yield return null;
        }

        ApplyFOV(targetFOV);
        yield return new WaitForSeconds(0.05f);
        while (t > 0)
        {
            t -= Time.deltaTime * speed;
            overlay.alpha = t;
            yield return null;
        }

        overlay.alpha = 0;
        isBlinking = false;
    }

    private static CanvasGroup GetOverlay()
    {
        if (blackOverlay != null) return blackOverlay;
        var obj = new GameObject("ZoomBlackOverlay");
        obj.transform.SetParent(uGUI.main.transform, false);
        obj.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        obj.GetComponent<Canvas>().sortingOrder = 999;
        blackOverlay = obj.AddComponent<CanvasGroup>();
        blackOverlay.alpha = 0;
        blackOverlay.blocksRaycasts = false;
        var img = obj.AddComponent<Image>();
        img.color = Color.black;
        img.rectTransform.anchorMin = Vector2.zero;
        img.rectTransform.anchorMax = Vector2.one;
        return blackOverlay;
    }
}