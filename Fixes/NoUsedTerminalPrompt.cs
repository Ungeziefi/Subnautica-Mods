using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class NoUsedTerminalPrompt
    {
        [HarmonyPatch(typeof(StoryHandTarget), nameof(StoryHandTarget.OnHandHover)), HarmonyPrefix]
        public static bool StoryHandTarget_OnHandHover(StoryHandTarget __instance)
        {
            if (!Main.Config.NoUsedTerminalPrompt)
            {
                return true;
            }

            // Skip hover text if used
            var Terminal = __instance.GetComponent<PrecursorComputerTerminal>();
            if (Terminal && Terminal.used)
            {
                return false;
            }

            return true;
        }
    }
}