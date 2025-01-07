using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(CrafterLogic))]
    public class CraftTimeMultiplier
    {
        [HarmonyPatch(nameof(CrafterLogic.Craft)), HarmonyPrefix]
        public static void Craft(ref float craftTime)
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