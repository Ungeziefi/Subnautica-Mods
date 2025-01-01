using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(Constructable))]
    public class TweakConstructableBuildTimeMultiplier
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

    [HarmonyPatch(typeof(CrafterLogic))]
    public class TweakCrafterLogicCraftTimeMultiplier
    {
        [HarmonyPatch(nameof(CrafterLogic.Craft)), HarmonyPrefix]
        public static void Craft(ref float craftTime)
        {
            GameModeUtils.GetGameMode(out GameModeOption mode, out GameModeOption cheats);
            if (Main.TweaksConfig.CraftTimeMultiplier == 1f || mode == GameModeOption.Creative)
            {
                return;
            }
            craftTime *= Main.TweaksConfig.CraftTimeMultiplier;
        }
    }
}