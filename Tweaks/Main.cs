using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace Ungeziefi.Tweaks
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Tweaks";
        public const string PLUGIN_NAME = "Tweaks";
        public const string PLUGIN_VERSION = "1.0.0";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }
        internal static new Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        public void Awake()
        {
            // Set project-scoped logger instance
            Logger = base.Logger;

            // Register harmony patches
            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");

            // Apply all miscellaneous tweaks
            MiscTweaks.ApplyAllTweaks();

            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
        }
    }
}