using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class HarvestingRequirements
    {
        private static bool ShouldPreventAction(bool isOutcrop)
        {
            GameModeUtils.GetGameMode(out GameModeOption mode, out GameModeOption cheats);
            Exosuit exosuit = Player.main.GetVehicle() as Exosuit;
            Knife knife = Inventory.main.GetHeldTool() as Knife;
            HeatBlade heatblade = Inventory.main.GetHeldTool() as HeatBlade;

            if (!Main.Config.HarvestingRequirements &&
                mode == GameModeOption.Creative &&
                !exosuit)
            {
                return false;
            }

            // Check outcrops
            if (isOutcrop)
            {
                return Inventory.main.GetHeldTool() == null;
            }
            // Check plants
            else
            {
                return knife == null && heatblade == null;
            }
        }

        // Outcrops
        [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.OnHandHover)), HarmonyPrefix]
        public static bool BreakableResource_OnHandHover(BreakableResource __instance)
        {
            if (ShouldPreventAction(true))
            {
                HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, "A tool needs to be equipped");
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.OnHandClick)), HarmonyPrefix]
        public static bool BreakableResource_OnHandClick(BreakableResource __instance)
        {
            if (ShouldPreventAction(true))
            {
                return false;
            }
            return true;
        }

        // Plants
        [HarmonyPatch(typeof(PickPrefab), nameof(PickPrefab.OnHandHover)), HarmonyPrefix]
        public static bool PickPrefab_OnHandHover(PickPrefab __instance)
        {
            if (ShouldPreventAction(false))
            {
                HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, "A blade needs to be equipped");
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(PickPrefab), nameof(PickPrefab.OnHandClick)), HarmonyPrefix]
        public static bool PickPrefab_OnHandClick(PickPrefab __instance)
        {
            if (ShouldPreventAction(false))
            {
                return false;
            }
            return true;
        }
    }
}