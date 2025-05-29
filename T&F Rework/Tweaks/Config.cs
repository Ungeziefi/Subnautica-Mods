using Nautilus.Json;
using Nautilus.Options.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    public enum NoBundledBatteriesOption
    {
        Disabled,
        VanillaRecipes,
        AllRecipes
    }

    [Menu("Tweaks")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Seamoth pushing", Tooltip = "Allows pushing the Seamoth when it's on land.")]
        public bool SeamothPushing = false;

        [Toggle(Label = "No eating underwater")]
        public bool NoEatingUnderwater = false;

        [Toggle(Label = "No medkits underwater")]
        public bool NoMedkitsUnderwater = false;

        [Toggle(Label = "Chair swivelling")]
        public bool ChairSwivelling = false;

        [Toggle(Label = "No obstacle check when sitting", Tooltip = "Allows sitting even if there's an object in the way.")]
        public bool NoObstacleCheckWhenSitting = false;

        [Toggle(Label = "Outcrops require tool", Tooltip = "Harvesting outcrops requires any tool to be equipped.")]
        public bool OutcropsRequireTool = false;

        [Toggle(Label = "Power cell charge from batteries", Tooltip = "Sets the charge level of newly crafted power cells based on the charge level of the batteries used for crafting.")]
        public bool PowerCellChargeFromBatteries = false;

        [Toggle(Label = "Skip epilepsy warning")]
        public bool SkipEpilepsyWarning = false;

        [Toggle(Label = "Plant rotation randomizer")]
        public bool PlantRotationRandomizer = false;

        [Choice(Label = "No bundled batteries",
            Tooltip = "Tools and vehicles don't include batteries.\nVanilla recipes: Cyclops, PRAWN Suit, and Repair Tool.",
            Options = new[] { "Disabled", "Vanilla recipes", "All recipes" })]
        public NoBundledBatteriesOption NoBundledBatteriesOption = NoBundledBatteriesOption.Disabled;

        [Toggle(Label = "No floodlights on build", Tooltip = "Stops the floodlights from being defaulted to on when a Cyclops is built.")]
        public bool NoFloodlightsOnBuild = false;

        [Toggle(Label = "Mobile Vehicle Bay beacon")]
        public bool MobileVehicleBayBeacon = false;

        [Toggle(Label = "Escape closes PDA")]
        public bool EscapeClosesPDA = false;

        [Toggle(Label = "Cyclops displays needs power", Tooltip = "Disables various screens and systems when the power is off.")]
        public bool CyclopsDisplaysNeedPower = false;

        [Toggle(Label = "Batteries have tooltips")]
        public bool BatteriesHaveTooltips = false;

        [Toggle(Label = "No PDA delay")]
        public bool NoPDADelay = false;

        [Toggle(Label = "Disable email box", Tooltip = "Disables the email box in the main menu when news are disabled.")]
        public bool DisableEmailBox = false;

        [Toggle(Label = "Destructible Drooping Stingers")]
        public bool DestructibleDroopingStingers = false;

        [Toggle(Label = "Land_tree_01 light removal")]
        public bool Land_tree_01LightRemoval = false;

        [Slider(Label = "Scanner Room speed multiplier", Tooltip = "Lower values mean faster scanning.",
            DefaultValue = 1f, Min = 0.1f, Max = 3f, Step = 0.1f, Format = "{0:0.0}x")]
        public float ScannerRoomSpeedMultiplier = 1f;

        [Toggle("<color=#f1c353>Inventory transfer</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool InventoryTransferDivider;

        [Toggle(Label = "Enable transfer all items", Tooltip = "Hold a key to transfer all items between containers at once.")]
        public bool EnableTransferAllItems = false;

        [Toggle(Label = "Enable transfer similar items", Tooltip = "Hold a key to transfer all items of the same type between containers at once.")]
        public bool EnableTransferSimilarItems = false;

        [Keybind(Label = "Transfer all items key", Tooltip = "Hold this key while clicking an item to transfer all items.")]
        public KeyCode TransferAllItemsKey = KeyCode.LeftShift;

        [Keybind(Label = "Transfer similar items key", Tooltip = "Hold this key while clicking an item to transfer all items of the same type.")]
        public KeyCode TransferSimilarItemsKey = KeyCode.LeftControl;

        [Toggle("<color=#f1c353>Seaglide light angle</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool SeaglideAngleLightDivider;

        [Toggle(Label = "Enable feature")]
        public bool SLAEnableFeature = false;

        [Slider(Label = "Light angle", Tooltip = "The angle in degrees to tilt the Seaglide light beam upward (higher value) or downward (lower value).",
            DefaultValue = 30f, Min = -10f, Max = 60f, Step = 1f, Format = "{0:0}°")]
        public float LightAngle = 30f;

        [Toggle("<color=#f1c353>New commands</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool NewCommandsDivider;

        [Toggle(Label = "RestoreHealth")]
        public bool RestoreHealth = false;

        [Toggle(Label = "RestoreHunger")]
        public bool RestoreHunger = false;

        [Toggle(Label = "RestoreThirst")]
        public bool RestoreThirst = false;

        [Toggle(Label = "RestoreAll")]
        public bool RestoreAll = false;

        [Toggle(Label = "QQQ", Tooltip= "Quits the game without confirmation.")]
        public bool QQQ = false;

        [Toggle("<color=#f1c353>Creature size randomizer</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool CreatureSizeRandomizerDivider;

        [Toggle(Label = "Enable feature")]
        public bool CSREnableFeature = false;

        public List<string> CSRTargetCreatures = new List<string>
        {
            "CaveCrawler",
            "LavaLarva",
            "Bleeder",
            "Rockgrub",
            "Blighter",
            "Floater"
        };

        [Slider(Label = "Minimum multiplier", DefaultValue = 0.75f, Min = 0.01f, Max = 1f, Step = 0.01f, Format = "{0:0.00}x")]
        public float MinCreatureSize = 0.75f;

        [Slider(Label = "Maximum multiplier", DefaultValue = 1.25f, Min = 1f, Max = 2f, Step = 0.01f, Format = "{0:0.00}x")]
        public float MaxCreatureSize = 1.25f;

        [Toggle("<color=#f1c353>Miscellaneous</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool MiscellaneousTweaksDivider;

        [Toggle(Label = "Bladderfish tooltip", Tooltip = "Adds a tooltip about the Bladderfish providing oxygen if consumed raw.")]
        public bool BladderfishTooltip = false;

        [Toggle(Label = "\"Use Trashcan\" -> \"Use Trash can\"", Tooltip = "Adds a missing space to the Trashcan prompt for consistency with its recipe name.")]
        public bool TrashcanNameConsistency = false;

        [Toggle(Label = "Capitalize \"Use\"", Tooltip = "Capitalizes the \"Use\" prompt for consistency with other prompts.")]
        public bool CapitalizeUseString = false;

        [Toggle("<color=#f1c353>Multipliers</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool MultipliersDivider;

        [Slider(Label = "Build time multiplier", Tooltip = "Higher values make building take longer. Lower values make building faster.",
            DefaultValue = 1f, Min = 0.01f, Max = 10f, Step = 0.01f, Format = "{0:0.00}x")]
        public float BuildTimeMultiplier = 1f;

        [Slider(Label = "Craft time multiplier", Tooltip = "Higher values make crafting take longer. Lower values make crafting faster.",
            DefaultValue = 1f, Min = 0.01f, Max = 10f, Step = 0.01f, Format = "{0:0.00}x")]
        public float CraftTimeMultiplier = 1f;

        [Slider(Label = "Plant growth time multiplier", Tooltip = "Higher values make plants grow slower. Lower values make plants grow faster.",
            DefaultValue = 1f, Min = 0.01f, Max = 10f, Step = 0.01f, Format = "{0:0.00}x")]
        public float PlantGrowthTimeMultiplier = 1f;

        [Slider(Label = "Day/night cycle speed multiplier", Tooltip = "Higher values make days and nights shorter. Lower values make days and nights longer.",
            DefaultValue = 1f, Min = 0.01f, Max = 10f, Step = 0.01f, Format = "{0:0.00}x")]
        public float DayNightCycleSpeedMultiplier = 1f;
    }
}