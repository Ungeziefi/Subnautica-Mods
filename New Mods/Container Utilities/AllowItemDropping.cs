using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Ungeziefi.Container_Utilities
{
    [HarmonyPatch]
    public class AllowItemDropping
    {
        private static bool HandleSubCheckResult(bool isInSub, Player player)
        {
            if (isInSub && player.currentWaterPark == null && Main.Config.AllowDropInHabitats)
            {
                return false; // Prevent branch if in sub (not in water park)
            }

            // Otherwise return the original result
            return isInSub;
        }

        [HarmonyPatch(typeof(Inventory), nameof(Inventory.CanDropItemHere)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Inventory_CanDropItemHere_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Config.AllowDropInHabitats) return instructions;

            var matcher = new CodeMatcher(instructions);

            matcher.MatchForward(true,
                new CodeMatch(OpCodes.Ldloc_0), // Load Player.main
                new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Player), "get_currentSub")),
                new CodeMatch(OpCodes.Ldnull),
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(UnityEngine.Object), "op_Inequality"))
            ).Advance(1);

            matcher.Insert(
                new CodeInstruction(OpCodes.Ldloc_0), // Load Player.main again
                Transpilers.EmitDelegate(HandleSubCheckResult)
            );

            return matcher.InstructionEnumeration();
        }

        // Add patch to InternalDropItem
    }
}