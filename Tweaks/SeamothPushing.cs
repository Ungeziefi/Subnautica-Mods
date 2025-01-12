using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class SeamothPushing
    {
        private static bool IsPushable(Vehicle vehicle)
        {
            return vehicle.onGround &&
                   !Inventory.main.GetHeld() &&
                   vehicle is SeaMoth &&
                   !vehicle.docked &&
                   !Player.main.IsSwimming();
        }

        private static void Push(Vehicle vehicle)
        {
            var rb = vehicle.GetComponent<Rigidbody>();
            var direction = new Vector3(MainCameraControl.main.transform.forward.x, 0.2f, MainCameraControl.main.transform.forward.z);
            rb.AddForce(direction * 3333f, ForceMode.Impulse);
        }

        [HarmonyPatch(typeof(Vehicle), nameof(Vehicle.OnHandHover)), HarmonyPostfix]
        public static void Vehicle_OnHandHover(Vehicle __instance)
        {
            if (!Main.Config.SeamothPushing) return;

            if (IsPushable(__instance))
            {
                HandReticle.main.SetText(HandReticle.TextType.Hand, "Push", false, GameInput.Button.RightHand);
                if (GameInput.GetButtonDown(GameInput.Button.RightHand))
                    Push(__instance);
            }
        }
    }
}