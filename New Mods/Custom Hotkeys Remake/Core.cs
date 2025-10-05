using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static void Player_Start(Player __instance)
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
            if (!Main.Config.EnableFeature || isExecutingCommands) return;

            if ((DevConsole.instance != null &&
                DevConsole.instance.IsVisible()) ||
                FreezeTime.PleaseWait ||
                Time.timeScale == 0f) return;

            foreach (var config in Main.Config.HotkeyConfigurations)
                if (AreAllKeysPressed(config.Keys))
                {
                    StartCoroutine(ExecuteCommands(config));
                    break;
                }
        }

        private bool AreAllKeysPressed(List<KeyCode> keys)
        {
            if (keys == null || keys.Count == 0) return false;
            bool hasKeyDown = false;

            foreach (var key in keys)
            {
                if (!Input.GetKey(key)) return false;
                if (Input.GetKeyDown(key)) hasKeyDown = true;
            }
            return hasKeyDown;
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