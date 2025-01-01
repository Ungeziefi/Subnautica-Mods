using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch(typeof(Inventory))]
    public class TweakInventoryUnderwaterLimitations
    {
        [HarmonyPatch(nameof(Inventory.GetItemAction)), HarmonyPostfix]
        public static void GetItemAction(Inventory __instance, ref ItemAction __result, InventoryItem item)
        {
            var pickupable = item.item;
            var tt = pickupable.GetTechType();

            // Disables eating underwater
            if (Main.TweaksConfig.NoEatingUnderwater &&
                Player.main.IsUnderwater() &&
                __result == ItemAction.Eat &&
                pickupable.gameObject.GetComponent<Eatable>())
            {
                __result = ItemAction.None;
                return;
            }

            // Disables using medkits underwater
            if (tt == TechType.FirstAidKit && __result == ItemAction.Use)
            {
                if ((Main.TweaksConfig.NoMedkitsUnderwater &&
                    Player.main.IsUnderwater()) ||
                    Player.main.GetComponent<LiveMixin>().maxHealth - Player.main.GetComponent<LiveMixin>().health < 0.01f)
                {
                    __result = ItemAction.None;
                }
            }
        }
    }
}