using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class CraftTimeMultiplier
    {
        [HarmonyPatch(typeof(CrafterLogic), nameof(CrafterLogic.Craft)), HarmonyPrefix]
        public static void CrafterLogic_Craft(ref float craftTime)
        {
            GameModeUtils.GetGameMode(out GameModeOption mode, out GameModeOption cheats);
            if (Main.Config.CraftTimeMultiplier == 1 || mode == GameModeOption.Creative)
            {
                return;
            }

            craftTime *= Main.Config.CraftTimeMultiplier;
        }
    }
}