using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace Ungeziefi.Console_Autocompletion
{
    [HarmonyPatch]
    public class Console_Autocompletion
    {
        private static HashSet<string> commandCache = new();
        private static HashSet<string> techtypeCache = new();
        private static HashSet<string> gotoLocationCache = new();

        // Start caching
        [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.Start)), HarmonyPostfix]
        public static void SubRoot_Start()
        {
            if (Main.Config.EnableFeature) InitializeCaches();
        }

        [HarmonyPatch(typeof(ConsoleInput), nameof(ConsoleInput.KeyPressedOverride)), HarmonyPrefix]
        public static bool ConsoleInput_KeyPressedOverride(ConsoleInput __instance, ref bool __result)
        {
            if (!Main.Config.EnableFeature) return true;

            // Only handle tab completion at end of text
            if (!GameInput.GetButtonDown(Main.AutocompleteButton)
                || string.IsNullOrEmpty(__instance.text)
                || __instance.caretPosition != __instance.text.Length)
                return true;

            // Complete
            string completion = TryCompleteText(__instance.text);
            if (!string.IsNullOrEmpty(completion))
            {
                __instance.text = completion;
                __instance.caretPosition = __instance.text.Length;
            }

            __result = true;
            return false;
        }

        // Build caches
        private static void InitializeCaches()
        {
            commandCache = new HashSet<string>(DevConsole.commands.Keys.Select(k => k.ToLower()));

            // Item TechTypes
            techtypeCache = new HashSet<string>(
                System.Enum.GetValues(typeof(TechType))
                    .Cast<TechType>()
                    .Select(t => t.ToString().ToLower())
            );

            // TeleportCommandData locations
            if (GotoConsoleCommand.main != null && GotoConsoleCommand.main.data != null)
            {
                gotoLocationCache = new HashSet<string>(
                    GotoConsoleCommand.main.data.locations
                        .Select(loc => loc.name.ToLower())
                );
            }
        }

        private static string TryCompleteText(string text)
        {
            int lastSpace = text.LastIndexOf(' ');

            // Complete command if no space
            if (lastSpace == -1) return TryCompleteString(text, commandCache);

            // Complete parameter if after space
            string cmd = text[..lastSpace].Trim().ToLower();
            var targetCache = cmd switch
            {
                "spawn" => techtypeCache,
                "goto" => gotoLocationCache,
                "gotofast" => gotoLocationCache,
                "item" => techtypeCache,
                "unlock" => techtypeCache,
                _ => null
            };

            // Return original if no cache
            if (targetCache == null) return text;

            string param = text[(lastSpace + 1)..];

            // Don't complete if no parameter
            if (string.IsNullOrWhiteSpace(param)) return text;

            string completion = TryCompleteString(param, targetCache);
            return string.IsNullOrEmpty(completion) ? text : $"{cmd} {completion}";
        }

        private static string TryCompleteString(string input, HashSet<string> cache)
        {
            var matches = cache.Where(c => c.StartsWith(input, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!matches.Any()) return string.Empty;

            return matches[0];
        }
    }
}