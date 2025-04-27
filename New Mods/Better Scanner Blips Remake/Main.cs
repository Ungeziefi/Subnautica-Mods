using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;

namespace Ungeziefi.Better_Scanner_Blips_Remake
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Better_Scanner_Blips_Remake";
        public const string PLUGIN_NAME = "Better Scanner Blips Remake";
        public const string PLUGIN_VERSION = "1.0.0";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }
        internal static new Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        #region Incompatible Plugins
        private static readonly Dictionary<string, string> IncompatiblePlugins = new Dictionary<string, string>
        {
            {"BetterScannerBlips", "Better Scanner Blips (Purple Edition) (Nautilus)"},
            {"com.whotnt.subnautica.dynamicscannerblips.mod", "Dynamic Scanner Blips"},
            // {"plugin.guid", "Plugin Display Name"},
        };

        private bool HasIncompatiblePlugins(out string incompatiblePluginNames)
        {
            List<string> foundIncompatiblePlugins = new List<string>();

            foreach (var plugin in IncompatiblePlugins)
            {
                if (Chainloader.PluginInfos.ContainsKey(plugin.Key))
                {
                    foundIncompatiblePlugins.Add(plugin.Value);
                }
            }

            incompatiblePluginNames = string.Join(", ", foundIncompatiblePlugins);
            return foundIncompatiblePlugins.Count > 0;
        }
        #endregion

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            if (HasIncompatiblePlugins(out string incompatiblePluginNames))
            {
                Logger.LogError($"Incompatible mod(s) detected: {incompatiblePluginNames}");
                Logger.LogError("Please remove the incompatible mod(s) to use this mod properly.");
                return; // Prevent patch loading
            }

            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
        }
    }
}