using System.Collections;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Custom_Hotkeys_Remake
{
    [HarmonyPatch]
    public class ConsoleHotkeys : MonoBehaviour
    {
        private static ConsoleHotkeys instance;
        private bool isExecutingCommands = false;

        [HarmonyPatch(typeof(Player), nameof(Player.Start)), HarmonyPostfix]
        public static void Player_Start()
        {
            if (!Main.Config.EnableFeature) return;

            if (instance == null)
            {
                instance = new GameObject("ConsoleHotkeysManager")
                    .AddComponent<ConsoleHotkeys>();
                DontDestroyOnLoad(instance.gameObject);
            }
        }

        [System.Obsolete]
        private void Awake()
        {
            DevConsole.RegisterConsoleCommand(this, "refreshhotkeys", false, false);
        }

        [HarmonyPatch(typeof(WaitScreen), nameof(WaitScreen.ReportStageDurations)), HarmonyPostfix]
        public static void WaitScreen_ReportStageDurations()
        {
            if (!Main.Config.EnableFeature) return;

            foreach (var config in Main.Config.HotkeyConfigurations)
            {
                if (config.RunOnLoad)
                {
                    instance.StartCoroutine(instance.ExecuteCommands(config));
                    Main.Logger.LogInfo($"Executing on load: {config.Name}");
                    break;
                }
            }
        }

        private void Update()
        {
            if (!Main.Config.EnableFeature ||
                isExecutingCommands ||
                DevConsole.instance.state ||
                WaitScreen.IsWaiting)
                return;

            if (GameInput.GetButtonDown(Main.RefreshHotkeysButton))
            {
                OnConsoleCommand_refreshhotkeys();
                return;
            }

            foreach (var config in Main.Config.HotkeyConfigurations)
            {
                if (config.Keys == null || config.Keys.Count == 0) return;

                if (config.Keys.Any(Input.GetKeyDown) && config.Keys.All(Input.GetKey))
                {
                    StartCoroutine(ExecuteCommands(config));
                    break;
                }
            }
        }

        private void OnConsoleCommand_refreshhotkeys()
        {
            Main.ReloadConfig();
            ErrorMessage.AddMessage("Hotkeys configuration reloaded");
        }

        private IEnumerator ExecuteCommands(Config.CommandHotkey hotkeyConfig)
        {
            if (hotkeyConfig.Commands == null || hotkeyConfig.Commands.Count == 0) yield break;

            isExecutingCommands = true;

            try
            {
                ErrorMessage.AddMessage($"Executing: {hotkeyConfig.Name}");
                var commandsToExecute = hotkeyConfig.Commands.Where(cmd => !string.IsNullOrWhiteSpace(cmd)).ToList();

                for (int i = 0; i < commandsToExecute.Count; i++)
                {
                    string command = commandsToExecute[i];

                    ErrorMessage.AddMessage($"Executing command #{i + 1}: {command}");
                    DevConsole.SendConsoleCommand(command);

                    if (hotkeyConfig.ExecutionDelay > 0 && i < commandsToExecute.Count - 1)
                    {
                        Main.Logger.LogInfo($"Index is now {i} and count is {commandsToExecute.Count}");
                        yield return new WaitForSeconds(hotkeyConfig.ExecutionDelay);
                    }
                }

                ErrorMessage.AddMessage($"Completed: {hotkeyConfig.Name}");
            }
            finally
            {
                isExecutingCommands = false;
            }
        }
    }
}