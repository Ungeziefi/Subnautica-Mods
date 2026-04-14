using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Cuddlefish_Renamer
{
    [HarmonyPatch]
    public class CuddlefishRenamer
    {
        private static readonly Dictionary<string, GameObject> nameLabels = new();
        private static bool isRenamingActive = false;

        #region Patches
        [HarmonyPatch(typeof(CuteFish), nameof(CuteFish.Start)), HarmonyPostfix]
        public static void CuteFish_Start(CuteFish __instance)
        {
            if (!Main.Config.EnableFeature) return;

            string cuddlefishId = GetCuddlefishId(__instance);
            if (TryGetSavedName(cuddlefishId, out string savedName) && Main.Config.ShowNameAbove)
            {
                UpdateNameLabel(__instance, savedName);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CuteFishHandTarget), nameof(CuteFishHandTarget.OnHandHover))]
        [HarmonyPatch(typeof(CuteFishHandTarget), nameof(CuteFishHandTarget.OnHandClick))]
        public static void CuteFishHandTarget_Interaction(CuteFishHandTarget __instance)
        {
            if (!Main.Config.EnableFeature || (!__instance.AllowedToInteract() && isRenamingActive)) return;

            UpdateHandReticle(__instance);
            if (GameInput.GetButtonDown(Main.RenameCuddlefishButton))
                CheckRenameInput(__instance);
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Update)), HarmonyPostfix]
        public static void Player_Update()
        {
            if (!Main.Config.EnableFeature) return;

            if (Main.Config.ShowNameAbove)
            {
                UpdateAllNameLabelsVisibility();
                UpdateAllNameLabelsAppearance();
            }
        }
        #endregion

        #region Interaction Logic
        private static void UpdateHandReticle(CuteFishHandTarget handTarget)
        {
            // Update play prompt with custom name
            if (Main.Config.UseNameInPlayPrompt)
            {
                string cuddlefishId = GetCuddlefishId(handTarget.cuteFish);
                if (TryGetSavedName(cuddlefishId, out string savedName))
                {
                    HandReticle.main.SetText(HandReticle.TextType.Hand, $"Play With {savedName}", false, GameInput.Button.LeftHand);
                }
            }

            // Add rename prompt
            string renameText = GameInput.FormatButton(Main.RenameCuddlefishButton, false);
            HandReticle.main.SetText(
                HandReticle.TextType.HandSubscript,
                $"Press {renameText} to rename",
                false);
        }

        private static void CheckRenameInput(CuteFishHandTarget handTarget)
        {
            if (!GameInput.GetButtonDown(Main.RenameCuddlefishButton)) return;

            CuteFish cuddlefish = handTarget.cuteFish;
            if (cuddlefish == null || !handTarget.liveMixin.IsAlive()) return;

            string cuddlefishId = GetCuddlefishId(cuddlefish);
            if (string.IsNullOrEmpty(cuddlefishId)) return;

            TryGetSavedName(cuddlefishId, out string currentName);
            isRenamingActive = true;

            uGUI.main.userInput.RequestString(
                "Cuddlefish Name",
                "Submit",
                currentName ?? string.Empty,
                Main.Config.MaxNameLength,
                (newName) =>
                {
                    try
                    {
                        SetCuddlefishName(cuddlefish, cuddlefishId, newName);
                    }
                    finally
                    {
                        isRenamingActive = false;
                    }
                });
        }
        #endregion

        #region Settings Updates
        private static void UpdateAllNameLabelsAppearance()
        {
            foreach (var label in nameLabels.Values)
            {
                if (label == null) continue;

                label.transform.localPosition = new Vector3(0, Main.Config.NameLabelHeight, 0);

                var text = label.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                {
                    text.fontSize = Main.Config.NameFontSize;
                    text.color = Main.Config.NameColor;
                    text.fontStyle = Main.Config.BoldText ? TMPro.FontStyles.Bold : TMPro.FontStyles.Normal;
                }
            }
        }
        #endregion

        #region Distance-Based Visibility
        private static void UpdateAllNameLabelsVisibility()
        {
            if (Player.main == null) return;
            Vector3 playerPosition = Player.main.transform.position;

            foreach (var label in nameLabels.Values)
            {
                if (label == null) continue;

                if (Main.Config.FadeWithDistance)
                {
                    CuteFish cuddlefish = label.GetComponentInParent<CuteFish>();
                    if (cuddlefish != null)
                    {
                        float distance = Vector3.Distance(playerPosition, cuddlefish.transform.position);
                        UpdateLabelOpacity(label, distance, Main.Config.FadeStartDistance);
                    }
                }
                else
                {
                    label.SetActive(true);
                    TMPro.TextMeshProUGUI text = label.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    if (text != null) text.alpha = 1f;
                }
            }
        }

        private static void UpdateLabelOpacity(GameObject labelObj, float distance, float fadeStartDistance)
        {
            TMPro.TextMeshProUGUI text = labelObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text == null) return;

            float maxDistance = fadeStartDistance * 2;

            if (distance <= fadeStartDistance)
            {
                text.alpha = 1f;
                labelObj.SetActive(true);
            }
            else if (distance >= maxDistance)
            {
                text.alpha = 0f;
                labelObj.SetActive(false);
            }
            else
            {
                float fadeAmount = (distance - fadeStartDistance) / fadeStartDistance;
                text.alpha = Mathf.Clamp01(1f - fadeAmount);
                labelObj.SetActive(true);
            }
        }
        #endregion

        #region Name Label Management
        private static void SetCuddlefishName(CuteFish cuddlefish, string cuddlefishId, string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                Main.SaveData.CuddlefishNames.Remove(cuddlefishId);

                if (nameLabels.TryGetValue(cuddlefishId, out GameObject label) && label != null)
                {
                    GameObject.Destroy(label);
                    nameLabels.Remove(cuddlefishId);
                }
            }
            else
            {
                Main.SaveData.CuddlefishNames[cuddlefishId] = newName;

                if (Main.Config.ShowNameAbove && cuddlefish != null)
                {
                    UpdateNameLabel(cuddlefish, newName);
                }
            }
        }

        private static void UpdateNameLabel(CuteFish cuddlefish, string name)
        {
            string cuddlefishId = GetCuddlefishId(cuddlefish);
            if (string.IsNullOrEmpty(cuddlefishId)) return;

            if (!nameLabels.TryGetValue(cuddlefishId, out GameObject labelObj) || labelObj == null)
            {
                labelObj = CreateNameLabel(cuddlefish, cuddlefishId);
            }

            TMPro.TextMeshProUGUI text = labelObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
            {
                text.text = name;
                ApplyTextFormatting(text);
            }

            UpdateInitialVisibility(labelObj, cuddlefish);
        }

        private static void UpdateInitialVisibility(GameObject labelObj, CuteFish cuddlefish)
        {
            if (!Main.Config.ShowNameAbove || Player.main == null)
            {
                labelObj.SetActive(Main.Config.ShowNameAbove);
                return;
            }

            if (Main.Config.FadeWithDistance)
            {
                float distance = Vector3.Distance(Player.main.transform.position, cuddlefish.transform.position);
                UpdateLabelOpacity(labelObj, distance, Main.Config.FadeStartDistance);
            }
            else
            {
                labelObj.SetActive(true);
            }
        }

        private static GameObject CreateNameLabel(CuteFish cuddlefish, string cuddlefishId)
        {
            // Container object
            GameObject labelObj = new("CuddlefishNameLabel");
            labelObj.transform.SetParent(cuddlefish.transform, false);
            labelObj.transform.localPosition = new Vector3(0, Main.Config.NameLabelHeight, 0);
            labelObj.AddComponent<FaceCamera>();

            // Canvas for UI rendering
            Canvas canvas = labelObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            // Text object
            GameObject textObj = new("NameText");
            textObj.transform.SetParent(labelObj.transform, false);

            RectTransform rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;

            TMPro.TextMeshProUGUI text = textObj.AddComponent<TMPro.TextMeshProUGUI>();
            text.alignment = TMPro.TextAlignmentOptions.Center;
            text.fontSize = Main.Config.NameFontSize;
            text.enableWordWrapping = false;

            ApplyTextFormatting(text);
            nameLabels[cuddlefishId] = labelObj;

            return labelObj;
        }

        private static void ApplyTextFormatting(TMPro.TextMeshProUGUI text)
        {
            text.color = Main.Config.NameColor;
            text.fontStyle = Main.Config.BoldText ? TMPro.FontStyles.Bold : TMPro.FontStyles.Normal;
        }
        #endregion

        #region Utility Methods
        private static string GetCuddlefishId(CuteFish cuddlefish)
        {
            UniqueIdentifier identifier = cuddlefish.GetComponent<UniqueIdentifier>();
            if (identifier != null)
            {
                return identifier.id;
            }

            return null;
        }

        private static bool TryGetSavedName(string cuddlefishId, out string name)
        {
            name = null;
            return !string.IsNullOrEmpty(cuddlefishId) &&
                   Main.SaveData.CuddlefishNames.TryGetValue(cuddlefishId, out name) &&
                   !string.IsNullOrEmpty(name);
        }
        #endregion
    }

    #region FaceCamera Component
    public class FaceCamera : MonoBehaviour
    {
        private Transform cameraTransform;

        void Start()
        {
            cameraTransform = MainCamera.camera.transform;
        }

        void LateUpdate()
        {
            if (cameraTransform != null)
            {
                transform.rotation = Quaternion.LookRotation(
                    transform.position - cameraTransform.position
                );
            }
        }
    }
    #endregion
}