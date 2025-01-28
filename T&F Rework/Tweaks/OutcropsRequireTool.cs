using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class OutcropsRequireTool
    {
        private static bool ShouldPreventAction()
        {
            GameModeUtils.GetGameMode(out GameModeOption mode, out GameModeOption cheats);
            Exosuit exosuit = Player.main.GetVehicle() as Exosuit;
            var heldTool = Inventory.main.GetHeldTool();

            // Don't prevent action if:
            if (!Main.Config.OutcropsRequireTool || mode == GameModeOption.Creative || exosuit != null)
                return false;

            // Check if any tool is equipped
            return heldTool == null;
        }

        // Outcrops
        [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.OnHandHover)), HarmonyPrefix]
        public static bool BreakableResource_OnHandHover(BreakableResource __instance)
        {
            if (ShouldPreventAction())
            {
                HandReticle.main.SetTextRaw(HandReticle.TextType.Hand, "A tool needs to be equipped");
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(BreakableResource), nameof(BreakableResource.OnHandClick)), HarmonyPrefix]
        public static bool BreakableResource_OnHandClick(BreakableResource __instance) => !ShouldPreventAction();
    }
}