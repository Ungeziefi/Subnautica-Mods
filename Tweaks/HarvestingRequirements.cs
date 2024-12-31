using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    public class TweakHarvestingRequirements
    {
        // Determines if the player should be prevented from breaking outcrops and picking fruits without a tool equipped
        private static bool ShouldPreventAction(bool isOutcrop)
        {
            GameModeUtils.GetGameMode(out GameModeOption mode, out GameModeOption cheats);
            Exosuit exosuit = Player.main.GetVehicle() as Exosuit;
            Knife knife = Inventory.main.GetHeldTool() as Knife;
            HeatBlade heatblade = Inventory.main.GetHeldTool() as HeatBlade;

            if (Language.main.GetCurrentLanguage() == "English" &&
                Main.TweaksConfig.HarvestingRequirements &&
                mode == GameModeOption.Creative &&
                !exosuit)
            {
                return false;
            }

            // Check if the action should be prevented for outcrops
            if (isOutcrop)
            {
                return Inventory.main.GetHeldTool() == null;
            }

            // Check if the action should be prevented for plants
            else
            {
                return knife == null && heatblade == null;
            }
        }

        // Outcrops
        [HarmonyPatch(typeof(BreakableResource))]
        public class TweakBarehandsHarvesting_BreakableResourceOnHandHover
        {
            [HarmonyPatch(nameof(BreakableResource.OnHandHover)), HarmonyPrefix]
            public static bool OnHandHover(BreakableResource __instance)
            {
                if (ShouldPreventAction(true))
                {
                    HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, "A tool needs to be equipped");
                    return false;
                }
                return true;
            }

            [HarmonyPatch(nameof(BreakableResource.OnHandClick)), HarmonyPrefix]
            public static bool OnHandClick(BreakableResource __instance)
            {
                if (ShouldPreventAction(true))
                {
                    return false;
                }
                return true;
            }
        }

        // Plants
        [HarmonyPatch(typeof(PickPrefab))]
        public class TweakBarehandsHarvesting_PickPrefab
        {
            [HarmonyPatch(nameof(PickPrefab.OnHandHover)), HarmonyPrefix]
            public static bool OnHandHover(PickPrefab __instance)
            {
                if (ShouldPreventAction(false))
                {
                    HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, "A knife or Thermoblade needs to be equipped");
                    return false;
                }
                return true;
            }

            [HarmonyPatch(nameof(PickPrefab.OnHandClick)), HarmonyPrefix]
            public static bool OnHandClick(PickPrefab __instance)
            {
                if (ShouldPreventAction(false))
                {
                    return false;
                }
                return true;
            }
        }
    }
}