using HarmonyLib;

namespace Ungeziefi.Rotatable_Ladders;

[HarmonyPatch]
public partial class RotatableLadders
{
    [HarmonyPatch(typeof(BaseDeconstructable), nameof(BaseDeconstructable.Deconstruct))]
    [HarmonyPrefix]
    public static void BaseDeconstructable_Deconstruct(BaseDeconstructable __instance)
    {
        if (__instance == null || __instance.transform == null)
            return;

        var ladder = __instance.GetComponentInChildren<BaseLadder>();
        if (ladder != null && ladder.transform.parent != null)
        {
            var coords = GetLadderCoords(ladder.transform.parent);
            var isTopLadder = ladder.transform.parent.name.Contains("LadderTop");

            // Parse coords
            var coordParts = coords.Split(',');
            if (coordParts.Length == 3 &&
                int.TryParse(coordParts[0], out var x) &&
                int.TryParse(coordParts[1], out var y) &&
                int.TryParse(coordParts[2], out var z))
            {
                // Clean up the other piece based on whether this is the top or bottom ladder
                // Height difference is 3 units
                var topCoords = isTopLadder ? coords : $"{x},{y - 3},{z}";
                var bottomCoords = isTopLadder ? $"{x},{y + 3},{z}" : coords;

                if (Main.SaveData.RotatedLaddersTop.ContainsKey(topCoords))
                    Main.SaveData.RotatedLaddersTop.Remove(topCoords);

                if (Main.SaveData.RotatedLaddersBottom.ContainsKey(bottomCoords))
                    Main.SaveData.RotatedLaddersBottom.Remove(bottomCoords);
            }
        }
    }
}