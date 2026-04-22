using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Ungeziefi.Fixes.Misc;

namespace Ungeziefi.Fixes;

[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
[BepInIncompatibility("qqqbbb.subnautica.tweaksAndFixes")]
public class Main : BaseUnityPlugin
{
    public const string PLUGIN_GUID = "Ungeziefi.Fixes";
    public const string PLUGIN_NAME = "Fixes";
    public const string PLUGIN_VERSION = "1.5.0";

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
    internal new static ManualLogSource Logger { get; private set; }
    internal new static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
    internal static SaveData SaveData { get; private set; }

    public void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

        Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
        MiscFixes.ApplyAllFixes();
        SaveData = SaveDataHandler.RegisterSaveDataCache<SaveData>();
        SaveLastHeldItem.RegisterLoadingTask();
    }
}