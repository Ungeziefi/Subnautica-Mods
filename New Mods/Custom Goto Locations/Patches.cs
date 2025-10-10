using System;
using System.Linq;
using HarmonyLib;
using TeleportLocation = Ungeziefi.Custom_Goto_Locations.Config.TeleportLocation;

namespace Ungeziefi.Custom_Goto_Locations
{
    public partial class CustomGoto
    {
        [HarmonyPatch(typeof(GotoConsoleCommand), nameof(GotoConsoleCommand.HandleGotoCommand))]
        private static class GotoHandleCommandPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(NotificationCenter.Notification n, bool gotoImmediate, GotoConsoleCommand __instance)
            {
                if (!Main.Config.EnableFeature) return true;

                if (n.data == null || n.data.Count != 1)
                {
                    ErrorMessage.AddMessage(Messages.UsageVanillaGoto);
                    bool hasLocations = ListVanillaLocations(string.Empty);
                    hasLocations |= ListCustomLocations(string.Empty);
                    return false;
                }

                string locationName = (string)n.data[0];

                // Try vanilla locations first
                if (__instance.data != null && __instance.data.locations != null)
                {
                    foreach (var vanillaLocation in __instance.data.locations)
                    {
                        if (string.Equals(vanillaLocation.name, locationName, StringComparison.OrdinalIgnoreCase))
                        {
                            ErrorMessage.AddMessage(string.Format(Messages.JumpingToLocation,
                                locationName, vanillaLocation.position));

                            __instance.GotoPosition(vanillaLocation.position, gotoImmediate);
                            return false;
                        }
                    }
                }

                // Then check custom locations
                TeleportLocation customLocation = Main.Config.CustomTeleportLocations
                    .FirstOrDefault(loc => string.Equals(loc.Name, locationName, StringComparison.OrdinalIgnoreCase));

                if (customLocation != null)
                {
                    ErrorMessage.AddMessage(string.Format(Messages.JumpingToLocation,
                        locationName, customLocation.Position));

                    __instance.GotoPosition(customLocation.Position, gotoImmediate);
                    return false;
                }

                // No exact match found - list all matching locations
                ErrorMessage.AddMessage(string.Format(Messages.LocationsMatching, locationName));
                ListLocationsMatching(locationName);
                return false;
            }
        }

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
    }
}