using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace Ungeziefi.Rotatable_Ladders;

[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Main : BaseUnityPlugin
{
    public const string PLUGIN_GUID = "Ungeziefi.Rotatable_Ladders";
    public const string PLUGIN_NAME = "Rotatable Ladders";
    public const string PLUGIN_VERSION = "2.2.1";

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
    internal new static ManualLogSource Logger { get; private set; }
    internal new static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
    internal static SaveData SaveData { get; private set; }
    internal static Main Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");


        Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
        SaveData = SaveDataHandler.RegisterSaveDataCache<SaveData>();
    }
}