using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using InputPaths = Nautilus.Handlers.GameInputHandler.Paths;

namespace Ungeziefi.Console_Autocompletion
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Console_Autocompletion";
        public const string PLUGIN_NAME = "Console Autocompletion";
        public const string PLUGIN_VERSION = "1.0.0";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }
        internal static new Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        public static GameInput.Button AutocompleteButton;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            AutocompleteButton = EnumHandler.AddEntry<GameInput.Button>("AutocompleteButton")
                .CreateInput("Autocomplete")
                .WithKeyboardBinding(InputPaths.Keyboard.Tab)
                .WithCategory("Console Autocompletion")
                .AvoidConflicts();

            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
        }
    }
}