using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using System.Reflection;
using InputPaths = Nautilus.Handlers.GameInputHandler.Paths;

namespace Ungeziefi.Cockpit_Free_Look
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Cockpit_Free_Look";
        public const string PLUGIN_NAME = "Cockpit Free Look";
        public const string PLUGIN_VERSION = "2.1.0";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }
        internal static new Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        public static GameInput.Button FreeLookButton;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            FreeLookButton = EnumHandler.AddEntry<GameInput.Button>("FreeLookButton")
                .CreateInput("Toggle free look")
                .WithKeyboardBinding(InputPaths.Keyboard.F)
                .WithControllerBinding(InputPaths.Gamepad.DpadUp)
                .WithCategory("Cockpit Free Look")
                .AvoidConflicts();

            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
        }
    }
}