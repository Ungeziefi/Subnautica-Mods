using System.Collections.Generic;
using HarmonyLib;
using Nautilus.Handlers;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    // Adds a push feature to the Seamoth
    [HarmonyPatch(typeof(Vehicle))]
    public class TweakVehiclePush
    {
        private static bool IsPushable(Vehicle vehicle)
        {
            return Language.main.GetCurrentLanguage() == "English" &&
                   Main.Config.SeamothPushing &&
                   vehicle.onGround &&
                   !Inventory.main.GetHeld() &&
                   vehicle is SeaMoth &&
                   !vehicle.docked &&
                   !Player.main.IsSwimming();
        }

        private static void Push(Vehicle vehicle)
        {
            var rb = vehicle.GetComponent<Rigidbody>();
            var direction = new Vector3(MainCameraControl.main.transform.forward.x, 0.2f, MainCameraControl.main.transform.forward.z);
            rb.AddForce(direction * 3333f, ForceMode.Impulse);
        }

        [HarmonyPatch(nameof(Vehicle.OnHandHover))]
        public static void Postfix(Vehicle __instance)
        {
            if (IsPushable(__instance))
            {
                HandReticle.main.SetText(HandReticle.TextType.Hand, "Push", false, GameInput.Button.RightHand);
                if (GameInput.GetButtonDown(GameInput.Button.RightHand))
                {
                    Push(__instance);
                }
            }
        }
    }

    // Multiplies the build time of structures
    [HarmonyPatch(typeof(Constructable))]
    public class TweakConstructableBuildTimeMultiplier
    {
        [HarmonyPatch(nameof(Constructable.GetConstructInterval))]
        public static void Postfix(ref float __result)
        {
            if (Main.Config.BuildTimeMultiplier == 1f || NoCostConsoleCommand.main.fastBuildCheat || !GameModeUtils.RequiresIngredients())
            {
                return;
            }
            __result *= Main.Config.BuildTimeMultiplier;
        }
    }

    // Multiplies the craft time of items
    [HarmonyPatch(typeof(CrafterLogic))]
    public class TweakCrafterLogicCraftTimeMultiplier
    {
        [HarmonyPatch(nameof(CrafterLogic.Craft))]
        public static void Prefix(ref float craftTime)
        {
            GameModeUtils.GetGameMode(out GameModeOption mode, out GameModeOption cheats);
            if (Main.Config.CraftTimeMultiplier == 1f || mode == GameModeOption.Creative)
            {
                return;
            }
            craftTime *= Main.Config.CraftTimeMultiplier;
        }
    }

    // Randomizes the size of creatures
    [HarmonyPatch(typeof(Creature))]
    public class TweakCreatureSizeRandomizer
    {
        private static readonly HashSet<TechType> targetCreatures = new HashSet<TechType>
                        {
                            TechType.CaveCrawler,
                            TechType.LavaLarva,
                            TechType.Bleeder,
                            TechType.Rockgrub,
                            TechType.Blighter,
                            TechType.Floater,
                        };

        [HarmonyPatch(nameof(Creature.Start))]
        public static void Postfix(Creature __instance)
        {
            var tt = CraftData.GetTechType(__instance.gameObject);
            // Main.Logger.LogInfo($"Creature TechType: {tt}");

            if (targetCreatures.Contains(tt))
            {
                float size = Random.Range(Main.Config.MinCreatureSize / 10f, Main.Config.MaxCreatureSize / 10f);
                __instance.transform.localScale *= size;
                // Main.Logger.LogInfo($"Applied size for {tt}: {__instance.transform.localScale}");
            }
        }
    }

    [HarmonyPatch(typeof(Inventory))]
    public class TweakInventoryUnderwaterLimitations
    {
        [HarmonyPatch(nameof(Inventory.GetItemAction))]
        public static void Postfix(Inventory __instance, ref ItemAction __result, InventoryItem item)
        {
            var pickupable = item.item;
            var tt = pickupable.GetTechType();

            // Disables eating underwater
            if (Main.Config.NoEatingUnderwater &&
                Player.main.IsUnderwater() &&
                __result == ItemAction.Eat &&
                pickupable.gameObject.GetComponent<Eatable>())
            {
                __result = ItemAction.None;
                return;
            }

            // Disables using medkits underwater
            if (tt == TechType.FirstAidKit && __result == ItemAction.Use)
            {
                if ((Main.Config.NoMedkitsUnderwater && Player.main.IsUnderwater()) ||
                    Player.main.GetComponent<LiveMixin>().maxHealth - Player.main.GetComponent<LiveMixin>().health < 0.01f)
                {
                    __result = ItemAction.None;
                }
            }
        }
    }

    public class TweakBladderfishTooltip
    {
        public static void ApplyBladderfishTooltip()
        {
            if (Language.main.GetCurrentLanguage() == "English" && Main.Config.BladderfishTooltip)
            {
                LanguageHandler.SetTechTypeTooltip(TechType.Bladderfish, Language.main.Get("Tooltip_Bladderfish") + " Provides some oxygen when consumed raw.");
            }
        }
    }
}