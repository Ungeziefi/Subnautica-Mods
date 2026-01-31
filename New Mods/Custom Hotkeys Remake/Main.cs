using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using System.Reflection;
using InputPaths = Nautilus.Handlers.GameInputHandler.Paths;

namespace Ungeziefi.Custom_Hotkeys_Remake
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Custom_Hotkeys_Remake";
        public const string PLUGIN_NAME = "Custom Hotkeys Remake";
        public const string PLUGIN_VERSION = "1.4.0";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }
        internal static new Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        public static GameInput.Button RefreshHotkeysButton;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            RefreshHotkeysButton = EnumHandler.AddEntry<GameInput.Button>("RefreshHotkeysButton")
                .CreateInput("Refresh hotkeys")
                .WithKeyboardBinding(InputPaths.Keyboard.F9)
                .WithCategory("Custom Hotkeys Remake")
                .AvoidConflicts();

            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
        }

        public static void ReloadConfig()
        {
            Config.Load();
        }
    }
}