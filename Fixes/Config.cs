﻿using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Fixes
{
    [Menu("Fixes")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Scanner charge indicator", Tooltip = "Adds the missing charge level when using the Scanner.")]
        public bool ScannerChargeIndicator = true;

        [Toggle(Label = "Leviathans don't attack land targets", Tooltip = "Stops leviathans from trying to attack targets on land.")]
        public bool LeviathansDontAttackLandTargets = true;

        [Toggle(Label = "No Flashlight point lights", Tooltip = "Stops the flashlight from lighting the enviroment around it.")]
        public bool NoFlashlightPointLights = true;

        [Toggle(Label = "No plant waving indoors", Tooltip = "Removes the waving animation from indoor plants.")]
        public bool NoPlantWavingIndoors = true;

        [Toggle(Label = "Nuclear waste disposal name", Tooltip = "Corrected the Nuclear waste disposal bin's name and added the missing space to \"Trashcan\".")]
        public bool NuclearWasteDisposalName = true;

        [Toggle(Label = "Add missing Bulb Bush data entries", Tooltip = "Allows scanning the pygmy and large bulb bush to get the Bulb Bush data entry.")]
        public bool AddMissingBulbBushDataEntries = true;

        [Toggle(Label = "No prompt on cut doors", Tooltip = "Removes the laser cutting prompt from already cut doors.")]
        public bool NoPromptOnCutDoors = true;

        [Toggle(Label = "Beacon faces the player", Tooltip = "Makes the beacon face the player when deployed.")]
        public bool BeaconFacePlayer = true;

        [Toggle(Label = "No redundant climb prompt", Tooltip = "Removes the prompt to climb the Mobile Vehicle Bay while already standing on it.")]
        public bool NoRedundantMobileVehicleBayClimbing = true;

        [Toggle(Label = "Deadly Cyclops explosion", Tooltip = "Stops the player from respawning inside the Cyclops after its destruction.")]
        public bool DeadlyCyclopsExplosion = true;

        [Toggle(Label = "Smooth Cyclops wheel", Tooltip = "Makes the Cyclops' wheel movement smooth when using a controller.")]
        public bool SmoothCyclopsWheel = true;

        [Toggle(Label = "Silent Running no idle power drain", Tooltip = "Stops Silent Running from draining power when the engine is off.")]
        public bool SilentRunningNoIdleCost = true;

        [Toggle(Label = "Force anisotropic filtering", Tooltip = "Forces anisotropic filtering on every texture.")]
        public bool ForceAnisotropicFiltering = true;

        [Toggle(Label = "Seamoth storage modules gap", Tooltip = "Fixes the gap between the Seamoth and its lower storage modules.")]
        public bool SeamothStorageModulesGap = true;

        [Toggle(Label = "Docking bay sound checks", Tooltip = "Stops the Cyclops docking bay sounds from playing if it's already occupied.")]
        public bool DockingBaySoundChecks = true;

        [Toggle(Label = "Thermoblade dynamic particles", Tooltip = "Applies the correct particle effects from the Thermoblade depending on whether the player is underwater or not.")]
        public bool ThermobladeDynamicParticles = true;

        [Toggle(Label = "Cyclops power percentage clamp", Tooltip = "Fixes an underflow (very low negative number) when no cells are inserted.")]
        public bool CyclopsPowerPercentageClamp = true;

        [Toggle("<color=#f1c353>Persistence Fixes</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool PersistenceFixesDivider;

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
    }
}