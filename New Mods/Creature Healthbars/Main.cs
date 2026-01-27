using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using InputPaths = Nautilus.Handlers.GameInputHandler.Paths;

namespace Ungeziefi.Creature_Healthbars
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Creature_Healthbars";
        public const string PLUGIN_NAME = "Creature Healthbars";
        public const string PLUGIN_VERSION = "2.1.1";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }
        internal static new Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        public static GameInput.Button FreezeCreaturesToggleButton;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            FreezeCreaturesToggleButton = EnumHandler.AddEntry<GameInput.Button>("FreezeCreaturesToggleButton")
                .CreateInput("Toggle frozen creatures")
                .WithKeyboardBinding(InputPaths.Keyboard.Backslash)
                .WithCategory("Creature Healthbars (Debug)")
                .AvoidConflicts();

            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
        }
    }
}