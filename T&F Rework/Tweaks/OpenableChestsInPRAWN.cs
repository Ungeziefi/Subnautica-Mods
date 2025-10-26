using System;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class OpenableChestsInPRAWN
    {
        [HarmonyPatch(typeof(ExosuitClawArm), "IExosuitArm.GetInteractableRoot"), HarmonyPostfix]
        public static void IExosuitArm_GetInteractableRoot(ExosuitClawArm __instance, GameObject target, ref GameObject __result)
        {
            if (!Main.Config.OpenableChestsInPRAWN) return;

            // Return SupplyCrate as interactable
            if (__result == null && target.GetComponent<SupplyCrate>())
                __result = target;
        }

        [HarmonyPatch(typeof(ExosuitClawArm), "TryUse", new Type[] { typeof(float) }, new[] { ArgumentType.Out }), HarmonyPrefix]
        public static bool ExosuitClawArm_TryUse(ExosuitClawArm __instance, ref float cooldownDuration, ref bool __result)
        {
            if (!Main.Config.OpenableChestsInPRAWN) return true;

            if (Time.time - __instance.timeUsed < __instance.cooldownTime)
            {
                cooldownDuration = 0f;
                __result = false;
                return false;
            }

            Pickupable pickupable = null;
            PickPrefab pickPrefab = null;
            SupplyCrate supplyCrate = null;

            GameObject activeTarget = __instance.exosuit.GetActiveTarget();

            if (activeTarget)
            {
                pickupable = activeTarget.GetComponent<Pickupable>();
                pickPrefab = activeTarget.GetComponent<PickPrefab>();
                supplyCrate = activeTarget.GetComponent<SupplyCrate>(); // Add SupplyCrate component check
            }

            if (pickupable != null && pickupable.isPickupable)
            {
                if (__instance.exosuit.storageContainer.container.HasRoomFor(pickupable))
                {
                    __instance.animator.SetTrigger("use_tool");
                    __instance.cooldownTime = cooldownDuration = __instance.cooldownPickup;
                    if (__instance.shownNoRoomNotification)
                        __instance.shownNoRoomNotification = false;
                    __result = true;
                    return false;
                }

                if (!__instance.shownNoRoomNotification)
                {
                    ErrorMessage.AddMessage(Language.main.Get("ContainerCantFit"));
                    __instance.shownNoRoomNotification = true;
                }
                cooldownDuration = 0f;
                __result = false;
                return false;
            }

            if (pickPrefab != null)
            {
                __instance.animator.SetTrigger("use_tool");
                __instance.cooldownTime = cooldownDuration = __instance.cooldownPickup;
                __result = true;
                return false;
            }

            // Supply crate handling
            if (supplyCrate != null)
            {
                // Check if sealed
                if (supplyCrate.sealedComp && supplyCrate.sealedComp.IsSealed())
                {
                    cooldownDuration = 0f;
                    __result = false;
                    return false;
                }

                bool playAnim = false;

                if (!supplyCrate.open)
                {
                    // Open crate
                    supplyCrate.ToggleOpenState();
                    playAnim = true;
                }
                else if (supplyCrate.open && supplyCrate.itemInside)
                {
                    // Take item from crate
                    if (__instance.exosuit.storageContainer.container.HasRoomFor(supplyCrate.itemInside))
                    {
                        ItemsContainer container = __instance.exosuit.storageContainer.container;
                        supplyCrate.itemInside.Initialize();
                        InventoryItem inventoryItem = new(supplyCrate.itemInside);
                        container.UnsafeAdd(inventoryItem);
                        FMODUWE.PlayOneShot(__instance.pickupSound, __instance.front, 5f);
                        supplyCrate.itemInside = null;
                        playAnim = true;
                    }
                    else
                    {
                        ErrorMessage.AddMessage(Language.main.Get("ContainerCantFit"));
                        cooldownDuration = 0f;
                        __result = false;
                        return false;
                    }
                }

                if (playAnim)
                {
                    __instance.animator.SetTrigger("use_tool");
                    __instance.cooldownTime = cooldownDuration = __instance.cooldownPickup;
                    __result = true;
                    return false;
                }

                cooldownDuration = 0f;
                __result = false;
                return false;
            }

            __instance.animator.SetTrigger("bash");
            __instance.cooldownTime = cooldownDuration = __instance.cooldownPunch;
            __instance.fxControl.Play(0);
            __result = true;
            return false;
        }
    }
}