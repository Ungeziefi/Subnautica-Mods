using HarmonyLib;

namespace Ungeziefi.Abandon_Ship_During_Cyclops_Fire
{
    [HarmonyPatch]
    public class AbandonShipDuringCyclopsFire
    {
        [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.Start)), HarmonyPostfix]
        public static void SubRoot_Start(SubRoot __instance)
        {
            if (!Main.Config.EnableFeature) return;

            if (!__instance.isCyclops)
                return;

            if (__instance.gameObject.scene.name != "Cyclops")
                return;

            var subFire = __instance.GetComponent<SubFire>();
            if (subFire?.fireMusic == null)
                return;

            subFire.fireMusic.asset = new FMODAsset
            {
                name = "abandon_ship",
                path = "event:/main_menu/credits_music",
                id = "{b17609fe-cdc9-4039-85b6-062e4ceab82a}"
            };
        }
    }
}