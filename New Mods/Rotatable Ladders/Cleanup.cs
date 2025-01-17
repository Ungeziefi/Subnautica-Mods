using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Rotatable_Ladders
{
    [HarmonyPatch]
    public partial class Core
    {
        [HarmonyPatch(typeof(BaseLadder), nameof(BaseLadder.Start)), HarmonyPostfix]
        public static void BaseLadder_Start()
        {
            // Get ladders
            var allLadders = Object.FindObjectsOfType<BaseLadder>();
            var validCoords = new HashSet<string>();

            // Get their coords
            foreach (var ladder in allLadders)
            {
                if (ladder?.transform?.parent != null)
                {
                    validCoords.Add(GetLadderCoords(ladder.transform.parent));
                }
            }

            // Remove invalid coords
            var bottomKeys = Main.SaveData.RotatedLaddersBottom.Keys.ToList();
            var topKeys = Main.SaveData.RotatedLaddersTop.Keys.ToList();

            foreach (var coord in bottomKeys)
            {
                if (!validCoords.Contains(coord))
                {
                    Main.SaveData.RotatedLaddersBottom.Remove(coord);
                }
            }

            foreach (var coord in topKeys)
            {
                if (!validCoords.Contains(coord))
                {
                    Main.SaveData.RotatedLaddersTop.Remove(coord);
                }
            }
        }
    }
}