using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Rotatable_Ladders;

[HarmonyPatch]
public class RotatableLadders
{
    private static readonly HashSet<Vector3Int> activeRotations = new();
    private static readonly Dictionary<Vector3Int, Transform> ladderCache = new();

    private static Vector3Int GetCoords(Transform t)
    {
        return new Vector3Int(
            Mathf.RoundToInt(t.position.x),
            Mathf.RoundToInt(t.position.y),
            Mathf.RoundToInt(t.position.z));
    }

    private static bool IsTop(Transform t)
    {
        return t.name.Contains("LadderTop");
    }

    private static Vector3Int GetPartnerKey(Transform t)
    {
        var pos = GetCoords(t);
        // LadderTop is the long piece, Bottom is the hatch above
        return IsTop(t) ? pos + new Vector3Int(0, 3, 0) : pos - new Vector3Int(0, 3, 0);
    }

    // InOutQuad ease, always the short 90 degree arc
    private static IEnumerator RotateSmoothly(Transform t, float targetY, Vector3Int key)
    {
        var from = t.localRotation;
        var to = Quaternion.Euler(0f, targetY, 0f);
        var duration = Main.Config.RotationDuration;

        for (var elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            if (t == null) break;

            var p = elapsed / duration;
            var eased = p < 0.5f ? 2f * p * p : 1f - Mathf.Pow(-2f * p + 2f, 2f) / 2f;
            t.localRotation = Quaternion.Slerp(from, to, eased);
            yield return null;
        }

        if (t != null) t.localRotation = to;
        activeRotations.Remove(key);
    }

    private static void ApplyRotation(Transform ladderTransform, bool isPartner = false)
    {
        if (ladderTransform == null) return;

        var key = GetCoords(ladderTransform);

        // Block input if rotating
        if (activeRotations.Contains(key)) return;

        var newY = (ladderTransform.localRotation.eulerAngles.y + 90f) % 360f;

        // Rotation
        if (Main.Config.SmoothRotation)
        {
            activeRotations.Add(key);

            Main.Instance.StartCoroutine(RotateSmoothly(ladderTransform, newY, key));
        }
        else
        {
            ladderTransform.localRotation = Quaternion.Euler(0f, newY, 0f);
        }

        // SaveData
        var data = IsTop(ladderTransform) ? Main.SaveData.RotatedLaddersTop : Main.SaveData.RotatedLaddersBottom;
        var stringKey = $"{key.x},{key.y},{key.z}";

        if (Mathf.Approximately(newY, 0f) || Mathf.Approximately(newY, 360f))
            data.Remove(stringKey);
        else
            data[stringKey] = newY;

        // Rotate connected ladder
        if (!isPartner && Main.Config.AffectConnectedLadder)
        {
            var partnerKey = GetPartnerKey(ladderTransform);
            if (ladderCache.TryGetValue(partnerKey, out var partnerTransform)) ApplyRotation(partnerTransform, true);
        }
    }

    [HarmonyPatch(typeof(BaseLadder), nameof(BaseLadder.OnHandHover))]
    [HarmonyPostfix]
    public static void BaseLadder_OnHandHover(BaseLadder __instance)
    {
        if (!Main.Config.EnableFeature || __instance == null || !__instance.enabled) return;
        if (Main.Config.EmptyHandsOnly && Inventory.main.GetHeldTool() != null) return;

        var parent = __instance.transform.parent;
        if (parent == null) return;

        var key = GetCoords(parent);

        // Hide prompt if the ladder is rotating
        if (activeRotations.Contains(key)) return;

        HandReticle.main.SetText(HandReticle.TextType.HandSubscript,
            $"Press {Main.Config.RotateKey} to rotate", false);

        if (Input.GetKeyDown(Main.Config.RotateKey)) ApplyRotation(parent);
    }

    [HarmonyPatch(typeof(BaseLadder), nameof(BaseLadder.Start))]
    [HarmonyPostfix]
    public static void BaseLadder_Start(BaseLadder __instance)
    {
        var parent = __instance.transform.parent;
        if (parent == null) return;

        var key = GetCoords(parent);
        ladderCache[key] = parent;

        var data = IsTop(parent) ? Main.SaveData.RotatedLaddersTop : Main.SaveData.RotatedLaddersBottom;
        if (data.TryGetValue($"{key.x},{key.y},{key.z}", out var savedRotation))
            parent.localRotation = Quaternion.Euler(0f, savedRotation, 0f);
    }

    [HarmonyPatch(typeof(BaseDeconstructable), nameof(BaseDeconstructable.Deconstruct))]
    [HarmonyPrefix]
    public static void BaseDeconstructable_Deconstruct(BaseDeconstructable __instance)
    {
        var ladder = __instance.GetComponentInChildren<BaseLadder>();
        if (ladder?.transform.parent == null) return;

        var parent = ladder.transform.parent;
        var key = GetCoords(parent);
        var partnerKey = GetPartnerKey(parent);

        // Remove from SaveData
        Main.SaveData.RotatedLaddersTop.Remove($"{key.x},{key.y},{key.z}");
        Main.SaveData.RotatedLaddersBottom.Remove($"{key.x},{key.y},{key.z}");
        Main.SaveData.RotatedLaddersTop.Remove($"{partnerKey.x},{partnerKey.y},{partnerKey.z}");
        Main.SaveData.RotatedLaddersBottom.Remove($"{partnerKey.x},{partnerKey.y},{partnerKey.z}");

        // Clean up caches
        ladderCache.Remove(key);
        ladderCache.Remove(partnerKey);
        activeRotations.Remove(key);
        activeRotations.Remove(partnerKey);
    }
}