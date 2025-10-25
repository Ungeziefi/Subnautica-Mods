using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Fixes
{
    [Menu("Fixes")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Scanner charge indicator", Tooltip = "Adds the missing charge level when using the Scanner.")]
        public bool ScannerChargeIndicator = true;

        [Toggle(Label = "Leviathans don't attack land targets", Tooltip = "Stops Leviathans from trying to attack targets on land.")]
        public bool LeviathansDontAttackLandTargets = true;

        [Toggle(Label = "No Flashlight point lights", Tooltip = "Stops the Flashlight from lighting the enviroment around it.")]
        public bool NoFlashlightPointLights = true;

        [Toggle(Label = "No plant waving indoors", Tooltip = "Removes the waving animation from indoor plants.")]
        public bool NoPlantWavingIndoors = true;

        [Toggle(Label = "Nuclear waste disposal name", Tooltip = "Corrected the Nuclear waste disposal bin's name and added the missing space to \"Trashcan\".")]
        public bool NuclearWasteDisposalName = true;

        [Toggle(Label = "Add missing Bulb Bush data entries", Tooltip = "Allows scanning any type of Bulb Bush to unlock its data entry.")]
        public bool AddMissingBulbBushDataEntries = true;

        [Toggle(Label = "No prompt on cut doors", Tooltip = "Removes the laser cutting prompt from already cut doors.")]
        public bool NoPromptOnCutDoors = true;

        [Toggle(Label = "Beacon faces the player", Tooltip = "Makes the beacon face the player when deployed.")]
        public bool BeaconFacePlayer = true;

        [Toggle(Label = "No MVB climb on top", Tooltip = "Removes the prompt to climb the Mobile Vehicle Bay while already standing on it.")]
        public bool NoMVBClimbOnTop = true;

        [Toggle(Label = "Deadly Cyclops explosion", Tooltip = "Stops the player from respawning inside the Cyclops after its destruction.")]
        public bool DeadlyCyclopsExplosion = true;

        [Toggle(Label = "Smooth Cyclops wheel", Tooltip = "Makes the Cyclops' wheel movement smooth when using a controller.")]
        public bool SmoothCyclopsWheel = true;

        [Toggle(Label = "Silent Running no idle power drain", Tooltip = "Stops Silent Running from draining power when the engine is off.")]
        public bool SilentRunningNoIdleCost = true;

        [Toggle(Label = "Seamoth storage modules gap", Tooltip = "Fixes the gap between the Seamoth and its lower storage modules.")]
        public bool SeamothStorageModulesGap = true;

        [Toggle(Label = "Docking bay sound checks", Tooltip = "Stops the Cyclops docking bay sounds from playing if it's already occupied.")]
        public bool DockingBaySoundChecks = true;

        [Toggle(Label = "Thermoblade dynamic particles", Tooltip = "Applies the correct particle effects from the Thermoblade depending on whether the player is underwater or not.")]
        public bool ThermobladeDynamicParticles = true;

        [Toggle(Label = "Cyclops power percentage clamp", Tooltip = "Fixes an underflow (very low negative number) when no cells are inserted.")]
        public bool CyclopsPowerPercentageClamp = true;

        [Toggle(Label = "Force engine shutdown", Tooltip = "Automatically shuts down the Cyclops engine when the power is off.")]
        public bool ForceEngineShutdown = true;

        [Toggle(Label = "Coffee drinking sound", Tooltip = "Changes the consume sound of coffee from eating to drinking.")]
        public bool CoffeeDrinkingSound = true;

        [Toggle(Label = "Drill sound with no target", Tooltip = "Prevents the drill sounds from stopping when nothing is being drilled.")]
        public bool DrillSoundWithNoTarget = true;

        [Toggle(Label = "Keep drill particles on load", Tooltip = "Fixes the missing drilling particles when drilling directly after loading and while the drill was already pointed at the deposit.")]
        public bool KeepDrillParticlesOnLoad = true;

        [Toggle(Label = "No used Data Box light")]
        public bool NoUsedDataBoxLight = true;

        [Toggle(Label = "No Seamoth drip particles")]
        public bool NoSeamothDripParticles = true;

        [Toggle(Label = "No used terminal prompt", Tooltip = "Data terminals don't show the download prompt after downloading.")]
        public bool NoUsedTerminalPrompt = true;

        [Toggle(Label = "Sulfur plant rotation", Tooltip = "Fixes the rotation of 2 Sulfur Plants (280 -40 -195 and 272 -41 -199).")]
        public bool SulfurPlantRotation = true;

        [Toggle(Label = "Reset Databank scroll", Tooltip = "Makes Databank entries always start at the top when opened instead of keeping the previous scroll position.")]
        public bool ResetDatabankScroll = true;

        [Toggle(Label = "Treaders can attack", Tooltip = "Fixes the Sea Treaders not being able to attack the player.")]
        public bool TreadersCanAttack = true;

        [Toggle(Label = "No Cyclops pushing", Tooltip = "Stops Reaper Leviathans from just pushing the Cyclops instead of attacking it.")]
        public bool NoCyclopsPushing = true;

        [Toggle(Label = "Matching Bulbo Tree LOD", Tooltip = "Disables the LOD system of land_plant_middle_01 to hide the mismatched LOD texture. Might affect performance very slightly.")]
        public bool MatchingBulboTreeLOD = true;

        [Toggle(Label = "Dynamic Creepvine light", Tooltip = "Updates the Creepvine seed light according to the amount of remaining seeds.")]
        public bool DynamicCreepvineLight = true;

        [Toggle(Label = "PRAWN collision sounds")]
        public bool PRAWNCollisionSounds = true;

        [Toggle(Label = "No voice while loading")]
        public bool NoVoiceWhileLoading = true;

        [Toggle(Label = "Keep dead raw fish stored", Tooltip = "Prevents stored raw fish from disappearing when loading a save.")]
        public bool KeepDeadRawFishStored = true;

        [Toggle(Label = "PRAWN particles only on sand", Tooltip = "Prevents PRAWN Suit landings from spawning particles if not on sand.")]
        public bool PRAWNParticlesOnlyOnSand = true;

        [Toggle(Label = "Add missing VFXSurfaces")]
        public bool AddMissingVFXSurfaces = true;

        [Choice(Label = "No deposit pop in", Tooltip = "Controls how far away resource deposits become visible.",
            Options = new[] { "Disabled", "Medium", "Far", "Very Far" })]
        public string NoDepositPopIn = "Far";

        [Choice(Label = "No Jellyshroom Cave pop in", Tooltip = "Controls how far away Jellyshroom Cave entities become visible.",
            Options = new[] { "Disabled", "Medium", "Far", "Very Far" })]
        public string NoJellyshroomCavePopIn = "Far";

        [Toggle(Label = "Delay Aurora reply", Tooltip = "Prevents the Aurora reply from being received before sending the distress signal.")]
        public bool DelayAuroraReply = true;

        [Toggle(Label = "No low speed splat", Tooltip = "Makes the Seamoth play the fish splat sound only when hitting creatures, using appropriate collision sounds for other objects.")]
        public bool NoLowSpeedSplat = true;

        [Toggle(Label = "Dead Peepers close eye LOD", Tooltip = "Fixes the Peepers' eyes in LOD distance from being open when dead.")]
        public bool DeadPeepersCloseEyeLOD = true;

        [Toggle(Label = "Oxygen Pipes across sub-biomes", Tooltip = "Allows Oxygen Pipes to be placed across sub-biomes such as caves or plateaus.")]
        public bool OxygenPipesAcrossSubBiomes = true;

        [Toggle(Label = "No Geyser safe spot", Tooltip = "Extends the Geysers' capsule collider vertically, removing the safe spot at the bottom.")]
        public bool NoGeyserSafeSpot = true;

        [Toggle(Label = "Soft collision sound minimum mass", Tooltip = "Prevents the soft collision sound from playing for objects with very low mass, such as Brain Coral bubbles.")]
        public bool SoftCollisionSoundMinimumMass = true;

        [Toggle(Label = "Disable ghost helm buttons", Tooltip = "Disables the invisible (but still clickable) Cyclops helm HUD buttons when the HUD is off.")]
        public bool DisableGhostHelmButtons = true;

        [Toggle("<color=#FFAC09FF>No fleeing to origin</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool NoFleeingToOriginDivider;

        [Toggle(Label = "Enable feature", Tooltip = "Stops creatures from returning to the origin coordinates (0, 0, 0) when fleeing.")]
        public bool NFTOEnableFeature = true;

        [Slider(Label = "Damage to distance ratio",
            Tooltip = "How far creatures flee per point of damage taken. Higher values make creatures flee farther when hit.",
            DefaultValue = 0.033f, Min = 0.01f, Max = 0.1f, Step = 0.001f, Format = "{0:0.000}")]
        public float DamageToDistanceRatio = 0.033f;

        [Slider(Label = "Maximum damage based distance",
            Tooltip = "The maximum extra distance creatures will flee regardless of damage taken. Prevents fleeing too far away.",
            DefaultValue = 50f, Min = 10f, Max = 100f, Step = 5f)]
        public float MaxDamageBasedDistance = 50f;

        [Toggle("<color=#FFAC09FF>Persistence</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool PersistenceDivider;

        [Toggle(Label = "Save open wreck doors")]
        public bool SaveOpenWreckDoors = true;

        [Toggle(Label = "Save closed Cyclops doors")]
        public bool SaveClosedCyclopsDoors = true;

        [Toggle(Label = "Save Cyclops speed mode")]
        public bool SaveCyclopsSpeedMode = true;

        [Toggle(Label = "Save Cyclops internal lights")]
        public bool SaveCyclopsInternalLights = true;

        [Toggle(Label = "Save Cyclops floodlights")]
        public bool SaveCyclopsFloodlights = true;

        //[Toggle(Label = "Save Seaglide toggles", Tooltip = "Saves the state of both the light and map.")]
        //public bool SaveSeaglideToggles = true;

        [Toggle(Label = "Save last held item")]
        public bool SaveLastHeldItem = true;
    }
}