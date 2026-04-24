using System.Collections.Generic;
using DG.Tweening;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Rotatable_Ladders;

[HarmonyPatch]
public class RotatableLadders
{
    private static readonly Dictionary<Vector3Int, Transform> ladderCache = new();

    private static Vector3Int GetCoords(Transform t)
    {
        return new Vector3Int(Mathf.RoundToInt(t.position.x),
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

    private static void ApplyRotation(Transform ladderTransform, bool isPartner = false)
    {
        if (ladderTransform == null) return;

        var key = GetCoords(ladderTransform);
        var newY = (ladderTransform.localRotation.eulerAngles.y + 90f) % 360f;

        // Rotation
        if (Main.Config.SmoothRotation)
            ladderTransform.DOLocalRotate(new Vector3(0, newY, 0), Main.Config.RotationDuration);
        else
            ladderTransform.localRotation = Quaternion.Euler(0f, newY, 0f);

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

        HandReticle.main.SetText(HandReticle.TextType.HandSubscript,
            $"Press {GameInput.FormatButton(Main.RotateLadderButton)} to rotate", false);

        if (GameInput.GetButtonDown(Main.RotateLadderButton)) ApplyRotation(__instance.transform.parent);
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

    // Cleanup
    [HarmonyPatch(typeof(BaseDeconstructable), nameof(BaseDeconstructable.Deconstruct))]
    [HarmonyPrefix]
    public static void BaseDeconstructable_Deconstruct(BaseDeconstructable __instance)
    {
        var ladder = __instance.GetComponentInChildren<BaseLadder>();
        if (ladder?.transform.parent == null) return;

        var parent = ladder.transform.parent;
        var key = GetCoords(parent);
        var partnerKey = GetPartnerKey(parent);

        Main.SaveData.RotatedLaddersTop.Remove($"{key.x},{key.y},{key.z}");
        Main.SaveData.RotatedLaddersBottom.Remove($"{key.x},{key.y},{key.z}");
        Main.SaveData.RotatedLaddersTop.Remove($"{partnerKey.x},{partnerKey.y},{partnerKey.z}");
        Main.SaveData.RotatedLaddersBottom.Remove($"{partnerKey.x},{partnerKey.y},{partnerKey.z}");

        ladderCache.Remove(key);
        ladderCache.Remove(partnerKey);
    }
}