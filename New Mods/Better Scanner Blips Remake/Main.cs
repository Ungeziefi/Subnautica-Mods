using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using InputPaths = Nautilus.Handlers.GameInputHandler.Paths;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]

    // Incompatible plugins (GUID)
    [BepInIncompatibility("BetterScannerBlips")]
    [BepInIncompatibility("com.whotnt.subnautica.dynamicscannerblips.mod")]

    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Better_Scanner_Blips_Remake";
        public const string PLUGIN_NAME = "Better Scanner Blips Remake";
        public const string PLUGIN_VERSION = "2.0.0";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }
        internal static new Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        public static GameInput.Button ToggleBlipsButton;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            ToggleBlipsButton = EnumHandler.AddEntry<GameInput.Button>("ToggleBlipsButton")
                .CreateInput("Toggle blips")
                .WithKeyboardBinding(InputPaths.Keyboard.B)
                .WithCategory("Better Scanner Blips Remake");

            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
        }
    }
}