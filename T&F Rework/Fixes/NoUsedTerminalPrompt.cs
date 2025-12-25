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
            if (precursorTerminal != null && precursorTerminal.used) return false;

            var genericConsole = __instance.GetComponent<GenericConsole>();
            if (genericConsole != null && genericConsole.gotUsed) return false;

            return true;
        }
    }
}