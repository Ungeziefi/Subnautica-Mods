using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class NewCommands : MonoBehaviour
    {
        #region Initialization
        private static NewCommands instance;

        [HarmonyPatch(typeof(Player), nameof(Player.Start)), HarmonyPostfix]
        public static void Player_Start(Player __instance)
        {
            if (instance == null)
            {
                GameObject commandsObject = new GameObject("TweaksCommandsManager");
                instance = commandsObject.AddComponent<NewCommands>();
                DontDestroyOnLoad(commandsObject);
            }
        }

        [System.Obsolete]
        private void Awake()
        {
            string[] commands = {
                "restorehealth",
                "restorehunger",
                "restorethirst",
                "restoreall",
                "qqq"
            };

            foreach (var cmd in commands)
            {
                DevConsole.RegisterConsoleCommand(this, cmd, false, false);
            }
        }
        #endregion

        #region Messages
        private const string HealedPlayer = "Player health restored to 100%";
        private const string RestoredHunger = "Player hunger restored to 100%";
        private const string RestoredWater = "Player water restored to 100%";
        private const string RestoredAll = "Player health, hunger, and water restored to 100%";

        private const string PlayerNotFound = "Player not found";
        private const string CommandDisabled = "This command is disabled in the Tweaks settings";
        #endregion

        #region Command Handlers
        private void OnConsoleCommand_restorehealth(NotificationCenter.Notification n)
        {
            if (!Main.Config.RestoreHealth)
            {
                ErrorMessage.AddMessage(CommandDisabled);
                return;
            }

            if (Player.main == null)
            {
                ErrorMessage.AddMessage(PlayerNotFound);
                return;
            }

            Player.main.liveMixin.health = Player.main.liveMixin.maxHealth;

            ErrorMessage.AddMessage(HealedPlayer);
        }

        private void OnConsoleCommand_restorehunger(NotificationCenter.Notification n)
        {
            if (!Main.Config.RestoreHunger)
            {
                ErrorMessage.AddMessage(CommandDisabled);
                return;
            }

            if (Player.main == null || Player.main.GetComponent<Survival>() == null)
            {
                ErrorMessage.AddMessage(PlayerNotFound);
                return;
            }

            Player.main.GetComponent<Survival>().food = 100f;

            ErrorMessage.AddMessage(RestoredHunger);
        }

        private void OnConsoleCommand_restorethirst(NotificationCenter.Notification n)
        {
            if (!Main.Config.RestoreThirst)
            {
                ErrorMessage.AddMessage(CommandDisabled);
                return;
            }

            if (Player.main == null || Player.main.GetComponent<Survival>() == null)
            {
                ErrorMessage.AddMessage(PlayerNotFound);
                return;
            }

            Player.main.GetComponent<Survival>().water = 100f;

            ErrorMessage.AddMessage(RestoredWater);
        }

        private void OnConsoleCommand_restoreall(NotificationCenter.Notification n)
        {
            if (!Main.Config.RestoreAll)
            {
                ErrorMessage.AddMessage(CommandDisabled);
                return;
            }

            if (Player.main == null || Player.main.GetComponent<Survival>() == null)
            {
                ErrorMessage.AddMessage(PlayerNotFound);
                return;
            }

            Player.main.liveMixin.health = Player.main.liveMixin.maxHealth;
            Player.main.GetComponent<Survival>().food = 100f;
            Player.main.GetComponent<Survival>().water = 100f;

            ErrorMessage.AddMessage(RestoredAll);
        }

        private void OnConsoleCommand_qqq(NotificationCenter.Notification n)
        {
            if (!Main.Config.QQQ)
            {
                ErrorMessage.AddMessage(CommandDisabled);
                return;
            }

            IngameMenu menu = IngameMenu.main;
            if (menu != null)
            {
                menu.QuitGame(true);
            }
        }
        #endregion
    }
}