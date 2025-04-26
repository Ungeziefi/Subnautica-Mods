using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace Ungeziefi.Console_Autocompletion
{
    [HarmonyPatch]
    public class Console_Autocompletion
    {
        // Cache
        private static HashSet<string> commandCache = new();
        private static HashSet<string> techtypeCache = new();

        // Start caching
        [HarmonyPatch(typeof(SubRoot), nameof(SubRoot.Start)), HarmonyPostfix]
        public static void SubRoot_Start(SubRoot __instance)
        {
            if (Main.Config.EnableFeature) InitializeCaches();
        }

        [HarmonyPatch(typeof(ConsoleInput), nameof(ConsoleInput.KeyPressedOverride)), HarmonyPrefix]
        public static bool ConsoleInput_KeyPressedOverride(ConsoleInput __instance, ref bool __result)
        {
            if (!Main.Config.EnableFeature) return true;

            // Only handle tab completion at end of text
            if (__instance.processingEvent.keyCode != Main.Config.ConsoleAutocompletionKey
                || string.IsNullOrEmpty(__instance.text)
                || __instance.caretPosition != __instance.text.Length)
                return true;

            // Complete text
            string completion = TryCompleteText(__instance.text);
            if (!string.IsNullOrEmpty(completion))
            {
                __instance.text = completion;
                __instance.caretPosition = __instance.text.Length;
            }

            __result = true;
            return false;
        }

        // Build command and item caches
        private static void InitializeCaches()
        {
            // Cache commands
            commandCache = new HashSet<string>(DevConsole.commands.Keys.Select(k => k.ToLower()));

            // Cache item types
            techtypeCache = new HashSet<string>(
                System.Enum.GetValues(typeof(TechType))
                    .Cast<TechType>()
                    .Select(t => t.ToString().ToLower())
            );
        }

        // Complete command or parameter
        private static string TryCompleteText(string text)
        {
            int lastSpace = text.LastIndexOf(' ');

            // Complete command if no space
            if (lastSpace == -1)
                return TryCompleteString(text, commandCache);

            // Complete parameter if after space
            string cmd = text[..lastSpace].Trim().ToLower();
            var targetCache = cmd switch
            {
                "spawn" => techtypeCache,
                _ => commandCache
            };

            string param = text[(lastSpace + 1)..];
            // Don't complete if parameter is empty (just spaces)
            if (string.IsNullOrWhiteSpace(param))
                return text;

            string completion = TryCompleteString(param, targetCache);
            return string.IsNullOrEmpty(completion) ? text : $"{cmd} {completion}";
        }

        // Find and return matching completion
        private static string TryCompleteString(string input, HashSet<string> cache)
        {
            var matches = cache.Where(c => c.StartsWith(input, StringComparison.OrdinalIgnoreCase)).ToList();

            if (Main.Config.LogMatchingItems)
            {
                Main.Logger.LogInfo($"Input: {input}");
                Main.Logger.LogInfo($"Matches found: {matches.Count}");
                Main.Logger.LogInfo($"Matches: {string.Join(", ", matches)}");
            }

            if (!matches.Any()) return string.Empty;

            // Return original case for commands
            if (cache == commandCache)
            {
                var originalCase = DevConsole.commands.Keys.First(k =>
                    k.Equals(matches[0], StringComparison.OrdinalIgnoreCase));
                if (Main.Config.LogMatchingItems)
                    Main.Logger.LogInfo($"Command match, returning: {originalCase}");
                return originalCase;
            }

            if (Main.Config.LogMatchingItems)
                Main.Logger.LogInfo($"Non-command match, returning: {matches[0]}");
            return matches[0];
        }
    }
}