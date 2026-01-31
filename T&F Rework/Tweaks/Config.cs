using Nautilus.Json;
using Nautilus.Options.Attributes;
using System.Collections.Generic;

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

        [Toggle(Label = "Outcrops require tool", Tooltip = "Harvesting outcrops requires any tool to be equipped.")]
        public bool OutcropsRequireTool = false;

        [Toggle(Label = "Power cell charge from batteries", Tooltip = "Sets the charge level of newly crafted power cells based on the charge level of the batteries used for crafting.")]
        public bool PowerCellChargeFromBatteries = false;

        [Toggle(Label = "Skip epilepsy warning")]
        public bool SkipEpilepsyWarning = false;

        [Toggle(Label = "Plant rotation randomizer")]
        public bool PlantRotationRandomizer = false;

        [Toggle(Label = "Mobile Vehicle Bay beacon")]
        public bool MobileVehicleBayBeacon = false;

        [Toggle(Label = "Cyclops displays needs power", Tooltip = "Disables various screens and systems when the power is off.")]
        public bool CyclopsDisplaysNeedPower = false;

        [Toggle(Label = "Batteries have tooltips")]
        public bool BatteriesHaveTooltips = false;

        [Toggle(Label = "No PDA delay")]
        public bool NoPDADelay = false;

        [Toggle(Label = "Destructible Drooping Stingers")]
        public bool DestructibleDroopingStingers = false;

        [Toggle(Label = "Land_tree_01 light removal")]
        public bool Land_tree_01LightRemoval = false;

        [Toggle(Label = "No Reefback surfacing")]
        public bool NoReefbackSurfacing = false;

        [Toggle(Label = "Moveable Mobile Vehicle Bay", Tooltip = "Allows the repulsion and propulsion cannons to move the Mobile Vehicle Bay.")]
        public bool MoveableMobileVehicleBay = false;

        [Toggle(Label = "Creepvine unlocks Fiber Mesh", Tooltip = "Scanning Creepvine unlocks the Fiber Mesh blueprint.")]
        public bool CreepvineUnlocksFiberMesh = false;

        [Toggle(Label = "Openable chests in PRAWN", Tooltip = "The PRAWN Suit can now open supply chests.")]
        public bool OpenableChestsInPRAWN = false;

        [Toggle(Label = "No obstacle check when sitting", Tooltip = "Allows sitting even if there's an object in the way.")]
        public bool NoObstacleCheckWhenSitting = false;

        [Toggle(Label = "No obstacle check when sleeping", Tooltip = "Allows sleeping even if there's an object in the way.")]
        public bool NoObstacleCheckWhenSleeping = false;

        [Choice(Label = "No bundled batteries",
            Tooltip = "Tools and vehicles don't include batteries.\nVanilla recipes: Cyclops, PRAWN Suit, and Repair Tool.",
            Options = new[] { "Disabled", "Vanilla recipes", "All recipes" })]
        public NoBundledBatteriesOption NoBundledBatteriesOption = NoBundledBatteriesOption.Disabled;

        [Toggle(Label = "No floodlights on build", Tooltip = "Stops the floodlights from being defaulted to on when a Cyclops is built.")]
        public bool NoFloodlightsOnBuild = false;

        [Toggle(Label = "Escape closes PDA")]
        public bool EscapeClosesPDA = false;

        [Slider(Label = "Seaglide light angle", Tooltip = "The angle in degrees to tilt the Seaglide light beam upward (higher value) or downward (lower value).",
            DefaultValue = 0f, Min = -10f, Max = 60f, Step = 1f, Format = "{0:0}°")]
        public float SeaglideLightAngle = 0f;

        [Slider(Label = "PRAWN Suit claw damage", DefaultValue = 50f, Min = 10f, Max = 1000f, Step = 1f)]
        public float PRAWNSuitClawDamage = 50f;

        [Toggle(Label = "PRAWN Suit lights follow camera")]
        public bool PRAWNSuitLightsFollowCamera = false;

        [Toggle(Label = "PRAWN Suit arms need power")]
        public bool PRAWNSuitArmsNeedPower = false;

        [Toggle(Label = "Passive engine overheating", Tooltip = "The Cyclops engine can now overheat even when the throttle is not applied.")]
        public bool PassiveEngineOverheating = false;

        [Toggle(Label = "Smoke clears on open", Tooltip = "Opening the top hatch of the life pod clears the smoke inside.")]
        public bool SmokeClearsOnOpen = false;

        [Toggle(Label = "Upgrade only when docked", Tooltip = "Prevents opening the Seamoth's or PRAWN Suit's upgrade panel unless docked.")]
        public bool UpgradeOnlyWhenDocked = false;

        [Toggle("<color=#FFAC09FF>Torpedo cycling</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool TorpedoCyclingDivider;

        [Toggle(Label = "Enable feature")]
        public bool TCEnableFeature = false;

        [Toggle("<color=#FFAC09FF>Animated locker doors</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool AnimatedLockerDoorsDivider;

        [Toggle(Label = "Animate small lockers")]
        public bool AnimateSmallLockers = false;

        [Toggle(Label = "Animate large lockers")]
        public bool AnimateLargeLockers = false;

        [Slider(Label = "Locker door animation duration", DefaultValue = 0.5f, Min = 0.1f, Max = 1f, Step = 0.1f, Format = "{0:0.0}s")]
        public float LockerDoorAnimationDuration = 0.5f;

        [Slider(Label = "Single door opening angle", DefaultValue = 135f, Min = 15f, Max = 180f, Step = 5f, Format = "{0:0}°")]
        public float SingleDoorOpenAngle = 135f;

        [Slider(Label = "Double door opening angle", DefaultValue = 90f, Min = 15f, Max = 135f, Step = 5f, Format = "{0:0}°")]
        public float DoubleDoorOpenAngle = 90f;

        [Toggle("<color=#FFAC09FF>Miscellaneous</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool MiscellaneousTweaksDivider;

        [Toggle(Label = "Bladderfish tooltip", Tooltip = "Adds a tooltip about the Bladderfish providing oxygen if consumed raw.")]
        public bool BladderfishTooltip = false;

        [Toggle(Label = "\"Use Trashcan\" -> \"Use Trash can\"", Tooltip = "Adds a missing space to the Trashcan prompt for consistency with its recipe name.")]
        public bool TrashcanNameConsistency = false;

        [Toggle(Label = "Capitalize \"Use\"", Tooltip = "Capitalizes the \"Use\" prompt for consistency with other prompts.")]
        public bool CapitalizeUseString = false;

        [Toggle("<color=#FFAC09FF>Multipliers</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
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

        [Slider(Label = "Scanner Room speed multiplier", Tooltip = "Lower values mean faster scanning.",
            DefaultValue = 1f, Min = 0.01f, Max = 10f, Step = 0.01f, Format = "{0:0.00}x")]
        public float ScannerRoomSpeedMultiplier = 1f;

        [Toggle("<color=#FFAC09FF>Creature size randomizer</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool CreatureSizeRandomizerDivider;

        [Toggle(Label = "Enable feature")]
        public bool CSREnableFeature = false;

        public List<string> CSRTargetCreatures = new()
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

        [Toggle("<color=#FFAC09FF>New commands</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool NewCommandsDivider;

        [Toggle(Label = "RestoreHealth")]
        public bool RestoreHealth = false;

        [Toggle(Label = "RestoreHunger")]
        public bool RestoreHunger = false;

        [Toggle(Label = "RestoreThirst")]
        public bool RestoreThirst = false;

        [Toggle(Label = "RestoreAll")]
        public bool RestoreAll = false;

        [Toggle(Label = "QQQ", Tooltip = "Quits the game without confirmation.")]
        public bool QQQ = false;

        [Toggle("<color=#FFAC09FF>Toggle PRAWN Suit lights</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool TogglePRAWNSuitLightsDivider;

        [Toggle(Label = "Enable feature")]
        public bool TPSLEnableFeature = false;

        [Toggle("<color=#FFAC09FF>Base lights</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool BaseLightsDivider;

        [Toggle(Label = "Toggle base lights", Tooltip = "Allows toggling the lights in the current base. Keybind configurable in the \"Mod Input\" menu.")]
        public bool ToggleBaseLights = false;

        [Slider(Label = "Toggle hold duration", Tooltip = "The duration (in seconds) you need to hold the key to toggle base lights.", DefaultValue = 2, Min = 1, Max = 4, Step = 1, Format = "{0}s")]
        public int ToggleHoldDuration = 2;

        [Toggle(Label = "Toggle lights for sleep", Tooltip = "Automatically toggles the lights when going to bed and waking up.")]
        public bool ToggleLightsForSleep = false;

        [Slider(Label = "Lights on after sleep delay", Tooltip = "Delay (in seconds) before base lights turn back on after sleeping.", DefaultValue = 10, Min = 1, Max = 10, Step = 1, Format = "{0}s")]
        public int LightsOnAfterSleepDelay = 10;

        [Toggle("<color=#FFAC09FF>Geysers push objects</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool GeysersPushObjectsDivider;

        [Toggle(Label = "Geysers push objects")]
        public bool GPOEnableFeature = false;

        [Slider(Label = "Eruption force", Tooltip = "The force applied to objects during geyser eruptions. Higher values push objects farther.",
            DefaultValue = 50f, Min = 0f, Max = 200f, Step = 5f)]
        public float GeyserEruptionForce = 50f;
    }
}