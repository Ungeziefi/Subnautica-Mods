using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.ModSync.UI
{
    public class ModDifferenceDialog : MonoBehaviour
    {
        private GameObject modSyncPopup;
        private Action<bool> callback;
        private MainMenuShifter shifter;

        public static IEnumerator Show(string message, Action<bool> resultCallback)
        {
            GameObject dialogObj = new("ModDifferenceDialogController");
            ModDifferenceDialog dialog = dialogObj.AddComponent<ModDifferenceDialog>();

            dialog.callback = resultCallback;
            dialog.CreateDialog(message);

            // Wait for user to close the dialog
            while (dialog != null && dialog.gameObject != null)
            {
                yield return null;
            }
        }

        private void CreateDialog(string message)
        {
            GameObject menuPanel = GameObject.Find("Menu canvas/Panel");
            GameObject newsfeedPopup = GameObject.Find("Menu canvas/Panel/NewsfeedPopup");

            // Clone and setup popup
            modSyncPopup = Instantiate(newsfeedPopup, menuPanel.transform);
            modSyncPopup.name = "ModSync";

            // Remove vanilla components
            DestroyImmediate(modSyncPopup.GetComponent<uGUI_NewsfeedPopup>());
            DestroyImmediate(modSyncPopup.GetComponent<uGUI_NavigableControlGrid>());
            DestroyImmediate(modSyncPopup.GetComponent<NewsfeedLayoutGroup>());

            // Canvas
            CanvasGroup canvasGroup = modSyncPopup.GetComponent<CanvasGroup>() ?? modSyncPopup.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            // Background
            Transform background = modSyncPopup.transform.Find("Background");
            ConfigureBackground(background, message);

            // MainMenuShifter (animation)
            shifter = modSyncPopup.AddComponent<MainMenuShifter>();
        }

        private void ConfigureBackground(Transform background, string message)
        {
            // Resize
            RectTransform bgRect = background.GetComponent<RectTransform>();
            if (bgRect != null)
            {
                bgRect.sizeDelta = new Vector2(DialogConfig.DIALOG_WIDTH, DialogConfig.DIALOG_HEIGHT);
            }

            // Close button
            Transform closeButton = background.Find("Close");
            if (closeButton != null)
            {
                SetupTransformSize(closeButton, DialogConfig.CLOSE_BUTTON_SIZE);
                SetupTransformSize(closeButton.Find("Background"), DialogConfig.CLOSE_BUTTON_SIZE);
                SetupTransformSize(closeButton.Find("Icon"), DialogConfig.CLOSE_ICON_SIZE);

                Button closeBtn = closeButton.GetComponent<Button>();
                if (closeBtn != null)
                {
                    closeBtn.onClick.RemoveAllListeners();
                    closeBtn.onClick.AddListener(() => callback.Invoke(false));
                    closeBtn.onClick.AddListener(CloseDialog);
                }
            }

            // Remove RawImage
            Transform rawImage = background.Find("RawImage");
            if (rawImage != null) Destroy(rawImage.gameObject);

            // Setup ScrollView
            Transform textElement = background.Find("Text");
            GameObject scrollView = ScrollViewBuilder.CreateScrollView(background, textElement);
            ScrollViewBuilder.ConfigureContent(scrollView, message);

            // Setup buttons
            Transform buttonObj = background.Find("Button");
            if (buttonObj != null)
            {
                ButtonBuilder.CreateDualButtons(
                    buttonObj,
                    onContinue: () => { callback.Invoke(true); CloseDialog(); },
                    onCancel: () => { callback.Invoke(false); CloseDialog(); }
                );
            }
        }

        private void SetupTransformSize(Transform target, float size)
        {
            if (target == null) return;

            RectTransform rect = target.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(size, size);
            }
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

        private void OnDestroy()
        {
            if (modSyncPopup != null)
            {
                Destroy(modSyncPopup);
            }
        }
    }
}