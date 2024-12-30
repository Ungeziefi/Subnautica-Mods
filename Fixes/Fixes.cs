using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nautilus.Handlers;
using UnityEngine;

namespace Ungeziefi.Fixes
{
    // Fixed leviathans trying to attack targets on land  
    [HarmonyPatch(typeof(AggressiveWhenSeeTarget))]
    public class FixAggressiveWhenSeeTargetLeviathanLandAttack
    {
        [HarmonyPatch(nameof(AggressiveWhenSeeTarget.IsTargetValid))]
        [HarmonyPatch(new Type[] { typeof(GameObject) })]
        public static void Postfix(AggressiveWhenSeeTarget __instance, ref bool __result, GameObject target)
        {
            if (CreatureData.GetBehaviourType(__instance.myTechType) == BehaviourType.Leviathan && target.transform.position.y > Ocean.GetOceanLevel())
            {
                __result = false;
                // Main.Logger.LogInfo("Leviathan target is on land, attack aborted.");
            }
            else
            {
                __result = true;
                // Main.Logger.LogInfo("Leviathan target is valid.");
            }
        }
    }

    // Fixed missing charge level when equipping the scanner
    [HarmonyPatch(typeof(ScannerTool))]
    public class FixScannerToolChargeLevel
    {
        [HarmonyPatch(nameof(ScannerTool.Update))]
        public static bool Prefix(ScannerTool __instance)
        {
            if (__instance.isDrawn)
            {
                if (__instance.idleTimer > 0f)
                {
                    __instance.idleTimer = Mathf.Max(0f, __instance.idleTimer - Time.deltaTime);
                }
                var buttonFormat = LanguageCache.GetButtonFormat("ScannerSelfScanFormat", GameInput.Button.AltTool);
                HandReticle.main.SetTextRaw(HandReticle.TextType.Use, buttonFormat);
            }
            return false;
        }
    }

    // Removed point lights from the flashlight to prevent illuminating behind the player
    [HarmonyPatch(typeof(FlashLight))]
    public class FixFlashLightPointLights
    {
        [HarmonyPatch(nameof(FlashLight.Start))]
        public static void Prefix(FlashLight __instance)
        {
            var lights = __instance.GetComponentsInChildren<Light>(true);
            for (int i = lights.Length - 1; i >= 0; i--)
            {
                if (lights[i].type == LightType.Point)
                    lights[i].enabled = false;
            }
        }
    }

    // Removed waving animations from indoor plants
    [HarmonyPatch(typeof(LargeWorldEntity))]
    public class FixLargeWorldEntityWavingAnimationRemoval
    {
        private static readonly HashSet<TechType> techTypesToRemoveWavingShader = new HashSet<TechType>
            {
                TechType.BulboTree,
                TechType.PurpleVasePlant,
                TechType.OrangePetalsPlant,
                TechType.PinkMushroom,
                TechType.PurpleRattle,
                TechType.PinkFlower
            };

        private static void DisableWavingShader(Component component)
        {
            foreach (var material in component.GetComponentsInChildren<MeshRenderer>().SelectMany(mr => mr.materials))
            {
                material.DisableKeyword("UWE_WAVING");
            }
        }

        [HarmonyPatch(nameof(LargeWorldEntity.Awake))]
        public static void Prefix(LargeWorldEntity __instance)
        {
            var tt = CraftData.GetTechType(__instance.gameObject);
            if (techTypesToRemoveWavingShader.Contains(tt) && __instance.gameObject.GetComponentInParent<Base>(true))
            {
                DisableWavingShader(__instance);
            }
        }
    }

    // From "Use Trashcan" to "Use Trash can"
    // From "Use Trashcan" to "Use Nuclear waste disposal"
    // Capitalized "Use"
    [HarmonyPatch(typeof(Trashcan))]
    public class FixTrashcanNames
    {
        [HarmonyPatch(nameof(Trashcan.OnEnable))]
        public static void Prefix(Trashcan __instance)
        {
            if (Language.main.GetCurrentLanguage() == "English")
            {
                // Override localization strings at runtime using Nautilus
                LanguageHandler.SetLanguageLine("Trashcan", "Trash can");
                LanguageHandler.SetLanguageLine("Use", "Use");
                var usePrefix = Language.main.Get("Use") + " ";
                __instance.storageContainer.hoverText = usePrefix + (__instance.biohazard ? Language.main.Get("LabTrashcan") : Language.main.Get("Trashcan"));
            }
        }
    }

    // Allow scanning the pygmy and large bulb bush
    [HarmonyPatch(typeof(PDAScanner))]
    public class FixPDAScannerKoosh
    {
        [HarmonyPatch(nameof(PDAScanner.Initialize))]
        public static void Postfix()
        {
            if (PDAScanner.mapping.ContainsKey(TechType.MediumKoosh))
            {
                var entryData = PDAScanner.mapping[TechType.MediumKoosh];

                if (!PDAScanner.mapping.ContainsKey(TechType.LargeKoosh))
                    PDAScanner.mapping.Add(TechType.LargeKoosh, entryData);

                if (!PDAScanner.mapping.ContainsKey(TechType.SmallKoosh))
                    PDAScanner.mapping.Add(TechType.SmallKoosh, entryData);
            }
        }

        [HarmonyPatch(nameof(PDAScanner.Unlock))]
        public static void Postfix(PDAScanner.EntryData entryData)
        {
            if (entryData.key == TechType.MediumKoosh || entryData.key == TechType.SmallKoosh || entryData.key == TechType.LargeKoosh)
            {
                PDAScanner.complete.Add(TechType.LargeKoosh);
                PDAScanner.complete.Add(TechType.SmallKoosh);
                PDAScanner.complete.Add(TechType.MediumKoosh);
            }
        }
    }

    // Prevent interaction with cut-open doors
    [HarmonyPatch(typeof(StarshipDoor))]
    public class FixStarshipDoorInteraction
    {
        private static readonly HashSet<StarshipDoor> cutOpenedDoors = new HashSet<StarshipDoor>();

        [HarmonyPatch(nameof(StarshipDoor.OnHandHover))]
        public static bool Prefix(StarshipDoor __instance)
        {
            if (cutOpenedDoors.Contains(__instance))
            {
                return false;
            }

            var laserCutObject = __instance.GetComponent<LaserCutObject>();
            if (laserCutObject != null && laserCutObject.isCutOpen)
            {
                cutOpenedDoors.Add(__instance);
                return false;
            }

            return true;
        }
    }

    // Save the state of bulkhead doors
    [HarmonyPatch(typeof(BulkheadDoor))]
    public class FixBulkheadDoorState
    {
        private static readonly Dictionary<string, HashSet<Vector3Int>> openedWreckDoors = new Dictionary<string, HashSet<Vector3Int>>();

        [HarmonyPatch(nameof(BulkheadDoor.OnHandClick))]
        public static void Postfix(BulkheadDoor __instance)
        {
            var pos = new Vector3Int((int)__instance.transform.position.x, (int)__instance.transform.position.y, (int)__instance.transform.position.z);
            var slot = SaveLoadManager.main.currentSlot;
            if (!openedWreckDoors.ContainsKey(slot))
            {
                openedWreckDoors[slot] = new HashSet<Vector3Int>();
            }    

            if (__instance.opened)
            {
                openedWreckDoors[slot].Remove(pos);
                // Main.Logger.LogInfo($"Door at {pos} in slot {slot} closed and state saved.");
            }

            else
            {
                openedWreckDoors[slot].Add(pos);
                // Main.Logger.LogInfo($"Door at {pos} in slot {slot} opened and state saved.");
            }
        }

        [HarmonyPatch(nameof(BulkheadDoor.Awake))]
        public static void Prefix(BulkheadDoor __instance)
        {
            var pos = new Vector3Int((int)__instance.transform.position.x, (int)__instance.transform.position.y, (int)__instance.transform.position.z);
            var slot = SaveLoadManager.main.currentSlot;
            if (openedWreckDoors.ContainsKey(slot) && openedWreckDoors[slot].Contains(pos))
            {
                __instance.initiallyOpen = true;
            }
        }
    }

    // Fix beacon rotation when thrown
    [HarmonyPatch(typeof(Beacon))]
    public class FixBeaconRotation
    {
        [HarmonyPatch(nameof(Beacon.Throw))]
        public static void Postfix(Beacon __instance)
        {
            var cameraRotation = Camera.main.transform.rotation;
            __instance.transform.rotation = cameraRotation;
            __instance.transform.Rotate(0f, 180f, 0f);
        }
    }

    // Fix being able to climb the constructor when standing on it
    [HarmonyPatch(typeof(Constructor))]
    public class FixConstructorClimbing
    {
        [HarmonyPatch(nameof(Constructor.Update))]
        public static void Postfix(Constructor __instance)
        {
            if (Player.main.transform.position.y > Ocean.GetOceanLevel())
            {
                __instance.climbTrigger.SetActive(false);
            }
        }
    }

    // Prevent the player from respawning inside the Cyclops when it's destroyed
    [HarmonyPatch(typeof(CyclopsDestructionEvent))]
    public class FixCyclopsDestructionEventDeath
    {
        [HarmonyPatch(nameof(CyclopsDestructionEvent.DestroyCyclops))]
        public static void Prefix(CyclopsDestructionEvent __instance)
        {
            __instance.subLiveMixin.Kill();
        }
    }

    // Fix for the Cyclops' steering wheel only having 100% left or right when using a controller
    [HarmonyPatch(typeof(SubControl))]
    public class FixSubControlSteering
    {
        [HarmonyPatch(nameof(SubControl.UpdateAnimation))]
        public static bool Prefix(SubControl __instance)
        {
            float steeringWheelYaw = 0f;
            float steeringWheelPitch = 0f;

            // Get the throttle values for yaw and pitch
            float throttleX = __instance.throttle.x;
            float throttleY = __instance.throttle.y;

            // Handle yaw (left/right steering)
            if (Mathf.Abs(throttleX) > 0.0001)
            {
                ShipSide useShipSide;
                if (throttleX > 0)
                {
                    useShipSide = ShipSide.Port;
                    steeringWheelYaw = throttleX;
                }
                else
                {
                    useShipSide = ShipSide.Starboard;
                    steeringWheelYaw = throttleX;
                }

                // Trigger turn handlers if the throttle is significant
                if (throttleX < -0.1 || throttleX > 0.1)
                {
                    for (int index = 0; index < __instance.turnHandlers.Length; ++index)
                        __instance.turnHandlers[index].OnSubTurn(useShipSide);
                }
            }

            // Handle pitch (up/down steering)
            if (Mathf.Abs(throttleY) > 0.0001)
            {
                steeringWheelPitch = throttleY;
            }

            // Smoothly interpolate the steering wheel's yaw and pitch
            __instance.steeringWheelYaw = Mathf.Lerp(__instance.steeringWheelYaw, steeringWheelYaw, Time.deltaTime * __instance.steeringReponsiveness);
            __instance.steeringWheelPitch = Mathf.Lerp(__instance.steeringWheelPitch, steeringWheelPitch, Time.deltaTime * __instance.steeringReponsiveness);

            // Update the animator with the new yaw and pitch values
            if (__instance.mainAnimator)
            {
                __instance.mainAnimator.SetFloat("view_yaw", __instance.steeringWheelYaw * 100f);
                __instance.mainAnimator.SetFloat("view_pitch", __instance.steeringWheelPitch * 100f);
            }

            // Prevent the original method from running
            return false;
        }
    }

    // Print the current power drain of the Cyclops
    //[HarmonyPatch(typeof(PowerRelay))]
    //public class DebugCyclopsPowerDrain
    //{
    //    [HarmonyPatch(nameof(PowerRelay.ModifyPower))]
    //    public static void Postfix(PowerRelay __instance, float amount, float modified)
    //    {
    //        if (__instance.GetComponent<SubRoot>() is SubRoot sub && sub.isCyclops)
    //        {
    //            Main.Logger.LogInfo($"Cyclops power modification: {amount}, modified: {modified}");
    //        }
    //    }
    //}

    // Prevent power drain when using the silent mode of the submarine
    [HarmonyPatch(typeof(CyclopsSilentRunningAbilityButton))]
    public class FixCyclopsSilentRunningAbilityButtonPowerDrain
    {
        [HarmonyPatch(nameof(CyclopsSilentRunningAbilityButton.SilentRunningIteration))]
        public static bool Prefix(CyclopsSilentRunningAbilityButton __instance)
        {
            // Don't consume power when the engine is off
            if (Player.main.currentSub != null && Player.main.currentSub.noiseManager != null && Player.main.currentSub.noiseManager.noiseScalar == 0f)
            {
                return false;
            }

            // Consume energy for silent running mode
            if (__instance.subRoot.powerRelay.ConsumeEnergy(__instance.subRoot.silentRunningPowerCost, out float amountConsumed))
            {
                // Power consumption successful, continue silent running
                return false;
            }

            // Turn off silent running if power consumption fails
            __instance.TurnOffSilentRunning();
            return false;
        }
    }
}