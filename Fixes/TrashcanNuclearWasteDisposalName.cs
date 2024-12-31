﻿using HarmonyLib;

namespace Ungeziefi.Fixes
{
    // From "Use Trashcan" to "Use Nuclear waste disposal"
    [HarmonyPatch(typeof(Trashcan))]
    public class FixTrashcanNuclearWasteDisposalName
    {
        [HarmonyPatch(nameof(Trashcan.OnEnable)), HarmonyPrefix]
        public static void OnEnable(Trashcan __instance)
        {
            if (Language.main.GetCurrentLanguage() == "English" && Main.FixesConfig.NuclearWasteDisposalName)
            {
                var usePrefix = Language.main.Get("Use") + " ";
                __instance.storageContainer.hoverText = usePrefix + (__instance.biohazard ? Language.main.Get("LabTrashcan") : Language.main.Get("Trashcan"));
            }
        }
    }
}