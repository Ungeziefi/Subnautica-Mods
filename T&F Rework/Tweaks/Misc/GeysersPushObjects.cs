using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks;

[HarmonyPatch]
public class GeysersPushObjects
{
    private static readonly Dictionary<Geyser, Vector3> eruptionForce = new();
    private static readonly Dictionary<Geyser, Vector3> rotationForce = new();

    private static Vector3 CalculateGeyserForce()
    {
        var force = Main.Config.GeyserEruptionForce;
        var xForce = Random.Range(-force, force) * Random.value;
        var yForce = force + force * Random.value;
        var zForce = Random.Range(-force, force) * Random.value;
        return new Vector3(xForce, yForce, zForce);
    }

    [HarmonyPatch(typeof(Geyser), nameof(Geyser.Erupt))]
    [HarmonyPrefix]
    public static void Geyser_Erupt(Geyser __instance)
    {
        if (!Main.Config.GPOEnableFeature || __instance.erupting || !__instance.gameObject.activeInHierarchy)
            return;

        var force = CalculateGeyserForce();
        eruptionForce[__instance] = force;

        var rot = Random.Range(-force.y, force.y);
        rotationForce[__instance] = new Vector3(rot * Random.value, 0, rot * Random.value);
    }

    [HarmonyPatch(typeof(Geyser), nameof(Geyser.OnTriggerStay))]
    [HarmonyPostfix]
    public static void Geyser_OnTriggerStay(Geyser __instance, Collider other)
    {
        if (!Main.Config.GPOEnableFeature || !__instance.erupting || Main.Config.GeyserEruptionForce == 0f)
            return;

        var go = UWE.Utils.GetEntityRoot(other.gameObject) ?? other.gameObject;

        // Apply geyser physics
        var rb = go.GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            if (eruptionForce.TryGetValue(__instance, out var eruption))
                rb.AddForce(eruption);

            if (rotationForce.TryGetValue(__instance, out var rotation))
                rb.AddTorque(rotation);
        }
    }
}