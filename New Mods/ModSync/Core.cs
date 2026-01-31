using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using Ungeziefi.ModSync.UI;
using UWE;

namespace Ungeziefi.ModSync
{
    [HarmonyPatch]
    public class ModSync
    {
        #region Save/Load Integration
        [HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.SaveGame)), HarmonyPostfix]
        public static void IngameMenu_SaveGame()
        {
            ModSyncManager.SaveModList(SaveLoadManager.GetTemporarySavePath());
        }

        [HarmonyPatch(typeof(MainMenuLoadButton), nameof(MainMenuLoadButton.Load)), HarmonyPrefix]
        public static bool MainMenuLoadButton_Load(MainMenuLoadButton __instance)
        {
            CoroutineHost.StartCoroutine(CheckModsAndLoad(__instance));
            return false;
        }

        private static IEnumerator CheckModsAndLoad(MainMenuLoadButton button)
        {
            // Load ModSync.json file from the save slot
            ModListData savedMods = null;
            yield return LoadModListFromSave(button.saveGame, result => savedMods = result);

            // If no mod list found, load normally
            if (savedMods == null)
            {
                yield return uGUI_MainMenu.main.LoadGameAsync(button.saveGame, button.sessionId, button.changeSet, button.gameMode);
                yield break;
            }

            // Compare mod lists
            ModListData currentMods = ModSyncManager.GetCurrentModList();
            ModComparison comparison = ModSyncManager.CompareModLists(savedMods, currentMods);

            // No differences, load normally
            if (!comparison.HasDifferences)
            {
                yield return uGUI_MainMenu.main.LoadGameAsync(button.saveGame, button.sessionId, button.changeSet, button.gameMode);
                yield break;
            }

            // Warning dialog
            bool shouldLoad = false;
            yield return ShowModDifferenceDialog(comparison, result => shouldLoad = result);

            // Load if confirmed
            if (shouldLoad)
            {
                yield return uGUI_MainMenu.main.LoadGameAsync(button.saveGame, button.sessionId, button.changeSet, button.gameMode);
            }
        }
        #endregion

        #region Helper Methods
        private static IEnumerator LoadModListFromSave(string saveSlot, System.Action<ModListData> callback)
        {
            UserStorage userStorage = PlatformUtils.main.GetUserStorage();
            List<string> filesToLoad = new() { "ModSync.json" };

            UserStorageUtils.LoadOperation loadOp = userStorage.LoadFilesAsync(saveSlot, filesToLoad);
            yield return loadOp;

            if (loadOp.GetSuccessful() && loadOp.files.ContainsKey("ModSync.json"))
            {
                byte[] fileData = loadOp.files["ModSync.json"];
                ModListData modList = ModSyncManager.ParseModListFromBytes(fileData);

                callback(modList);
            }
        }

        private static IEnumerator ShowModDifferenceDialog(ModComparison comparison, System.Action<bool> callback)
        {
            string message = ModSyncManager.FormatModComparison(comparison);
            yield return ModDifferenceDialog.Show(message, callback);
        }
        #endregion
    }
}