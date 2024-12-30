using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    public class TweakBarehandsHarvesting
    {
        // Outcrops
        // Determines if the player should be prevented from breaking outcrops without a tool equipped
        private static bool ShouldPreventAction_Outcrops()
        {
            Exosuit exosuit = Player.main.GetVehicle() as Exosuit;
            return Language.main.GetCurrentLanguage() == "English" &&
                   !Main.Config.BarehandsHarvesting &&
                   Inventory.main.GetHeldTool() == null &&
                   !exosuit;
        }

        [HarmonyPatch(typeof(BreakableResource))]
        public class TweakBarehandsHarvesting_BreakableResourceOnHandHover
        {
            [HarmonyPatch(nameof(BreakableResource.OnHandHover))]
            public static bool Prefix(BreakableResource __instance)
            {
                if (ShouldPreventAction_Outcrops())
                {
                    HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, "A tool needs to be equipped");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BreakableResource))]
        public class TweakBarehandsHarvesting_BreakableResourceOnHandClick
        {
            [HarmonyPatch(nameof(BreakableResource.OnHandClick))]
            public static bool Prefix(BreakableResource __instance)
            {
                if (ShouldPreventAction_Outcrops())
                {
                    return false;
                }
                return true;
            }
        }

        // Plants
        // Determines if the player should be prevented from harvesting plants without a knife or heatblade
        private static bool ShouldPreventAction_Plants()
        {
            Knife knife = Inventory.main.GetHeldTool() as Knife;
            HeatBlade heatblade = Inventory.main.GetHeldTool() as HeatBlade;
            Exosuit exosuit = Player.main.GetVehicle() as Exosuit;
            return Language.main.GetCurrentLanguage() == "English" &&
                   !Main.Config.BarehandsHarvesting &&
                   !exosuit &&
                   (knife == null && heatblade == null);
        }

        [HarmonyPatch(typeof(PickPrefab))]
        public class TweakBarehandsHarvesting_PickPrefabOnHandHover
        {
            [HarmonyPatch(nameof(PickPrefab.OnHandHover))]
            public static bool Prefix(PickPrefab __instance)
            {
                if (ShouldPreventAction_Plants())
                {
                    HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, "A knife needs to be equipped");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PickPrefab))]
        public class TweakBarehandsHarvesting_PickPrefabOnHandClick
        {
            [HarmonyPatch(nameof(PickPrefab.OnHandClick))]
            public static bool Prefix(PickPrefab __instance)
            {
                if (ShouldPreventAction_Plants())
                {
                    return false;
                }
                return true;
            }
        }
    }
}