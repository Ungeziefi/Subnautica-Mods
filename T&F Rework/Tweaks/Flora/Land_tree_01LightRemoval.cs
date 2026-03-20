using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class Land_tree_01LightRemoval
    {
        [HarmonyPatch(typeof(LargeWorldEntity), nameof(LargeWorldEntity.Awake)), HarmonyPostfix]
        public static void LargeWorldEntity_Awake(LargeWorldEntity __instance)
        {
            if (!Main.Config.Land_tree_01LightRemoval) return;

            if (__instance.name == "Land_tree_01(Clone)")
            {
                __instance.GetComponentsInChildren<MeshRenderer>()
                    .SelectMany(mr => mr.materials)
                    .ToList()
                    .ForEach(m => m.DisableKeyword("MARMO_EMISSION"));
            }
        }
    }
}