using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Tweaks
{
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

        [Toggle(Label = "Harvesting requirements", Tooltip = "Harvesting outcrops and flora requires tools.")]
        public bool HarvestingRequirements = false;

        [Toggle(Label = "Power cell charge from batteries", Tooltip = "Sets the charge level of newly crafted power cells based on the charge level of the batteries used for crafting.")]
        public bool PowerCellChargeFromBatteries = false;

        [Toggle(Label = "Skip epilepsy warning")]
        public bool SkipEpilepsyWarning = false;

        [Toggle(Label = "Plant rotation randomizer")]
        public bool PlantRotationRandomizer = false;

        public enum NoBundledBatteriesOption
        {
            Disabled,
            VanillaRecipes,
            AllRecipes
        }

        [Choice(Label = "No bundled batteries", Tooltip = "Tools and vehicles don't include batteries.", Options = new[] { "Disabled", "Vanilla recipes", "All recipes" })]
        public NoBundledBatteriesOption NoBundledBatteries = NoBundledBatteriesOption.Disabled;

        [Toggle("<color=#f1c353>Creature size randomizer</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool CreatureSizeRandomizerDivider;

        [Toggle(Label = "Creature size randomizer", Tooltip = "Randomizes the size of Cave Crawlers, Lava Larvas, Bleeders, Rockgrubs, Blighters, and Floaters. Configurable through the sliders below.")]
        public bool CreatureSizeRandomizer = false;

        [Slider(Label = "Minimum multiplier", DefaultValue = 0.5f, Min = 0.1f, Max = 1f, Step = 0.1f, Format = "{0:0.0}")]
        public float MinCreatureSize = 0.5f;

        [Slider(Label = "Maximum multiplier", DefaultValue = 1.5f, Min = 1f, Max = 2f, Step = 0.1f, Format = "{0:0.0}")]
        public float MaxCreatureSize = 1.5f;

        [Toggle("<color=#f1c353>Miscellaneous</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool miscellaneousTweakerDivider;

        [Toggle(Label = "Bladderfish tooltip", Tooltip = "Adds a tooltip about the Bladderfish providing oxygen if consumed raw.")]
        public bool BladderfishTooltip = false;

        [Toggle(Label = "\"Use Trashcan\" -> \"Use Trash can\"", Tooltip = "Adds a missing space to the Trashcan prompt for consistency with its recipe name.")]
        public bool TrashcanNameConsistency = false;

        [Toggle(Label = "Capitalize \"Use\"", Tooltip = "Capitalizes the \"Use\" prompt for consistency with other prompts.")]
        public bool CapitalizeUseString = false;

        [Toggle("<color=#f1c353>Multipliers</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool MultipliersDivider;

        [Slider(Label = "Build time multiplier", DefaultValue = 1, Min = 1, Max = 10, Step = 1)]
        public float BuildTimeMultiplier = 1f;

        [Slider(Label = "Craft time multiplier", DefaultValue = 1, Min = 1, Max = 10, Step = 1)]
        public float CraftTimeMultiplier = 1f;

        [Slider(Label = "Plant growth time multiplier", DefaultValue = 1, Min = 1, Max = 10, Step = 1)]
        public float PlantGrowthTimeMultiplier = 1f;
    }
}