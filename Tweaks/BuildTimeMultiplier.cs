﻿using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(Constructable))]
    public class BuildTimeMultiplier
    {
        [HarmonyPatch(nameof(Constructable.GetConstructInterval)), HarmonyPostfix]
        public static void GetConstructInterval(ref float __result)
        {
            if (Main.TweaksConfig.BuildTimeMultiplier == 1f || NoCostConsoleCommand.main.fastBuildCheat || !GameModeUtils.RequiresIngredients())
            {
                return;
            }
            __result *= Main.TweaksConfig.BuildTimeMultiplier;
        }
    }
}