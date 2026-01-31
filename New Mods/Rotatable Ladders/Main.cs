using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using System.Reflection;
using InputPaths = Nautilus.Handlers.GameInputHandler.Paths;

namespace Ungeziefi.Rotatable_Ladders
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Rotatable_Ladders";
        public const string PLUGIN_NAME = "Rotatable Ladders";
        public const string PLUGIN_VERSION = "2.1.0";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }
        internal static new Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static SaveData SaveData { get; private set; }
        public static GameInput.Button RotateLadderButton;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            RotateLadderButton = EnumHandler.AddEntry<GameInput.Button>("RotateLadderButton")
                .CreateInput("Rotate ladder")
                .WithKeyboardBinding(InputPaths.Keyboard.R)
                .WithControllerBinding(InputPaths.Gamepad.DpadUp)
                .WithCategory("Rotatable Ladders")
                .AvoidConflicts();

            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
            SaveData = SaveDataHandler.RegisterSaveDataCache<SaveData>();
        }
    }
}