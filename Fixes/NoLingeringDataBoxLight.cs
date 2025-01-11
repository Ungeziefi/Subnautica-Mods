using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoUsedDataBoxLight
    {
        // Track all Data Box lights
        public static HashSet<GameObject> databoxLights = new HashSet<GameObject>();

        [HarmonyPatch(typeof(GenericHandTarget), nameof(GenericHandTarget.OnHandClick)), HarmonyPostfix]
        public static void GenericHandTarget_OnHandClick(GenericHandTarget __instance)
        {
            if (!Main.Config.NoUsedDataBoxLight || !__instance.GetComponent<BlueprintHandTarget>())
            {
                return;
            }

            // Closest to player within 5 units
            GameObject closest = null;
            float shortestDist = 5f;

            foreach (GameObject light in databoxLights)
            {
                // Skip inactive lights
                if (!light || !light.activeSelf)
                {
                    continue;
                }

                // Distance between light and player
                float distance = Vector3.Distance(light.transform.position, Player.main.transform.position);

                // Update closest light
                if (distance < shortestDist)
                {
                    shortestDist = distance;
                    closest = light;
                }
            }

            // Disable and remove from tracking
            if (closest != null)
            {
                closest.SetActive(false);
                databoxLights.Remove(closest);
            }
        }

        [HarmonyPatch(typeof(VFXVolumetricLight), nameof(VFXVolumetricLight.Awake)), HarmonyPostfix]
        public static void VFXVolumetricLight_Awake(VFXVolumetricLight __instance)
        {
            // Add Data Box lights to tracking
            if (__instance.transform.parent && __instance.transform.parent.name.StartsWith("DataboxLight"))
            {
                databoxLights.Add(__instance.transform.parent.gameObject);
            }
        }
    }
}