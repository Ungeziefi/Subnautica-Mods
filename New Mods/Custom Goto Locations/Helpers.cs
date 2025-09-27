using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TeleportLocation = Ungeziefi.Custom_Goto_Locations.Config.TeleportLocation;

namespace Ungeziefi.Custom_Goto_Locations
{
    public partial class CustomGoto
    {
        private static string GetFilterFromNotification(NotificationCenter.Notification n)
        {
            return (n.data != null && n.data.Count > 0) ? (string)n.data[0] : string.Empty;
        }

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

            ErrorMessage.AddMessage(Messages.AvailableCustomLocations);
            DisplayLocationList(filteredLocations.Select(loc => loc.Name));
            return true;
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

            ErrorMessage.AddMessage(Messages.AvailableVanillaLocations);
            DisplayLocationList(filteredLocations.Select(loc => loc.name));
            return true;
        }

        private static void DisplayLocationList(IEnumerable<string> locationNames)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var name in locationNames)
            {
                if (sb.Length > 0)
                    sb.Append(Messages.LocationSeparator);
                sb.Append(name);
            }
            ErrorMessage.AddMessage(sb.ToString());
        }

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

            // Display results
            if (hasVanillaMatches)
            {
                ErrorMessage.AddMessage(Messages.VanillaLocationsMatching + vanillaSb.ToString());
            }
            if (hasCustomMatches)
            {
                ErrorMessage.AddMessage(Messages.CustomLocationsMatching + customSb.ToString());
            }
        }

        private static void ListCustomMatchingLocations(string filter)
        {
            if (Main.Config.CustomTeleportLocations.Count == 0)
            {
                ErrorMessage.AddMessage(Messages.NoLocationsSet);
                return;
            }

            var matchingCustom = Main.Config.CustomTeleportLocations
                .Where(loc => loc.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (matchingCustom.Count > 0)
            {
                ErrorMessage.AddMessage(Messages.CustomLocationsMatching);
                DisplayLocationList(matchingCustom.Select(loc => loc.Name));
            }
            else
            {
                ErrorMessage.AddMessage(string.Format(Messages.NoLocationsMatching, filter));
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

        // Checks if custom location name exists
        private bool LocationExists(string name, string excludeName = null)
        {
            return Main.Config.CustomTeleportLocations.Any(loc =>
                string.Equals(loc.Name, name, StringComparison.OrdinalIgnoreCase) &&
                (excludeName == null || !string.Equals(loc.Name, excludeName, StringComparison.OrdinalIgnoreCase)));
        }

        // Finds custom location by name
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
    }
}