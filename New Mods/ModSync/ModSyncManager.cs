using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx.Bootstrap;
using Newtonsoft.Json;

namespace Ungeziefi.ModSync
{
    public static class ModSyncManager
    {
        private const string ModSyncFileName = "ModSync.json";

        public static ModListData GetCurrentModList()
        {
            ModListData modList = new();

            foreach (var plugin in Chainloader.PluginInfos)
            {
                var metadata = plugin.Value.Metadata;
                ModInfo modInfo = new(metadata.Name, metadata.Version.ToString());
                modList.Mods.Add(modInfo);
            }

            modList.NumberOfMods = modList.Mods.Count;
            return modList;
        }

        #region Save/Load
        public static void SaveModList(string savePath)
        {
            try
            {
                ModListData currentMods = GetCurrentModList();
                string filePath = Path.Combine(savePath, ModSyncFileName);
                string json = JsonConvert.SerializeObject(currentMods, Formatting.Indented);

                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Main.Logger.LogError($"Failed to save mod list: {ex.Message}");
            }
        }

        public static ModListData LoadModList(string savePath)
        {
            try
            {
                string filePath = Path.Combine(savePath, ModSyncFileName);

                if (!File.Exists(filePath)) return null;

                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<ModListData>(json);
            }
            catch (Exception ex)
            {
                Main.Logger.LogError($"Failed to load mod list: {ex.Message}");
                return null;
            }
        }

        public static ModListData ParseModListFromBytes(byte[] fileData)
        {
            try
            {
                string content = Encoding.UTF8.GetString(fileData);
                return JsonConvert.DeserializeObject<ModListData>(content);
            }
            catch (Exception ex)
            {
                Main.Logger.LogError($"Failed to parse mod list from bytes: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region Comparison
        public static ModComparison CompareModLists(ModListData savedMods, ModListData currentMods)
        {
            ModComparison comparison = new();

            if (savedMods == null || currentMods == null)
                return comparison;

            Dictionary<string, ModInfo> savedDict = savedMods.Mods.ToDictionary(m => m.Name);
            Dictionary<string, ModInfo> currentDict = currentMods.Mods.ToDictionary(m => m.Name);

            // Find added mods
            foreach (var current in currentMods.Mods)
            {
                if (!savedDict.ContainsKey(current.Name))
                {
                    comparison.Added.Add(current);
                }
            }

            // Find removed and updated mods
            foreach (var saved in savedMods.Mods)
            {
                if (!currentDict.TryGetValue(saved.Name, out ModInfo current))
                {
                    comparison.Removed.Add(saved);
                }
                else if (saved.Version != current.Version)
                {
                    comparison.VersionChanged.Add((saved, current));
                }
            }

            Main.Logger.LogInfo($"Comparison: {comparison.Added.Count} added, " +
                              $"{comparison.Removed.Count} removed, " +
                              $"{comparison.VersionChanged.Count} updated");
            return comparison;
        }

        public static string FormatModComparison(ModComparison comparison)
        {
            StringBuilder message = new();

            // Warning
            message.AppendLine(
                "Differences were found between the currently loaded mods and those used when this save was created. While updates and additions are generally safe, removed mods may cause unpredictable and potentially game-breaking side effects.\n\n" +
                "Regarding updated mods - consult their description and change log to understand what has changed and whether it may impact your save.\n\n" +
                "Continue anyway?\n"
            );

            // Added mods
            if (comparison.Added.Count > 0)
            {
                message.AppendLine("<color=#FFAC09FF><b>Added</b></color>");
                foreach (var mod in comparison.Added)
                {
                    message.AppendLine($"{mod.Name} (v{mod.Version})");
                }
                message.AppendLine(new string('\u2500', 34));
            }

            // Removed mods
            if (comparison.Removed.Count > 0)
            {
                message.AppendLine("<color=#FFAC09FF><b>Removed</b></color>");
                foreach (var mod in comparison.Removed)
                {
                    message.AppendLine($"{mod.Name} (v{mod.Version})");
                }
                message.AppendLine(new string('\u2500', 34));
            }

            // Updated mods
            if (comparison.VersionChanged.Count > 0)
            {
                message.AppendLine("<color=#FFAC09FF><b>Updated</b></color>");
                foreach (var (oldMod, newMod) in comparison.VersionChanged)
                {
                    message.AppendLine($"{oldMod.Name}: v{oldMod.Version} → v{newMod.Version}");
                }
                message.AppendLine();
            }

            return message.ToString();
        }
        #endregion
    }
}