using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Stasis_Rifle_Freeze_Fix;

[HarmonyPatch]
public class StasisRifleFreezeFix
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.TryStartAction))]
    public static bool Creature_TryStartAction(CreatureAction action)
    {
        if (!Main.Config.EnableFeature) return true;

        var component = action.creature.GetComponent<SRFF_Component>();
        if (!component) return false;

        if (component.IsFrozen()) return false;

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.Start))]
    public static void Creature_Start(Creature __instance)
    {
        if (!Main.Config.EnableFeature) return;

        var instanceRb = __instance.gameObject.GetComponentInChildren<Rigidbody>();

        var fixComponent = instanceRb.gameObject.EnsureComponent<SRFF_Component>();
        fixComponent.creature = __instance;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CuteFishHandTarget), nameof(CuteFishHandTarget.AllowedToInteract))]
    public static void CuteFishHandTarget_AllowedToInteract(CuteFishHandTarget __instance, ref bool __result)
    {
        if (!Main.Config.EnableFeature) return;

        if (CreatureIsFrozen(__instance.creatureRigidbody)) __result = false;
    }

    private static bool CreatureIsFrozen(Rigidbody instanceRb)
    {
        if (instanceRb == null) return false;

        if (!StasisRifle.sphere) return false;

        if (StasisRifle.sphere.targets == null) return false;

        if (StasisRifle.sphere.targets.Contains(instanceRb)) return true;

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GasPod), nameof(GasPod.Update))]
    public static bool GasPod_Update(GasPod __instance)
    {
        if (!Main.Config.EnableFeature) return true;

        var rb = __instance.mainCollider.attachedRigidbody;
        if (!rb) return true;

        return !rb.isKinematic;
    }
}