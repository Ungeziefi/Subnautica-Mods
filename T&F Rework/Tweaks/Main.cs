using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using InputPaths = Nautilus.Handlers.GameInputHandler.Paths;

namespace Ungeziefi.Tweaks
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]

    // Incompatible plugins (GUID)
    [BepInIncompatibility("qqqbbb.subnautica.tweaksAndFixes")]

    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Tweaks";
        public const string PLUGIN_NAME = "Tweaks";
        public const string PLUGIN_VERSION = "1.0.0";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }
        internal static new Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static SaveData SaveData { get; private set; }
        public static GameInput.Button SeamothCycleTorpedoButton;
        public static GameInput.Button PRAWNSuitCycleTorpedoButton;
        public static GameInput.Button PRAWNSuitLightsToggleButton;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            SeamothCycleTorpedoButton = EnumHandler.AddEntry<GameInput.Button>("SeamothCycleTorpedoButton")
                .CreateInput("Seamoth cycle torpedo")
                .WithKeyboardBinding(InputPaths.Keyboard.R)
                .WithControllerBinding(InputPaths.Gamepad.DpadDown)
                .WithCategory("Tweaks");

            PRAWNSuitCycleTorpedoButton = EnumHandler.AddEntry<GameInput.Button>("PRAWNSuitCycleTorpedoButton")
                .CreateInput("PRAWN Suit cycle torpedo")
                .WithKeyboardBinding(InputPaths.Keyboard.R)
                .WithControllerBinding(InputPaths.Gamepad.DpadDown)
                .WithCategory("Tweaks");

            PRAWNSuitLightsToggleButton = EnumHandler.AddEntry<GameInput.Button>("PRAWNSuitLightsToggleButton")
                .CreateInput("PRAWN Suit lights toggle")
                .WithKeyboardBinding(InputPaths.Keyboard.L)
                .WithControllerBinding(InputPaths.Gamepad.RightBumper)
                .WithCategory("Tweaks");

            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
            MiscTweaks.ApplyAllTweaks();
            SaveData = SaveDataHandler.RegisterSaveDataCache<SaveData>();
        }
    }
}