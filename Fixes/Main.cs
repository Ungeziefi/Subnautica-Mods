using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Fixes";
        public const string PLUGIN_NAME = "Fixes";
        public const string PLUGIN_VERSION = "1.0.0";

        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public void Awake()
        {
            // Set project-scoped logger instance
            Logger = base.Logger;

            // Register harmony patches
            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");

            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
        }
    }
}