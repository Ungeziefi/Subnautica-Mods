﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using System.Reflection;

namespace Ungeziefi.Cuddlefish_Renamer
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "Ungeziefi.Cuddlefish_Renamer";
        public const string PLUGIN_NAME = "Cuddlefish Renamer";
        public const string PLUGIN_VERSION = "1.2.1";

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        internal static new ManualLogSource Logger { get; private set; }
        internal static new Config Config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
        internal static SaveData SaveData { get; private set; }

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            Harmony.CreateAndPatchAll(Assembly, $"{PLUGIN_GUID}");
            SaveData = SaveDataHandler.RegisterSaveDataCache<SaveData>();
        }
    }
}