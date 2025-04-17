using Nautilus.Json;
using Nautilus.Options.Attributes;

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

        [Toggle(Label = "Auto-close Bulkheads", Tooltip = "Automatically closes Bulkheads when a leak starts in a base.")]
        public bool AutoCloseBulkheads = false;

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

        [Toggle("<color=#f1c353>Critter size randomizer</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool CritterSizeRandomizerDivider;

        [Toggle(Label = "Enable feature", Tooltip = "Randomizes the size of Cave Crawlers, Lava Larvas, Bleeders, Rockgrubs, Blighters, and Floaters.")]
        public bool CSREnableFeature = false;

        [Slider(Label = "Minimum multiplier", DefaultValue = 0.75f, Min = 0.01f, Max = 1f, Step = 0.01f, Format = "{0:0.00}")]
        public float MinCreatureSize = 0.75f;

        [Slider(Label = "Maximum multiplier", DefaultValue = 1.25f, Min = 1f, Max = 2f, Step = 0.01f, Format = "{0:0.00}")]
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

        [Slider(Label = "Build time", DefaultValue = 1f, Min = 0.01f, Max = 10f, Step = 0.01f, Format = "{0:0.00}")]
        public float BuildTimeMultiplier = 1f;

        [Slider(Label = "Craft time ", DefaultValue = 1f, Min = 0.01f, Max = 10f, Step = 0.01f, Format = "{0:0.00}")]
        public float CraftTimeMultiplier = 1f;

        [Slider(Label = "Plant growth time", DefaultValue = 1f, Min = 0.01f, Max = 10f, Step = 0.01f, Format = "{0:0.00}")]
        public float PlantGrowthTimeMultiplier = 1f;

        [Slider(Label = "Day/night cycle speed", Tooltip = "Lower values make days longer.",
            DefaultValue = 1f, Min = 0.01f, Max = 10f, Step = 0.01f, Format = "{0:0.00}")]
        public float DayNightCycleSpeed = 1f;
    }
}