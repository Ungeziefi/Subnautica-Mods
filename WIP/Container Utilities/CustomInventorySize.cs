// To-Do: Fix it breaking DimUnallowedItems

//using System.Collections.Generic;
//using System.Reflection.Emit;
//using HarmonyLib;
//using Ungeziefi.Container_Utilities.UI;
//using UnityEngine.UI;

//namespace Ungeziefi.Container_Utilities
//{
//    [HarmonyPatch]
//    public class CustomInventorySize
//    {
//        private static ScrollRect scrollRect;
//        private const int VANILLA_WIDTH = 6;
//        private const int VANILLA_HEIGHT = 8;

//        #region Patches
//        [HarmonyPatch(typeof(Inventory), nameof(Inventory.Awake)), HarmonyTranspiler]
//        public static IEnumerable<CodeInstruction> Inventory_Awake(IEnumerable<CodeInstruction> instructions)
//        {
//            if (Main.Config.InventoryWidth == VANILLA_WIDTH && Main.Config.InventoryHeight == VANILLA_HEIGHT)
//                return instructions;

//            var matcher = new CodeMatcher(instructions);

//            // Replace sizes
//            matcher.MatchForward(false,
//                new CodeMatch(OpCodes.Ldc_I4_6),
//                new CodeMatch(OpCodes.Ldc_I4_8)
//            );

//            matcher.SetInstruction(new CodeInstruction(OpCodes.Call,
//                AccessTools.Method(typeof(CustomInventorySize), nameof(GetInventoryWidth))));

//            matcher.Advance(1).SetInstruction(new CodeInstruction(OpCodes.Call,
//                AccessTools.Method(typeof(CustomInventorySize), nameof(GetInventoryHeight))));

//            return matcher.InstructionEnumeration();
//        }

//        [HarmonyPatch(typeof(uGUI_ItemsContainer), nameof(uGUI_ItemsContainer.Init)), HarmonyPostfix]
//        public static void uGUI_ItemsContainer_Init(uGUI_ItemsContainer __instance, ItemsContainer ___container)
//        {
//            if (!ShouldApplyCustomSize(__instance)) return;

//            if (Main.Config.InventoryHeight > VANILLA_HEIGHT)
//            {
//                scrollRect = ScrollbarUtility.CreateScrollView(__instance, ___container, VANILLA_HEIGHT);
//            }
//        }

//        [HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnOpenPDA)), HarmonyPostfix]
//        public static void uGUI_InventoryTab_OnOpenPDA()
//        {
//            if (!Main.Config.EnableCustomInventorySize || scrollRect == null)
//                return;

//            // Reset scroll to top on open
//            scrollRect.verticalNormalizedPosition = 1;
//        }
//        #endregion

//        #region Helper Methods
//        private static bool ShouldApplyCustomSize(uGUI_ItemsContainer instance)
//        {
//            return Main.Config.EnableCustomInventorySize &&
//                   instance.transform.name == "InventoryContainer" &&
//                   (Main.Config.InventoryWidth != VANILLA_WIDTH || Main.Config.InventoryHeight != VANILLA_HEIGHT);
//        }

//        private static int GetInventoryWidth() =>
//            Main.Config.EnableCustomInventorySize ? Main.Config.InventoryWidth : VANILLA_WIDTH;

//        private static int GetInventoryHeight() =>
//            Main.Config.EnableCustomInventorySize ? Main.Config.InventoryHeight : VANILLA_HEIGHT;
//        #endregion
//    }
//}