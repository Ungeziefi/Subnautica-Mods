using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using InputPaths = Nautilus.Handlers.GameInputHandler.Paths;

namespace Ungeziefi.Container_Utilities
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Container_Utilities";
        public const string PLUGIN_NAME = "Container Utilities";
        public const string PLUGIN_VERSION = "1.0.0";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }
        internal static new Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        public static GameInput.Button TransferAllItemsButton;
        public static GameInput.Button TransferAllSimilarItemsButton;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            TransferAllItemsButton = EnumHandler.AddEntry<GameInput.Button>("TransferAllItemsButton")
                .CreateInput("Transfer all items", "Hold this key and click an item to transfer all items.")
                .WithKeyboardBinding(InputPaths.Keyboard.Shift)
                .WithCategory("Container Utilities");

            TransferAllSimilarItemsButton = EnumHandler.AddEntry<GameInput.Button>("TransferAllSimilarItemsButton")
                .CreateInput("Transfer all similar items", "Hold this key and click an item to transfer all similar items.")
                .WithKeyboardBinding(InputPaths.Keyboard.Ctrl)
                .WithCategory("Container Utilities");

            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
        }
    }
}