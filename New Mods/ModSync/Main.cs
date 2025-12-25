using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace Ungeziefi.ModSync
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.ModSync";
        public const string PLUGIN_NAME = "ModSync";
        public const string PLUGIN_VERSION = "1.0.1";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
        }
    }
}