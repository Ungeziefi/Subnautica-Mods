using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.ModSync.UI;

public class ModDifferenceDialog : MonoBehaviour
{
    private Action<bool> callback;
    private GameObject modSyncPopup;
    private MainMenuShifter shifter;

    private void OnDestroy()
    {
        if (modSyncPopup != null) Destroy(modSyncPopup);
    }

    public static IEnumerator Show(string message, Action<bool> resultCallback)
    {
        GameObject dialogObj = new("ModDifferenceDialogController");
        var dialog = dialogObj.AddComponent<ModDifferenceDialog>();

        dialog.callback = resultCallback;
        dialog.CreateDialog(message);

        // Wait for user to close the dialog
        while (dialog != null && dialog.gameObject != null) yield return null;
    }

    private void CreateDialog(string message)
    {
        var menuPanel = GameObject.Find("Menu canvas/Panel");
        var newsfeedPopup = GameObject.Find("Menu canvas/Panel/NewsfeedPopup");

        // Clone and setup popup
        modSyncPopup = Instantiate(newsfeedPopup, menuPanel.transform);
        modSyncPopup.name = "ModSync";

        // Remove vanilla components
        DestroyImmediate(modSyncPopup.GetComponent<uGUI_NewsfeedPopup>());
        DestroyImmediate(modSyncPopup.GetComponent<uGUI_NavigableControlGrid>());
        DestroyImmediate(modSyncPopup.GetComponent<NewsfeedLayoutGroup>());

        // Canvas
        var canvasGroup = modSyncPopup.GetComponent<CanvasGroup>() ?? modSyncPopup.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Background
        var background = modSyncPopup.transform.Find("Background");
        ConfigureBackground(background, message);

        // MainMenuShifter (animation)
        shifter = modSyncPopup.AddComponent<MainMenuShifter>();
    }

    private void ConfigureBackground(Transform background, string message)
    {
        // Resize
        var bgRect = background.GetComponent<RectTransform>();
        if (bgRect != null) bgRect.sizeDelta = new Vector2(DialogConfig.DIALOG_WIDTH, DialogConfig.DIALOG_HEIGHT);

        // Close button
        var closeButton = background.Find("Close");
        if (closeButton != null)
        {
            SetupTransformSize(closeButton, DialogConfig.CLOSE_BUTTON_SIZE);
            SetupTransformSize(closeButton.Find("Background"), DialogConfig.CLOSE_BUTTON_SIZE);
            SetupTransformSize(closeButton.Find("Icon"), DialogConfig.CLOSE_ICON_SIZE);

            var closeBtn = closeButton.GetComponent<Button>();
            if (closeBtn != null)
            {
                closeBtn.onClick.RemoveAllListeners();
                closeBtn.onClick.AddListener(() => callback.Invoke(false));
                closeBtn.onClick.AddListener(CloseDialog);
            }
        }

        // Remove RawImage
        var rawImage = background.Find("RawImage");
        if (rawImage != null) Destroy(rawImage.gameObject);

        // Setup ScrollView
        var textElement = background.Find("Text");
        var scrollView = ScrollViewBuilder.CreateScrollView(background, textElement);
        ScrollViewBuilder.ConfigureContent(scrollView, message);

        // Setup buttons
        var buttonObj = background.Find("Button");
        if (buttonObj != null)
            ButtonBuilder.CreateDualButtons(
                buttonObj,
                () =>
                {
                    callback.Invoke(true);
                    CloseDialog();
                },
                () =>
                {
                    callback.Invoke(false);
                    CloseDialog();
                }
            );
    }

    private void SetupTransformSize(Transform target, float size)
    {
        if (target == null) return;

        var rect = target.GetComponent<RectTransform>();
        if (rect != null) rect.sizeDelta = new Vector2(size, size);
    }

    private void CloseDialog()
    {
        if (modSyncPopup != null)
        {
            Destroy(modSyncPopup);
            modSyncPopup = null;
        }

        Destroy(gameObject);
    }
}