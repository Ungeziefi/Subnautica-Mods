using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class CreepvineUnlocksFiberMesh
    {
        [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Initialize)), HarmonyPostfix]
        public static void PDAScanner_Initialize()
        {
            if (!Main.Config.CreepvineUnlocksFiberMesh) return;

            if (PDAScanner.mapping.ContainsKey(TechType.Creepvine))
            {
                PDAScanner.mapping[TechType.Creepvine].blueprint = TechType.FiberMesh;
            }
        }
    }
}