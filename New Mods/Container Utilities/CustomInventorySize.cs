using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine.UI;

namespace Ungeziefi.Container_Utilities
{
    [HarmonyPatch]
    public class CustomInventorySize
    {
        // UI components for scrollable inventory
        private static ScrollRect scrollRect;

        // Default inventory dimensions
        private const int VANILLA_WIDTH = 6;
        private const int VANILLA_HEIGHT = 8;

        #region Transpiler
        [HarmonyPatch(typeof(Inventory), nameof(Inventory.Awake)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Inventory_Awake(IEnumerable<CodeInstruction> instructions)
        {
            // Skip patching if using vanilla values
            if (Main.Config.InventoryWidth == VANILLA_WIDTH && Main.Config.InventoryHeight == VANILLA_HEIGHT)
                return instructions;

            var matcher = new CodeMatcher(instructions);

            // Find and replace the hard-coded inventory sizes
            matcher.MatchForward(false,
                new CodeMatch(OpCodes.Ldc_I4_6),
                new CodeMatch(OpCodes.Ldc_I4_8)
            );

            matcher.SetInstruction(new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(CustomInventorySize), nameof(GetInventoryWidth))));

            matcher.Advance(1).SetInstruction(new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(CustomInventorySize), nameof(GetInventoryHeight))));

            return matcher.InstructionEnumeration();
        }
        #endregion

        #region Harmony Patches
        [HarmonyPatch(typeof(uGUI_ItemsContainer), nameof(uGUI_ItemsContainer.Init)), HarmonyPostfix]
        public static void uGUI_ItemsContainer_Init(uGUI_ItemsContainer __instance, ItemsContainer ___container)
        {
            // Skip if feature disabled or not applicable
            if (!ShouldApplyCustomSize(__instance, ___container))
                return;

            // If already in a mask, just activate it
            if (__instance.transform.parent.name == "Mask")
            {
                __instance.transform.parent.gameObject.SetActive(true);
                return;
            }

            // Setup scrollview only if needed (if either dimension exceeds vanilla)
            bool needsHorizontalScroll = ___container.sizeX > VANILLA_WIDTH;
            bool needsVerticalScroll = ___container.sizeY > VANILLA_HEIGHT;

            if (needsHorizontalScroll || needsVerticalScroll)
            {
                // Use ScrollbarUtility to setup the scrollview
                scrollRect = ScrollbarUtility.SetupScrollView(
                    __instance,
                    ___container,
                    needsHorizontalScroll,
                    needsVerticalScroll,
                    VANILLA_WIDTH,
                    VANILLA_HEIGHT);
            }
        }

        [HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnOpenPDA)), HarmonyPostfix]
        public static void uGUI_InventoryTab_OnOpenPDA()
        {
            // Skip if feature disabled or scroll view not setup
            if (!Main.Config.EnableCustomInventorySize || scrollRect == null)
                return;

            // Reset scroll position when opening inventory
            scrollRect.verticalNormalizedPosition = 1;
            scrollRect.horizontalNormalizedPosition = 0;

            // Update scrollbar state using ScrollbarUtility
            ScrollbarUtility.UpdateScrollbarState(scrollRect);
        }

        [HarmonyPatch(typeof(uGUI_ItemsContainer), nameof(uGUI_ItemsContainer.Uninit)), HarmonyPostfix]
        public static void uGUI_ItemsContainer_Uninit(uGUI_ItemsContainer __instance)
        {
            // Skip if feature disabled or not applicable
            if (!Main.Config.EnableCustomInventorySize || __instance != __instance.inventory.inventory)
                return;

            // Hide mask when closing inventory
            if (__instance.transform.parent.name == "Mask")
                __instance.transform.parent.gameObject.SetActive(false);
        }
        #endregion

        #region Helper Methods
        private static bool ShouldApplyCustomSize(uGUI_ItemsContainer instance, ItemsContainer container)
        {
            return Main.Config.EnableCustomInventorySize &&
                   instance.transform.name == "InventoryContainer" &&
                   (Main.Config.InventoryWidth > VANILLA_WIDTH || Main.Config.InventoryHeight > VANILLA_HEIGHT);
        }

        private static int GetInventoryWidth() =>
            Main.Config.EnableCustomInventorySize ? Main.Config.InventoryWidth : VANILLA_WIDTH;

        private static int GetInventoryHeight() =>
            Main.Config.EnableCustomInventorySize ? Main.Config.InventoryHeight : VANILLA_HEIGHT;
        #endregion
    }
}