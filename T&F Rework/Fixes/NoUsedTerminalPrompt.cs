using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoUsedTerminalPrompt
    {
        [HarmonyPatch(typeof(StoryHandTarget), nameof(StoryHandTarget.OnHandHover)), HarmonyPrefix]
        public static bool StoryHandTarget_OnHandHover(StoryHandTarget __instance)
        {
            if (!Main.Config.NoUsedTerminalPrompt) return true;

            // Precursor terminal already used
            var precursorTerminal = __instance.GetComponent<PrecursorComputerTerminal>();
            if (precursorTerminal?.used == true) return false;

            // Generic console already used
            var genericConsole = __instance.GetComponent<GenericConsole>();
            if (genericConsole?.gotUsed == true) return false;

            // Not used yet, keep hover prompt
            return true;
        }
    }
}