using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace Ungeziefi.Stasis_Rifle_Freeze_Fix;

[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Main : BaseUnityPlugin
{
    public const string PLUGIN_GUID = "Ungeziefi.Stasis_Rifle_Freeze_Fix";
    public const string PLUGIN_NAME = "Stasis Rifle Freeze Fix";
    public const string PLUGIN_VERSION = "1.0.0"; // Unused - won't release

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
    internal new static ManualLogSource Logger { get; private set; }
    internal new static Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

    public void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

        Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
    }
}