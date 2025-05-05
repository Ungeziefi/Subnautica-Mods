using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TeleportLocation = Ungeziefi.Custom_Goto_Locations.Config.TeleportLocation;

namespace Ungeziefi.Custom_Goto_Locations
{
    public static class Messages
    {
        // Usage messages
        public const string UsageAddGoto = "Usage: addgoto <name>";
        public const string UsageDeleteGoto = "Usage: deletegoto <name>";
        public const string UsageRenameGoto = "Usage: renamegoto <oldname> <newname>";
        public const string UsageCustomGoto = "Usage: customgoto <name> or cgoto <name>";
        public const string UsageListGoto = "Usage: listgoto [filter]";
        public const string UsageListCustomGoto = "Usage: listcustomgoto [filter]";

        // Success messages
        public const string CreatedLocation = "Created goto location '{0}' at {1}";
        public const string RemovedLocation = "Removed goto location '{0}'";
        public const string RenamedLocation = "Renamed goto location from '{0}' to '{1}'";
        public const string JumpingToLocation = "Jumping to {0} at {1}";

        // Error messages
        public const string LocationExists = "A location named '{0}' already exists";
        public const string LocationNotFound = "No location named '{0}' found";
        public const string VanillaLocationReadOnly = "Vanilla locations are read-only and cannot be modified";

        // List messages
        public const string NoLocationsSet = "No custom goto locations have been set.";
        public const string NoLocationsMatching = "No locations matching '{0}' found.";
        public const string AvailableLocations = "Available goto locations:";
        public const string AvailableCustomLocations = "Available custom goto locations:";
        public const string AvailableVanillaLocations = "Available vanilla goto locations:";
        public const string LocationsMatching = "LOCATIONS MATCHING: {0}";
        public const string VanillaLocationsMatching = "VANILLA LOCATIONS: ";
        public const string CustomLocationsMatching = "CUSTOM LOCATIONS: ";
        public const string LocationSeparator = " - ";
    }

    [HarmonyPatch]
    public class CustomGoto : MonoBehaviour
    {
        #region Initialization
        private static CustomGoto instance;

        [HarmonyPatch(typeof(Player), nameof(Player.Start)), HarmonyPostfix]
        public static void Player_Start(Player __instance)
        {
            if (!Main.Config.EnableFeature) return;

            if (instance == null)
            {
                GameObject customGotoObject = new GameObject("CustomGotoManager");
                instance = customGotoObject.AddComponent<CustomGoto>();
                DontDestroyOnLoad(customGotoObject);
            }
        }

        private void Awake()
        {
            string[] commands = {
                "addgoto", "renamegoto", "deletegoto",
                "customgoto", "cgoto",
                "customgotofast", "cgotofast",
                "listcustomgoto", "listvanillagoto", "listgoto"
            };

            foreach (var cmd in commands)
            {
                DevConsole.RegisterConsoleCommand(this, cmd, false, false);
            }
        }
        #endregion

        #region AddGoto
        private void OnConsoleCommand_addgoto(NotificationCenter.Notification n)
        {
            if (!Main.Config.EnableFeature) return;

            if (!ValidateCommandParams(n, 1, Messages.UsageAddGoto)) return;

            string locationName = (string)n.data[0];

            // Check if the name exists in vanilla locations
            if (IsVanillaLocation(locationName))
            {
                ErrorMessage.AddMessage(string.Format(Messages.VanillaLocationReadOnly, locationName));
                return;
            }

            // Check if the name exists in custom locations
            if (LocationExists(locationName))
            {
                ErrorMessage.AddMessage(string.Format(Messages.LocationExists, locationName));
                return;
            }

            Vector3 playerPosition = Player.main.transform.position;
            SaveLocation(locationName, playerPosition);
            ErrorMessage.AddMessage(string.Format(Messages.CreatedLocation, locationName, playerPosition));
        }
        #endregion

        #region Listgoto
        private void OnConsoleCommand_listgoto(NotificationCenter.Notification n)
        {
            if (!Main.Config.EnableFeature) return;

            string filter = (n.data != null && n.data.Count > 0) ? (string)n.data[0] : string.Empty;

            // List both vanilla and custom locations
            bool hasLocations = ListVanillaLocations(filter);
            hasLocations |= ListCustomLocations(filter);

            if (!hasLocations)
            {
                ErrorMessage.AddMessage(string.Format(Messages.NoLocationsMatching, filter));
            }
        }

        private void OnConsoleCommand_listvanillagoto(NotificationCenter.Notification n)
        {
            if (!Main.Config.EnableFeature) return;

            string filter = (n.data != null && n.data.Count > 0) ? (string)n.data[0] : string.Empty;
            ListVanillaLocations(filter);
        }

        private void OnConsoleCommand_listcustomgoto(NotificationCenter.Notification n)
        {
            if (!Main.Config.EnableFeature) return;

            string filter = (n.data != null && n.data.Count > 0) ? (string)n.data[0] : string.Empty;
            ListCustomLocations(filter);
        }
        #endregion

        #region DeleteGoto
        private void OnConsoleCommand_deletegoto(NotificationCenter.Notification n)
        {
            if (!Main.Config.EnableFeature) return;

            if (!ValidateCommandParams(n, 1, Messages.UsageDeleteGoto)) return;

            string locationName = (string)n.data[0];

            // Check if it's a vanilla location
            if (IsVanillaLocation(locationName))
            {
                ErrorMessage.AddMessage(string.Format(Messages.VanillaLocationReadOnly, locationName));
                return;
            }

            TeleportLocation location = FindLocation(locationName);

            if (location == null)
            {
                ErrorMessage.AddMessage(string.Format(Messages.LocationNotFound, locationName));
                return;
            }

            Main.Config.CustomTeleportLocations.Remove(location);
            Main.SaveConfig();

            ErrorMessage.AddMessage(string.Format(Messages.RemovedLocation, locationName));
        }
        #endregion

        #region RenameGoto
        private void OnConsoleCommand_renamegoto(NotificationCenter.Notification n)
        {
            if (!Main.Config.EnableFeature) return;

            if (!ValidateCommandParams(n, 2, Messages.UsageRenameGoto)) return;

            string oldName = (string)n.data[0];
            string newName = (string)n.data[1];

            // Check if it's a vanilla location
            if (IsVanillaLocation(oldName))
            {
                ErrorMessage.AddMessage(string.Format(Messages.VanillaLocationReadOnly, oldName));
                return;
            }

            TeleportLocation location = FindLocation(oldName);

            if (location == null)
            {
                ErrorMessage.AddMessage(string.Format(Messages.LocationNotFound, oldName));
                return;
            }

            // Check if new name conflicts with vanilla
            if (IsVanillaLocation(newName))
            {
                ErrorMessage.AddMessage(string.Format(Messages.LocationExists, newName));
                return;
            }

            if (LocationExists(newName, oldName))
            {
                ErrorMessage.AddMessage(string.Format(Messages.LocationExists, newName));
                return;
            }

            location.Name = newName;
            Main.SaveConfig();

            ErrorMessage.AddMessage(string.Format(Messages.RenamedLocation, oldName, newName));
        }
        #endregion

        #region Goto
        private void OnConsoleCommand_customgoto(NotificationCenter.Notification n) => HandleCustomGoto(n);
        private void OnConsoleCommand_cgoto(NotificationCenter.Notification n) => HandleCustomGoto(n);

        private void HandleCustomGoto(NotificationCenter.Notification n)
        {
            if (!Main.Config.EnableFeature) return;

            if (n.data == null || n.data.Count < 1)
            {
                ErrorMessage.AddMessage(Messages.UsageCustomGoto);
                ListCustomLocations(string.Empty);
                return;
            }

            string locationName = (string)n.data[0];
            var location = FindLocation(locationName);

            if (location == null)
            {
                ErrorMessage.AddMessage(string.Format(Messages.LocationsMatching, locationName));
                ListLocationsMatching(locationName);
                return;
            }

            ErrorMessage.AddMessage(string.Format(Messages.JumpingToLocation, locationName, location.Position));
            GotoConsoleCommand.main.GotoPosition(location.Position, false);
        }
        #endregion

        #region Gotofast
        private void OnConsoleCommand_customgotofast(NotificationCenter.Notification n) => HandleCustomGotoFast(n);
        private void OnConsoleCommand_cgotofast(NotificationCenter.Notification n) => HandleCustomGotoFast(n);

        private void HandleCustomGotoFast(NotificationCenter.Notification n)
        {
            if (!Main.Config.EnableFeature) return;

            if (n.data == null || n.data.Count < 1)
            {
                ErrorMessage.AddMessage(Messages.UsageCustomGoto);
                ListCustomLocations(string.Empty);
                return;
            }

            string locationName = (string)n.data[0];
            var location = FindLocation(locationName);

            if (location == null)
            {
                ErrorMessage.AddMessage(string.Format(Messages.LocationsMatching, locationName));
                ListLocationsMatching(locationName);
                return;
            }

            ErrorMessage.AddMessage(string.Format(Messages.JumpingToLocation, locationName, location.Position));
            GotoConsoleCommand.main.GotoPosition(location.Position, true);
        }
        #endregion

        #region Helper Methods
        private static bool ListCustomLocations(string filter)
        {
            if (Main.Config.CustomTeleportLocations.Count == 0)
            {
                ErrorMessage.AddMessage(Messages.NoLocationsSet);
                return false;
            }

            bool hasFilter = !string.IsNullOrEmpty(filter);
            List<TeleportLocation> filteredLocations = hasFilter
                ? Main.Config.CustomTeleportLocations
                    .Where(loc => loc.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList()
                : Main.Config.CustomTeleportLocations;

            if (filteredLocations.Count == 0)
            {
                if (hasFilter)
                {
                    return false;
                }
                ErrorMessage.AddMessage(Messages.NoLocationsSet);
                return false;
            }

            ErrorMessage.AddMessage(hasFilter
                ? string.Format(Messages.LocationsMatching, filter)
                : Messages.AvailableCustomLocations);

            // Format locations in a single line with separation
            StringBuilder sb = new StringBuilder();
            foreach (var location in filteredLocations)
            {
                if (sb.Length > 0)
                    sb.Append(Messages.LocationSeparator);
                sb.Append(location.Name);
            }

            ErrorMessage.AddMessage(sb.ToString());
            return filteredLocations.Count > 0;
        }

        private static bool ListVanillaLocations(string filter)
        {
            if (GotoConsoleCommand.main == null || GotoConsoleCommand.main.data == null ||
                GotoConsoleCommand.main.data.locations == null || GotoConsoleCommand.main.data.locations.Length == 0)
            {
                return false;
            }

            bool hasFilter = !string.IsNullOrEmpty(filter);
            var vanillaLocations = GotoConsoleCommand.main.data.locations;

            var filteredLocations = hasFilter
                ? vanillaLocations.Where(loc =>
                    loc.name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0).ToArray()
                : vanillaLocations;

            if (filteredLocations.Length == 0)
            {
                return false;
            }

            ErrorMessage.AddMessage(hasFilter
                ? string.Format(Messages.LocationsMatching, filter)
                : Messages.AvailableVanillaLocations);

            // Format locations in a single line with separation
            StringBuilder sb = new StringBuilder();
            foreach (var location in filteredLocations)
            {
                if (sb.Length > 0)
                    sb.Append(Messages.LocationSeparator);
                sb.Append(location.name);
            }

            ErrorMessage.AddMessage(sb.ToString());
            return filteredLocations.Length > 0;
        }

        // Used when a location isn't found during goto
        private static void ListLocationsMatching(string filter)
        {
            bool hasVanillaMatches = false;
            bool hasCustomMatches = false;
            StringBuilder vanillaSb = new StringBuilder();
            StringBuilder customSb = new StringBuilder();

            // Get matching vanilla locations
            if (GotoConsoleCommand.main?.data?.locations != null)
            {
                var matchingVanilla = GotoConsoleCommand.main.data.locations
                    .Where(loc => loc.name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToArray();

                foreach (var location in matchingVanilla)
                {
                    if (vanillaSb.Length > 0)
                        vanillaSb.Append(Messages.LocationSeparator);
                    vanillaSb.Append(location.name);
                    hasVanillaMatches = true;
                }
            }

            // Get matching custom locations
            if (Main.Config.CustomTeleportLocations.Count > 0)
            {
                var matchingCustom = Main.Config.CustomTeleportLocations
                    .Where(loc => loc.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                foreach (var location in matchingCustom)
                {
                    if (customSb.Length > 0)
                        customSb.Append(Messages.LocationSeparator);
                    customSb.Append(location.Name);
                    hasCustomMatches = true;
                }
            }

            // Results
            if (hasVanillaMatches)
            {
                ErrorMessage.AddMessage(Messages.VanillaLocationsMatching + vanillaSb.ToString());
            }
            if (hasCustomMatches)
            {
                ErrorMessage.AddMessage(Messages.CustomLocationsMatching + customSb.ToString());
            }
        }

        private bool ValidateCommandParams(NotificationCenter.Notification n, int requiredParams, string usageMessage)
        {
            if (n.data == null || n.data.Count < requiredParams)
            {
                ErrorMessage.AddMessage(usageMessage);
                return false;
            }
            return true;
        }

        private bool IsVanillaLocation(string name)
        {
            if (GotoConsoleCommand.main == null || GotoConsoleCommand.main.data == null ||
                GotoConsoleCommand.main.data.locations == null)
                return false;

            return GotoConsoleCommand.main.data.locations
                .Any(loc => string.Equals(loc.name, name, StringComparison.OrdinalIgnoreCase));
        }

        private bool LocationExists(string name, string excludeName = null)
        {
            return Main.Config.CustomTeleportLocations.Any(loc =>
                string.Equals(loc.Name, name, StringComparison.OrdinalIgnoreCase) &&
                (excludeName == null || !string.Equals(loc.Name, excludeName, StringComparison.OrdinalIgnoreCase)));
        }

        private TeleportLocation FindLocation(string name)
        {
            return Main.Config.CustomTeleportLocations
                .FirstOrDefault(loc => string.Equals(loc.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private void SaveLocation(string name, Vector3 position)
        {
            TeleportLocation newLocation = new TeleportLocation(name, position);
            Main.Config.CustomTeleportLocations.Add(newLocation);
            Main.SaveConfig();
        }
        #endregion

        #region Harmony Patches
        // Replace HandleGotoCommand
        [HarmonyPatch(typeof(GotoConsoleCommand), nameof(GotoConsoleCommand.HandleGotoCommand))]
        private static class GotoHandleCommandPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(NotificationCenter.Notification n, bool gotoImmediate, GotoConsoleCommand __instance)
            {
                if (!Main.Config.EnableFeature) return true;

                if (n.data == null || n.data.Count != 1)
                {
                    // List both vanilla and custom locations
                    bool hasLocations = ListVanillaLocations(string.Empty);
                    hasLocations |= ListCustomLocations(string.Empty);
                    return false;
                }

                string locationName = (string)n.data[0];

                // Try custom locations first
                TeleportLocation customLocation = Main.Config.CustomTeleportLocations
                    .FirstOrDefault(loc => string.Equals(loc.Name, locationName, StringComparison.OrdinalIgnoreCase));

                if (customLocation != null)
                {
                    // Use custom location
                    ErrorMessage.AddMessage(string.Format(Messages.JumpingToLocation,
                        locationName, customLocation.Position));

                    __instance.GotoPosition(customLocation.Position, gotoImmediate);
                    return false;
                }

                // Try vanilla locations
                if (__instance.data != null && __instance.data.locations != null)
                {
                    foreach (var vanillaLocation in __instance.data.locations)
                    {
                        if (string.Equals(vanillaLocation.name, locationName, StringComparison.OrdinalIgnoreCase))
                        {
                            // Use vanilla location with our custom message
                            ErrorMessage.AddMessage(string.Format(Messages.JumpingToLocation,
                                locationName, vanillaLocation.position));

                            __instance.GotoPosition(vanillaLocation.position, gotoImmediate);
                            return false;
                        }
                    }
                }

                // No matching location found - list matching locations
                ErrorMessage.AddMessage(string.Format(Messages.LocationsMatching, locationName));

                // Use differentiated listing
                ListLocationsMatching(locationName);
                return false;
            }
        }

        // Replace OnConsoleCommand_goto
        [HarmonyPatch(typeof(GotoConsoleCommand), nameof(GotoConsoleCommand.OnConsoleCommand_goto))]
        private static class GotoCommandPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(NotificationCenter.Notification n, GotoConsoleCommand __instance)
            {
                if (!Main.Config.EnableFeature) return true;

                __instance.HandleGotoCommand(n, false);
                return false;
            }
        }

        // Replace OnConsoleCommand_gotofast to use our custom handler
        [HarmonyPatch(typeof(GotoConsoleCommand), nameof(GotoConsoleCommand.OnConsoleCommand_gotofast))]
        private static class GotoFastCommandPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(NotificationCenter.Notification n, GotoConsoleCommand __instance)
            {
                if (!Main.Config.EnableFeature) return true;

                __instance.HandleGotoCommand(n, true);
                return false;
            }
        }
        #endregion
    }
}