using HarmonyLib;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class NewCommands : MonoBehaviour
    {
        #region Messages
        private const string HealedPlayer = "Player health restored to 100%";
        private const string RestoredHunger = "Player hunger restored to 100%";
        private const string RestoredWater = "Player water restored to 100%";
        private const string RestoredAll = "Player health, hunger, and water restored to 100%";
        private const string CommandDisabled = "This command is disabled in the Tweaks settings";
        #endregion

        #region Initialization
        private static NewCommands instance;

        [HarmonyPatch(typeof(Player), nameof(Player.Start)), HarmonyPostfix]
        public static void Player_Start()
        {
            if (instance == null)
            {
                GameObject commandsObject = new("TweaksCommandsManager");
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

        #region Command Handlers
        private void OnConsoleCommand_restorehealth()
        {
            if (!Main.Config.RestoreHealth)
            {
                ErrorMessage.AddMessage(CommandDisabled);
                return;
            }

            Player.main.liveMixin.health = Player.main.liveMixin.maxHealth;

            ErrorMessage.AddMessage(HealedPlayer);
        }

        private void OnConsoleCommand_restorehunger()
        {
            if (!Main.Config.RestoreHunger)
            {
                ErrorMessage.AddMessage(CommandDisabled);
                return;
            }

            Player.main.GetComponent<Survival>().food = 100f;

            ErrorMessage.AddMessage(RestoredHunger);
        }

        private void OnConsoleCommand_restorethirst()
        {
            if (!Main.Config.RestoreThirst)
            {
                ErrorMessage.AddMessage(CommandDisabled);
                return;
            }

            Player.main.GetComponent<Survival>().water = 100f;

            ErrorMessage.AddMessage(RestoredWater);
        }

        private void OnConsoleCommand_restoreall()
        {
            if (!Main.Config.RestoreAll)
            {
                ErrorMessage.AddMessage(CommandDisabled);
                return;
            }

            Player.main.liveMixin.health = Player.main.liveMixin.maxHealth;
            Player.main.GetComponent<Survival>().food = 100f;
            Player.main.GetComponent<Survival>().water = 100f;

            ErrorMessage.AddMessage(RestoredAll);
        }

        private void OnConsoleCommand_qqq()
        {
            if (!Main.Config.QQQ)
            {
                ErrorMessage.AddMessage(CommandDisabled);
                return;
            }

            IngameMenu.main.QuitGame(true);
        }
        #endregion
    }
}