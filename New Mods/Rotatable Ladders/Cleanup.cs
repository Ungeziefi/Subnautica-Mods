using HarmonyLib;

namespace Ungeziefi.Rotatable_Ladders
{
    [HarmonyPatch]
    public partial class RotatableLadders
    {
        [HarmonyPatch(typeof(BaseDeconstructable), nameof(BaseDeconstructable.Deconstruct)), HarmonyPrefix]
        public static void BaseDeconstructable_Deconstruct(BaseDeconstructable __instance)
        {
            if (__instance == null || __instance.transform == null)
                return;

            BaseLadder ladder = __instance.GetComponentInChildren<BaseLadder>();
            if (ladder != null && ladder.transform.parent != null)
            {
                string coords = GetLadderCoords(ladder.transform.parent);
                bool isTopLadder = ladder.transform.parent.name.Contains("LadderTop");

                // Parse coords
                string[] coordParts = coords.Split(',');
                if (coordParts.Length == 3 &&
                    int.TryParse(coordParts[0], out int x) &&
                    int.TryParse(coordParts[1], out int y) &&
                    int.TryParse(coordParts[2], out int z))
                {
                    // Clean up the other piece based on whether this is the top or bottom ladder
                    // Height difference is 3 units
                    string topCoords = isTopLadder ? coords : $"{x},{y - 3},{z}";
                    string bottomCoords = isTopLadder ? $"{x},{y + 3},{z}" : coords;

                    if (Main.SaveData.RotatedLaddersTop.ContainsKey(topCoords))
                    {
                        Main.SaveData.RotatedLaddersTop.Remove(topCoords);
                    }

                    if (Main.SaveData.RotatedLaddersBottom.ContainsKey(bottomCoords))
                    {
                        Main.SaveData.RotatedLaddersBottom.Remove(bottomCoords);
                    }
                }
            }
        }
    }
}