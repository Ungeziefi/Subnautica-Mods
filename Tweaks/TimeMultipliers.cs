using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    // Multiplies the build time of structures
    [HarmonyPatch(typeof(Constructable))]
    public class TweakConstructableBuildTimeMultiplier
    {
        [HarmonyPatch(nameof(Constructable.GetConstructInterval))]
        public static void Postfix(ref float __result)
        {
            if (Main.Config.BuildTimeMultiplier == 1f || NoCostConsoleCommand.main.fastBuildCheat || !GameModeUtils.RequiresIngredients())
            {
                return;
            }
            __result *= Main.Config.BuildTimeMultiplier;
        }
    }

    // Multiplies the craft time of items
    [HarmonyPatch(typeof(CrafterLogic))]
    public class TweakCrafterLogicCraftTimeMultiplier
    {
        [HarmonyPatch(nameof(CrafterLogic.Craft))]
        public static void Prefix(ref float craftTime)
        {
            GameModeUtils.GetGameMode(out GameModeOption mode, out GameModeOption cheats);
            if (Main.Config.CraftTimeMultiplier == 1f || mode == GameModeOption.Creative)
            {
                return;
            }
            craftTime *= Main.Config.CraftTimeMultiplier;
        }
    }
}