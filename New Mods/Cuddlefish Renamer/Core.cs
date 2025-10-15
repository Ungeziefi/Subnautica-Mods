using System.Collections.Generic;
using HarmonyLib;
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
            if (Main.SaveData.CuddlefishNames.TryGetValue(cuddlefishId, out string savedName) && !string.IsNullOrEmpty(savedName) && Main.Config.ShowNameAbove)
            {
                UpdateNameLabel(__instance, savedName);
            }
        }

        [HarmonyPatch(typeof(CuteFishHandTarget), nameof(CuteFishHandTarget.OnHandHover)), HarmonyPostfix]
        public static void CuteFishHandTarget_OnHandHover(CuteFishHandTarget __instance, GUIHand hand)
        {
            if (!Main.Config.EnableFeature || !__instance.AllowedToInteract() || isRenamingActive) return;

            if (Main.Config.CustomPlayPrompt)
            {
                string cuddlefishId = GetCuddlefishId(__instance.cuteFish);
                if (!string.IsNullOrEmpty(cuddlefishId) &&
                    Main.SaveData.CuddlefishNames.TryGetValue(cuddlefishId, out string savedName) &&
                    !string.IsNullOrEmpty(savedName))
                {
                    HandReticle.main.SetText(HandReticle.TextType.Hand, $"Play With {savedName}", false, GameInput.Button.LeftHand);
                }
            }

            string renameText = GameInput.IsPrimaryDeviceGamepad() ? "Press to rename" : $"Press {Main.Config.RenameKey} to rename";

            // Add rename prompt
            var handReticle = HandReticle.main;
            if (handReticle.textHandSubscript.Length > 0)
            {
                string currentText = handReticle.textHandSubscript;

                if (!currentText.Contains(renameText))
                {
                    string newText = string.IsNullOrEmpty(currentText) ? renameText : $"{currentText}\n{renameText}";
                    handReticle.SetText(
                        HandReticle.TextType.HandSubscript,
                        newText,
                        false,
                        GameInput.IsPrimaryDeviceGamepad() ? GameInput.Button.AltTool : GameInput.Button.None);
                }
            }

            // Check for input
            if ((Input.GetKeyDown(Main.Config.RenameKey) ||
                ((GameInput.IsPrimaryDeviceGamepad()) && GameInput.GetButtonDown(GameInput.Button.AltTool)))
                && !Cursor.visible)
            {
                CuteFish cuddlefish = __instance.cuteFish;
                if (cuddlefish != null && __instance.liveMixin.IsAlive())
                {
                    string cuddlefishId = GetCuddlefishId(cuddlefish);
                    if (string.IsNullOrEmpty(cuddlefishId)) return;

                    Main.SaveData.CuddlefishNames.TryGetValue(cuddlefishId, out string currentName);

                    isRenamingActive = true;

                    uGUI.main.userInput.RequestString(
                        "Cuddlefish Name",
                        "Submit",
                        currentName ?? string.Empty,
                        Main.Config.MaxNameLength,
                        (newName) =>
                        {
                            isRenamingActive = false;
                            SetCuddlefishName(cuddlefish, cuddlefishId, newName);
                        });
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Update)), HarmonyPostfix]
        public static void Player_Update()
        {
            if (!Main.Config.EnableFeature) return;

            // Update visibility settings
            if (Main.Config.ShowNameAbove != nameLabelsVisible)
            {
                nameLabelsVisible = Main.Config.ShowNameAbove;
                UpdateAllNameLabelsVisibility(nameLabelsVisible);
            }

            // Check setting changes
            if (lastNameLabelHeight != Main.Config.NameLabelHeight ||
                lastBoldText != Main.Config.BoldText ||
                lastNameFontSize != Main.Config.NameFontSize ||
                lastNameColor != Main.Config.NameColor)
            {
                // Update appearance settings for all labels
                UpdateAllNameLabelsAppearance();

                // Update cache
                lastNameLabelHeight = Main.Config.NameLabelHeight;
                lastBoldText = Main.Config.BoldText;
                lastNameFontSize = Main.Config.NameFontSize;
                lastNameColor = Main.Config.NameColor;
            }

            // Distance-based visibility updates
            if (Main.Config.ShowNameAbove && Main.Config.FadeWithDistance)
            {
                UpdateNameLabelsDistanceVisibility();
            }
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

                // Calculate opacity based on distance
                UpdateLabelOpacity(entry.Value, distance, fadeStartDistance);
            }
        }

        private static void UpdateLabelOpacity(GameObject labelObj, float distance, float fadeStartDistance)
        {
            TMPro.TextMeshPro text = labelObj.GetComponent<TMPro.TextMeshPro>();
            if (text == null) return;

            // If closer than fade start distance, full opacity
            if (distance <= fadeStartDistance)
            {
                text.alpha = 1f;
                labelObj.SetActive(true);
            }
            // If beyond 2x fade start distance, hide completely
            else if (distance >= fadeStartDistance * 2)
            {
                text.alpha = 0f;
                labelObj.SetActive(false);
            }
            // Otherwise, fade gradually
            else
            {
                // Calculate opacity: 1.0 at fadeStartDistance, 0.0 at 2*fadeStartDistance
                float fadeRange = fadeStartDistance;
                float fadeAmount = (distance - fadeStartDistance) / fadeRange;
                text.alpha = Mathf.Clamp01(1f - fadeAmount);
                labelObj.SetActive(true);
            }
        }

        private static void UpdateAllNameLabelsVisibility(bool visible)
        {
            foreach (var label in nameLabels.Values)
            {
                if (label != null)
                {
                    label.SetActive(visible);

                    // Reset opacity to full when toggling visibility
                    if (visible)
                    {
                        TMPro.TextMeshPro text = label.GetComponent<TMPro.TextMeshPro>();
                        if (text != null)
                        {
                            text.alpha = 1f;
                        }
                    }
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

                // Update height
                entry.Value.transform.localPosition = new Vector3(0, Main.Config.NameLabelHeight, 0);

                // Update text formatting
                TMPro.TextMeshPro text = entry.Value.GetComponent<TMPro.TextMeshPro>();
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
            if (string.IsNullOrEmpty(newName))
            {
                Main.SaveData.CuddlefishNames.Remove(cuddlefishId);

                if (nameLabels.TryGetValue(cuddlefishId, out GameObject label))
                {
                    GameObject.Destroy(label);
                    nameLabels.Remove(cuddlefishId);
                }
            }
            else
            {
                Main.SaveData.CuddlefishNames[cuddlefishId] = newName;

                if (Main.Config.ShowNameAbove)
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

            TMPro.TextMeshPro text = labelObj.GetComponent<TMPro.TextMeshPro>();
            if (text != null)
            {
                text.text = name;
                ApplyTextFormatting(text);
            }

            // Set initial visibility and opacity based on distance
            if (Main.Config.ShowNameAbove && Player.main != null)
            {
                float distance = Vector3.Distance(Player.main.transform.position, cuddlefish.transform.position);

                if (Main.Config.FadeWithDistance)
                {
                    UpdateLabelOpacity(labelObj, distance, Main.Config.FadeStartDistance);
                }
                else
                {
                    labelObj.SetActive(true);
                }
            }
            else
            {
                labelObj.SetActive(Main.Config.ShowNameAbove);
            }
        }

        private static GameObject CreateNameLabel(CuteFish cuddlefish, string cuddlefishId)
        {
            GameObject labelObj = new("CuddlefishNameLabel");
            labelObj.transform.SetParent(cuddlefish.transform, false);
            labelObj.transform.localPosition = new Vector3(0, Main.Config.NameLabelHeight, 0);

            TMPro.TextMeshPro textMesh = labelObj.AddComponent<TMPro.TextMeshPro>();
            textMesh.alignment = TMPro.TextAlignmentOptions.Center;
            textMesh.fontSize = Main.Config.NameFontSize;

            ApplyTextFormatting(textMesh);
            labelObj.AddComponent<FaceCamera>();
            nameLabels[cuddlefishId] = labelObj;

            return labelObj;
        }

        private static void ApplyTextFormatting(TMPro.TextMeshPro textMesh)
        {
            textMesh.color = Main.Config.NameColor;
            textMesh.fontStyle = Main.Config.BoldText ? TMPro.FontStyles.Bold : TMPro.FontStyles.Normal;
        }
        #endregion

        #region Utility Methods
        private static string GetCuddlefishId(CuteFish cuddlefish)
        {
            UniqueIdentifier identifier = cuddlefish.GetComponent<UniqueIdentifier>();
            if (identifier != null && !string.IsNullOrEmpty(identifier.Id))
            {
                return identifier.Id;
            }

            return null;
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