using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using InputPaths = Nautilus.Handlers.GameInputHandler.Paths;

namespace Ungeziefi.Cuddlefish_Renamer;

[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Main : BaseUnityPlugin
{
    public const string PLUGIN_GUID = "Ungeziefi.Cuddlefish_Renamer";
    public const string PLUGIN_NAME = "Cuddlefish Renamer";
    public const string PLUGIN_VERSION = "2.1.1";

    public static GameInput.Button RenameCuddlefishButton;

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
    internal new static ManualLogSource Logger { get; private set; }
    internal new static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
    internal static SaveData SaveData { get; private set; }

    public void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

        RenameCuddlefishButton = EnumHandler.AddEntry<GameInput.Button>("RenameCuddlefishButton")
            .CreateInput("Rename Cuddlefish")
            .WithKeyboardBinding(InputPaths.Keyboard.R)
            .WithControllerBinding(InputPaths.Gamepad.DpadUp)
            .WithCategory("Cuddlefish Renamer")
            .AvoidConflicts();

        Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
        SaveData = SaveDataHandler.RegisterSaveDataCache<SaveData>();
    }
}