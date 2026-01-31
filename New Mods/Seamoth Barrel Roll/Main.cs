using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using System.Reflection;
using InputPaths = Nautilus.Handlers.GameInputHandler.Paths;

namespace Ungeziefi.Seamoth_Barrel_Roll
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Seamoth_Barrel_Roll";
        public const string PLUGIN_NAME = "Seamoth Barrel Roll";
        public const string PLUGIN_VERSION = "2.2.1";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }
        internal static new Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        public static GameInput.Button RollLeftButton;
        public static GameInput.Button RollRightButton;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            RollLeftButton = EnumHandler.AddEntry<GameInput.Button>("RollLeftButton")
                .CreateInput("Roll left")
                .WithKeyboardBinding(InputPaths.Keyboard.Ctrl)
                .WithCategory("Seamoth Barrel Roll")
                .AvoidConflicts();

            RollRightButton = EnumHandler.AddEntry<GameInput.Button>("RollRightButton")
                .CreateInput("Roll right")
                .WithKeyboardBinding(InputPaths.Keyboard.Alt)
                .WithCategory("Seamoth Barrel Roll")
                .AvoidConflicts();

            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
        }
    }
}