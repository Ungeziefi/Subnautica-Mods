﻿using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(CyclopsDestructionEvent))]
    public class DeadlyCyclopsExplosion
    {
        [HarmonyPatch(nameof(CyclopsDestructionEvent.DestroyCyclops)), HarmonyPrefix]
        public static void DestroyCyclops(CyclopsDestructionEvent __instance)
        {
            if (Main.Config.DeadlyCyclopsExplosion)
            {
                __instance.subLiveMixin.Kill();
            }
        }
    }
}