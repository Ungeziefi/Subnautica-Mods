using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Custom_Goto_Locations
{
    public static class Messages
    {
        // Usage messages
        public const string UsageAddGoto = "Usage: addgoto <name>";
        public const string UsageDeleteGoto = "Usage: deletegoto <name>";
        public const string UsageRenameGoto = "Usage: renamegoto <oldname> <newname>";
        public const string UsageVanillaGoto = "Usage: goto <name>";
        public const string UsageCustomGoto = "Usage: customgoto <name> or cgoto <name>";

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
        public const string AvailableCustomLocations = "Available custom locations:";
        public const string AvailableVanillaLocations = "Available vanilla locations:";
        public const string LocationsMatching = "Locations matching: {0}";
        public const string VanillaLocationsMatching = "Vanilla locations: ";
        public const string CustomLocationsMatching = "Custom locations: ";
        public const string LocationSeparator = " - ";
    }

    [HarmonyPatch]
    public partial class CustomGoto : MonoBehaviour
    {
        #region Initialization
        private static CustomGoto instance;

        // Initializes  CustomGoto instance when the player starts
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

        // Register new commands on CustomGotoManager Awake
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
    }
}