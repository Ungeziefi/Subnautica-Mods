using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class MoveableMobileVehicleBay
    {
        [HarmonyPatch(typeof(Constructor), nameof(Constructor.OnEnable)), HarmonyPostfix]
        public static void Constructor_OnEnable(Constructor __instance)
        {
            if (!Main.Config.MoveableMobileVehicleBay) return;

            ImmuneToPropulsioncannon immuneComponent = __instance.GetComponent<ImmuneToPropulsioncannon>();
            if (immuneComponent)
            {
                UnityEngine.Object.Destroy(immuneComponent);
            }
        }
    }
}