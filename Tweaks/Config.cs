using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Tweaks
{
    [Menu("Tweaks")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Seamoth pushing", Tooltip = "Allows pushing the Seamoth when it's on land.")]
        public bool SeamothPushing = true;

        [Toggle(Label = "No eating underwater")]
        public bool NoEatingUnderwater = true;

        [Toggle(Label = "No medkits underwater")]
        public bool NoMedkitsUnderwater = true;

        [Toggle(Label = "Chair swivelling")]
        public bool ChairSwivelling = true;

        [Toggle(Label = "No obstacle check when sitting", Tooltip = "Allows sitting even if there's an object in the way.")]
        public bool NoObstacleCheckWhenSitting = true;

        [Toggle(Label = "Harvesting requirements", Tooltip = "Harvesting outcrops and flora requires tools.")]
        public bool HarvestingRequirements = true;

        [Toggle(Label = "Power cell charge from batteries", Tooltip = "Sets the charge level of newly crafted power cells based on the charge level of the batteries used for crafting.")]
        public bool PowerCellChargeFromBatteries = true;

        [Toggle(Label = "Skip epilepsy warning")]
        public bool SkipEpilepsyWarning = true;

        [Toggle(Label = "Creature size randomizer", Tooltip = "Randomizes the size of small critters based on the multipliers below.")]
        public bool CreatureSizeRandomizer = true;

        [Slider(Label = "Minimum creature size", Tooltip = "Divide by 10 for the actual value.", DefaultValue = 5, Min = 1, Max = 10, Step = 1)]
        public int MinCreatureSize = 5;

        [Slider(Label = "Maximum creature size", Tooltip = "Divide by 10 for the actual value.", DefaultValue = 15, Min = 10, Max = 20, Step = 1)]
        public int MaxCreatureSize = 15;

        [Toggle(Label = "Plant rotation randomizer")]
        public bool PlantRotationRandomizer = true;

        [Toggle("<color=#f1c353>Minor Fixes</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool Minor_Fixes_Divider;

        [Toggle(Label = "Bladderfish tooltip", Tooltip = "Adds a tooltip about the Bladderfish providing oxygen if consumed raw.")]
        public bool BladderfishTooltip = true;

        [Toggle(Label = "\"Use Trashcan\" -> \"Use Trash can\"", Tooltip = "Adds a missing space to the Trashcan prompt for consistency with its recipe name.")]
        public bool TrashcanNameConsistency = true;

        [Toggle(Label = "Capitalize \"Use\"", Tooltip = "Capitalizes the \"Use\" string globally.")]
        public bool CapitalizeUseString = true;

        [Toggle("<color=#f1c353>Multipliers</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool Multipliers_Divider;

        [Slider(Label = "Build time multiplier", DefaultValue = 1, Min = 1, Max = 10, Step = 1)]
        public float BuildTimeMultiplier = 1f;

        [Slider(Label = "Craft time multiplier", DefaultValue = 1, Min = 1, Max = 10, Step = 1)]
        public float CraftTimeMultiplier = 1f;

        [Slider(Label = "Plant growth time multiplier", DefaultValue = 1, Min = 1, Max = 10, Step = 1)]
        public float PlantGrowthTimeMultiplier = 1f;
    }
}