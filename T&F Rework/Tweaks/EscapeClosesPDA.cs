using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class EscapeClosesPDA
    {
        [HarmonyPatch(typeof(PDA), nameof(PDA.ManagedUpdate)), HarmonyPostfix]
        public static void PDA_ManagedUpdate(PDA __instance)
        {
            if (Main.Config.EscapeClosesPDA &&
                __instance.isInUse && __instance.isFocused && !__instance.ui.introActive &&
                GameInput.GetKeyDown(KeyCode.Escape))
            {
                __instance.Close();
            }
        }
    }
}