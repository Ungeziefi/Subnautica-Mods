using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Cuddlefish_Renamer
{
    [HarmonyPatch]
    public class CuddlefishRenamer
    {
        #region Fields
        private static readonly Dictionary<string, GameObject> nameLabels = new();
        private static bool nameLabelsVisible = true;
        private static bool isRenamingActive = false;

        // Settings tracking
        private static float lastNameLabelHeight;
        private static bool lastBoldText;
        private static float lastNameFontSize;
        private static Color lastNameColor;
        #endregion

        #region Harmony Patches
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

        [HarmonyPatch(typeof(CuteFishHandTarget), nameof(CuteFishHandTarget.OnHandHover)), HarmonyPostfix]
        public static void CuteFishHandTarget_OnHandHover(CuteFishHandTarget __instance)
        {
            if (!ShouldProcessInteraction(__instance)) return;

            UpdateHandReticle(__instance);
            CheckRenameInput(__instance);
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Update)), HarmonyPostfix]
        public static void Player_Update()
        {
            if (!Main.Config.EnableFeature) return;

            UpdateVisibilitySettings();
            UpdateAppearanceSettings();
            UpdateDistanceBasedVisibility();
        }
        #endregion

        #region Interaction Logic
        private static bool ShouldProcessInteraction(CuteFishHandTarget handTarget)
        {
            return Main.Config.EnableFeature &&
                   handTarget.AllowedToInteract() &&
                   !isRenamingActive;
        }

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
            if (!GameInput.GetButtonDown(Main.RenameCuddlefishButton) || Cursor.visible) return;

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
        private static void UpdateVisibilitySettings()
        {
            if (Main.Config.ShowNameAbove == nameLabelsVisible) return;

            nameLabelsVisible = Main.Config.ShowNameAbove;
            UpdateAllNameLabelsVisibility(nameLabelsVisible);
        }

        private static void UpdateAppearanceSettings()
        {
            bool settingsChanged = lastNameLabelHeight != Main.Config.NameLabelHeight ||
                                   lastBoldText != Main.Config.BoldText ||
                                   lastNameFontSize != Main.Config.NameFontSize ||
                                   lastNameColor != Main.Config.NameColor;

            if (!settingsChanged) return;

            UpdateAllNameLabelsAppearance();

            // Update cache
            lastNameLabelHeight = Main.Config.NameLabelHeight;
            lastBoldText = Main.Config.BoldText;
            lastNameFontSize = Main.Config.NameFontSize;
            lastNameColor = Main.Config.NameColor;
        }

        private static void UpdateDistanceBasedVisibility()
        {
            if (!Main.Config.ShowNameAbove || !Main.Config.FadeWithDistance) return;

            UpdateNameLabelsDistanceVisibility();
        }
        #endregion

        #region Distance-Based Visibility
        private static void UpdateNameLabelsDistanceVisibility()
        {
            if (Player.main == null) return;

            Vector3 playerPosition = Player.main.transform.position;
            float fadeStartDistance = Main.Config.FadeStartDistance;

            foreach (var entry in nameLabels)
            {
                if (entry.Value == null) continue;

                CuteFish cuddlefish = entry.Value.GetComponentInParent<CuteFish>();
                if (cuddlefish == null) continue;

                float distance = Vector3.Distance(playerPosition, cuddlefish.transform.position);
                UpdateLabelOpacity(entry.Value, distance, fadeStartDistance);
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

        private static void UpdateAllNameLabelsVisibility(bool visible)
        {
            foreach (var label in nameLabels.Values)
            {
                if (label == null) continue;

                label.SetActive(visible);

                if (visible)
                {
                    TMPro.TextMeshProUGUI text = label.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    if (text != null) text.alpha = 1f;
                }
            }

            if (visible && Main.Config.FadeWithDistance)
            {
                UpdateNameLabelsDistanceVisibility();
            }
        }

        private static void UpdateAllNameLabelsAppearance()
        {
            foreach (var entry in nameLabels)
            {
                if (entry.Value == null) continue;

                entry.Value.transform.localPosition = new Vector3(0, Main.Config.NameLabelHeight, 0);

                TMPro.TextMeshProUGUI text = entry.Value.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                {
                    text.fontSize = Main.Config.NameFontSize;
                    ApplyTextFormatting(text);
                }
            }
        }
        #endregion

        #region Name Label Management
        private static void SetCuddlefishName(CuteFish cuddlefish, string cuddlefishId, string newName)
        {
            if (string.IsNullOrEmpty(newName) || string.IsNullOrWhiteSpace(newName))
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

            TMPro.TextMeshProUGUI textMesh = textObj.AddComponent<TMPro.TextMeshProUGUI>();
            textMesh.alignment = TMPro.TextAlignmentOptions.Center;
            textMesh.fontSize = Main.Config.NameFontSize;
            textMesh.enableWordWrapping = false;

            ApplyTextFormatting(textMesh);
            nameLabels[cuddlefishId] = labelObj;

            return labelObj;
        }

        private static void ApplyTextFormatting(TMPro.TextMeshProUGUI textMesh)
        {
            textMesh.color = Main.Config.NameColor;
            textMesh.fontStyle = Main.Config.BoldText ? TMPro.FontStyles.Bold : TMPro.FontStyles.Normal;
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