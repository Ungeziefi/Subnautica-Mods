using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class BuildTimeMultiplier
    {
        [HarmonyPatch(typeof(Constructable), nameof(Constructable.GetConstructInterval)), HarmonyPostfix]
        public static void Constructable_GetConstructInterval(ref float __result)
        {
            if (Main.Config.BuildTimeMultiplier == 1 || NoCostConsoleCommand.main.fastBuildCheat || !GameModeUtils.RequiresIngredients())
            {
                return;
            }

            __result *= Main.Config.BuildTimeMultiplier;
        }
    }
}