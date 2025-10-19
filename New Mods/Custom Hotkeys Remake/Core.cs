using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UWE;

namespace Ungeziefi.Custom_Hotkeys_Remake
{
    #region Extensions
    public static class DevConsoleExtensions
    {
        public static bool IsVisible(this DevConsole console)
        {
            if (console == null) return false;
            var stateField = typeof(DevConsole).GetField("state", BindingFlags.Instance | BindingFlags.NonPublic);
            return stateField != null && (bool)stateField.GetValue(console);
        }
    }
    #endregion

    [HarmonyPatch]
    public class ConsoleHotkeys : MonoBehaviour
    {
        #region Fields
        private static ConsoleHotkeys instance;
        private bool isExecutingCommands = false;
        #endregion

        #region Initialization
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
        #endregion

        #region Input Processing
        private void Update()
        {
            if (ShouldProcessInput())
                CheckHotkeyInputs();
        }

        private bool ShouldProcessInput()
        {
            if (!Main.Config.EnableFeature ||
                isExecutingCommands ||
                DevConsole.instance.IsVisible() ||
                WaitScreen.IsWaiting)
                return false;

            return true;
        }

        private void CheckHotkeyInputs()
        {
            foreach (var config in Main.Config.HotkeyConfigurations)
            {
                if (IsHotkeyPressed(config.Keys))
                {
                    StartCoroutine(ExecuteCommands(config));
                    break;
                }
            }
        }

        private bool IsHotkeyPressed(List<KeyCode> keys)
        {
            if (keys == null || keys.Count == 0) 
                return false;

            // Check if at least one key was just pressed
            bool hasKeyDown = keys.Any(Input.GetKeyDown);
            if (!hasKeyDown) 
                return false;

            // Check if all other keys are held
            return keys.All(Input.GetKey);
        }
        #endregion

        #region Command Execution
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
                    bool errorOccurred = false;

                    try
                    {
                        DevConsole.SendConsoleCommand(command);
                        if (hotkeyConfig.ExecutionDelay > 0)
                            ErrorMessage.AddMessage($"Command: {command}");
                    }
                    catch
                    {
                        errorOccurred = true;
                        ErrorMessage.AddMessage($"Error executing command #{i + 1}");
                    }

                    yield return errorOccurred ?
                        new WaitForSeconds(0.5f) :
                        (hotkeyConfig.ExecutionDelay > 0 ?
                            new WaitForSeconds(hotkeyConfig.ExecutionDelay) :
                            null);
                }

                ErrorMessage.AddMessage($"Completed: {hotkeyConfig.Name}");
            }
            finally
            {
                isExecutingCommands = false;
            }
        }
        #endregion
    }
}