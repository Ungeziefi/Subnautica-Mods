using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Rotatable_Ladders
{
    [HarmonyPatch]
    public partial class RotatableLadders
    {
        // Get coords as string
        public static string GetLadderCoords(Transform transform)
        {
            return $"{Mathf.RoundToInt(transform.position.x)},{Mathf.RoundToInt(transform.position.y)},{Mathf.RoundToInt(transform.position.z)}";
        }

        // Hover prompt
        [HarmonyPatch(typeof(BaseLadder), nameof(BaseLadder.OnHandHover)), HarmonyPostfix]
        public static void BaseLadder_OnHandHover(BaseLadder __instance, GUIHand hand)
        {
            if (!Main.Config.EnableFeature || __instance == null || !__instance.enabled) return;

            // Empty hands only feature check
            bool isHoldingItem = Inventory.main.GetHeldTool() != null;
            if (Main.Config.EmptyHandsOnly && isHoldingItem) return;

            var primaryDevice = GameInput.GetPrimaryDevice();
            var rotateText = primaryDevice == GameInput.Device.Controller ? "Press to rotate" : $"Press {Main.Config.RotateLadderKey} to rotate";

            HandReticle.main.SetText(type: HandReticle.TextType.HandSubscript,
                text: rotateText,
                translate: false,
                button: primaryDevice == GameInput.Device.Controller ? GameInput.Button.AltTool : GameInput.Button.None);

            if (GameInput.GetKeyDown(Main.Config.RotateLadderKey) ||
                ((GameInput.GetPrimaryDevice() == GameInput.Device.Controller) && GameInput.GetButtonDown(GameInput.Button.AltTool)))
            {
                Transform parent = __instance.transform.parent;
                if (parent == null) return;

                // Get coords and apply rotation
                string coords = GetLadderCoords(parent);
                float newYRotation = (parent.localRotation.eulerAngles.y + 90f) % 360f;
                parent.localRotation = Quaternion.Euler(0f, newYRotation, 0f);

                // Split based on bottom or top
                var dictionary = parent.name.Contains("LadderBottom") ?
                    Main.SaveData.RotatedLaddersBottom :
                    Main.SaveData.RotatedLaddersTop;

                // Update and remove if rotation is 0
                if (newYRotation == 0f)
                    dictionary.Remove(coords);
                else
                    dictionary[coords] = newYRotation;
            }
        }

        [HarmonyPatch(typeof(BaseLadder), nameof(BaseLadder.Start)), HarmonyPostfix]
        public static void BaseLadder_Start(BaseLadder __instance)
        {
            Transform parent = __instance.transform.parent;
            if (!Main.Config.EnableFeature || __instance == null || parent == null) return;

            // Get coords and check data
            string coords = GetLadderCoords(parent);
            var dictionary = parent.name.Contains("LadderBottom") ?
                Main.SaveData.RotatedLaddersBottom :
                Main.SaveData.RotatedLaddersTop;

            // Apply rotation
            if (dictionary.TryGetValue(coords, out float savedRotation))
            {
                parent.localRotation = Quaternion.Euler(0f, savedRotation, 0f);
            }
        }
    }
}