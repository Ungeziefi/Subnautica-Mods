using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(GenericHandTarget))]
    public class NoLingeringDataBoxLight
    {
        // Keeps track of all databox lights in the game
        public static HashSet<GameObject> databoxLights = new HashSet<GameObject>();

        [HarmonyPatch(nameof(GenericHandTarget.OnHandClick))]
        public static void Postfix(GenericHandTarget __instance)
        {
            if (!Main.Config.NoLingeringDataBoxLight || !__instance.GetComponent<BlueprintHandTarget>())
            {
                return;
            }

            // Find the closest databox light within 5 units of the player
            GameObject closest = null;
            float shortestDist = 5f;

            foreach (GameObject light in databoxLights)
            {
                // Skip inactive lights
                if (!light || !light.activeSelf)
                {
                    continue;
                }

                // Straight-line distance between the light and the player
                float distance = Vector3.Distance(light.transform.position, Player.main.transform.position);

                // If this light is closer than any we've found so far (or closer than 5 units)
                if (distance < shortestDist)  // shortestDist starts at 5f
                {
                    // Remember this as the new shortest distance
                    shortestDist = distance;
                    // Remember this as the closest light
                    closest = light;
                }
            }

            // Disable the closest light and remove it from tracking
            if (closest != null)
            {
                closest.SetActive(false);
                databoxLights.Remove(closest);
            }
        }
    }

    [HarmonyPatch(typeof(VFXVolumetricLight))]
    public class NoLingeringDataBoxLight_VFXVolumetricLight
    {
        // When a volumetric light is created, check if it's a Data Box light
        [HarmonyPatch(nameof(VFXVolumetricLight.Awake)), HarmonyPostfix]
        public static void Awake(VFXVolumetricLight __instance)
        {
            // Add to the tracking list
            if (__instance.transform.parent && __instance.transform.parent.name.StartsWith("DataboxLight"))
            {
                NoLingeringDataBoxLight.databoxLights.Add(__instance.transform.parent.gameObject);
            }
        }
    }
}