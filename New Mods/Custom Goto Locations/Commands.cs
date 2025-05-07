using System;
using System.Linq;
using UnityEngine;
using TeleportLocation = Ungeziefi.Custom_Goto_Locations.Config.TeleportLocation;

namespace Ungeziefi.Custom_Goto_Locations
{
    public partial class CustomGoto
    {
        #region AddGoto
        private void OnConsoleCommand_addgoto(NotificationCenter.Notification n)
        {
            if (!Main.Config.EnableFeature) return;

            if (!ValidateCommandParams(n, 1, Messages.UsageAddGoto)) return;

            string locationName = (string)n.data[0];

            // Only check if the name exists in custom locations
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

        #region ListGoto
        private void OnConsoleCommand_listgoto(NotificationCenter.Notification n)
        {
            if (!Main.Config.EnableFeature) return;

            string filter = GetFilterFromNotification(n);

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

            string filter = GetFilterFromNotification(n);
            ListVanillaLocations(filter);
        }

        private void OnConsoleCommand_listcustomgoto(NotificationCenter.Notification n)
        {
            if (!Main.Config.EnableFeature) return;

            string filter = GetFilterFromNotification(n);
            ListCustomLocations(filter);
        }
        #endregion

        #region DeleteGoto
        private void OnConsoleCommand_deletegoto(NotificationCenter.Notification n)
        {
            if (!Main.Config.EnableFeature) return;

            if (!ValidateCommandParams(n, 1, Messages.UsageDeleteGoto)) return;

            string locationName = (string)n.data[0];
            TeleportLocation location = FindLocation(locationName);

            if (location == null)
            {
                // If no custom location exists, check if it's a vanilla location
                if (IsVanillaLocation(locationName))
                {
                    ErrorMessage.AddMessage(string.Format(Messages.VanillaLocationReadOnly, locationName));
                    return;
                }

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
            TeleportLocation location = FindLocation(oldName);

            if (location == null)
            {
                // If no custom location exists, check if it's a vanilla location
                if (IsVanillaLocation(oldName))
                {
                    ErrorMessage.AddMessage(string.Format(Messages.VanillaLocationReadOnly, oldName));
                    return;
                }

                ErrorMessage.AddMessage(string.Format(Messages.LocationNotFound, oldName));
                return;
            }

            // Check if new name conflicts with another custom location
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
        private void OnConsoleCommand_customgoto(NotificationCenter.Notification n) => HandleCustomGoto(n, false);
        private void OnConsoleCommand_cgoto(NotificationCenter.Notification n) => HandleCustomGoto(n, false);
        private void OnConsoleCommand_customgotofast(NotificationCenter.Notification n) => HandleCustomGoto(n, true);
        private void OnConsoleCommand_cgotofast(NotificationCenter.Notification n) => HandleCustomGoto(n, true);

        private void HandleCustomGoto(NotificationCenter.Notification n, bool isFast)
        {
            if (!Main.Config.EnableFeature) return;

            if (n.data == null || n.data.Count < 1)
            {
                // No name provided - show only custom locations
                ErrorMessage.AddMessage(Messages.UsageCustomGoto);
                ListCustomLocations(string.Empty);
                return;
            }

            string locationName = (string)n.data[0];
            var location = FindLocation(locationName);

            if (location == null)
            {
                // Name provided but no exact match - look for partial matches in custom locations only
                ErrorMessage.AddMessage(string.Format(Messages.LocationsMatching, locationName));
                ListCustomMatchingLocations(locationName);
                return;
            }

            // Exact match found - teleport
            ErrorMessage.AddMessage(string.Format(Messages.JumpingToLocation, locationName, location.Position));
            GotoConsoleCommand.main.GotoPosition(location.Position, isFast);
        }
        #endregion
    }
}