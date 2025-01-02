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
            if (Main.TweaksConfig.CraftTimeMultiplier == 1f || mode == GameModeOption.Creative)
            {
                return;
            }
            craftTime *= Main.TweaksConfig.CraftTimeMultiplier;
        }
    }
}