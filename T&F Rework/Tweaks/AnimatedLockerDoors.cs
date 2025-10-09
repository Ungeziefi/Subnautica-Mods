using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class AnimatedLockerDoors
    {
        private static FMODAsset openSound;
        private static FMODAsset closeSound;

        private const string SmallLockerDoorPath = "model/submarine_locker_02/submarine_locker_02_door";
        private const string LargeLockerLeftDoorPath = "model/submarine_Storage_locker_big_01/submarine_Storage_locker_big_01_hinges_L";
        private const string LargeLockerRightDoorPath = "model/submarine_Storage_locker_big_01/submarine_Storage_locker_big_01_hinges_R";

        #region Animation Controller
        public class LockerDoorOpener : MonoBehaviour
        {
            public float startRotation;
            public float endRotation;
            public float t;
            public float duration;
            public float openAngle;
            public float doubleDoorOpenAngle;

            // Single door rotation
            public IEnumerator Rotate(Transform door, bool playCloseSound = false)
            {
                while (t < duration)
                {
                    t += Time.deltaTime;
                    float progress = Mathf.Clamp01(t / duration);
                    float rotation = Mathf.Lerp(startRotation, endRotation, progress);

                    door.localEulerAngles = new Vector3(door.localEulerAngles.x, door.localEulerAngles.y, rotation);

                    if (endRotation == 0f)
                    {
                        if (progress >= 0.6f && playCloseSound && closeSound != null)
                        {
                            playCloseSound = false;
                            Utils.PlayFMODAsset(closeSound, door.transform);
                        }
                        else if (progress >= 1f)
                        {
                            ReparentLabel(door);
                        }
                    }
                    yield return null;
                }
            }

            // Double door rotation
            public IEnumerator Rotate(Transform doorLeft, Transform doorRight, bool playCloseSound = false)
            {
                while (t < duration)
                {
                    t += Time.deltaTime;
                    float progress = Mathf.Clamp01(t / duration);
                    float rotation = Mathf.Lerp(startRotation, endRotation, progress);

                    doorLeft.localEulerAngles = new Vector3(doorLeft.localEulerAngles.x, doorLeft.localEulerAngles.y, -rotation);
                    doorRight.localEulerAngles = new Vector3(doorRight.localEulerAngles.x, doorRight.localEulerAngles.y, rotation);

                    if (progress >= 0.4f && playCloseSound && closeSound != null)
                    {
                        playCloseSound = false;
                        Utils.PlayFMODAsset(closeSound, doorLeft.transform.parent);
                    }
                    yield return null;
                }
            }

            private void ReparentLabel(Transform door)
            {
                var cl = door.GetComponentInChildren<ColoredLabel>();
                var parent = door.transform.parent.parent.parent;
                if (cl && parent)
                    cl.transform.SetParent(parent);
            }
        }
        #endregion

        #region Harmony Patches
        [HarmonyPatch(typeof(StorageContainer))]
        class StorageContainer_Patch
        {
            #region Helper Methods
            private static void OpenWallLocker(StorageContainer instance)
            {
                if (!Main.Config.AnimateSmallLockers) return;

                var door = instance.transform.Find(SmallLockerDoorPath);
                if (!door) return;

                var cl = instance.GetComponentInChildren<ColoredLabel>(true);
                if (cl) cl.transform.SetParent(door.transform);

                var rotater = instance.gameObject.EnsureComponent<LockerDoorOpener>();
                rotater.startRotation = door.transform.localEulerAngles.z;
                rotater.duration = Main.Config.LockerDoorAnimationDuration;
                rotater.openAngle = Main.Config.SingleDoorOpenAngle;
                rotater.endRotation = rotater.startRotation + rotater.openAngle;
                rotater.t = 0f;
                rotater.StartCoroutine(rotater.Rotate(door));

                if (openSound != null)
                    Utils.PlayFMODAsset(openSound, instance.transform);
            }

            private static void CloseWallLocker(StorageContainer instance)
            {
                if (!Main.Config.AnimateSmallLockers) return;

                var door = instance.transform.Find(SmallLockerDoorPath);
                if (!door) return;

                var rotater = instance.gameObject.EnsureComponent<LockerDoorOpener>();
                rotater.startRotation = door.transform.localEulerAngles.z;
                rotater.duration = Main.Config.LockerDoorAnimationDuration;
                rotater.endRotation = 0f;
                rotater.t = 0f;
                rotater.StartCoroutine(rotater.Rotate(door, true));
            }

            private static void OpenLargeLocker(StorageContainer instance)
            {
                if (!Main.Config.AnimateLargeLockers) return;

                var doorLeft = instance.transform.Find(LargeLockerLeftDoorPath);
                var doorRight = instance.transform.Find(LargeLockerRightDoorPath);
                if (!doorLeft || !doorRight) return;

                var rotater = instance.gameObject.EnsureComponent<LockerDoorOpener>();
                rotater.startRotation = doorLeft.transform.localEulerAngles.z;
                rotater.duration = Main.Config.LockerDoorAnimationDuration;
                rotater.doubleDoorOpenAngle = Main.Config.DoubleDoorOpenAngle;
                rotater.endRotation = rotater.startRotation + rotater.doubleDoorOpenAngle;
                rotater.t = 0f;
                rotater.StartCoroutine(rotater.Rotate(doorLeft, doorRight));

                if (openSound != null)
                    Utils.PlayFMODAsset(openSound, instance.transform);
            }

            private static void CloseLargeLocker(StorageContainer instance)
            {
                if (!Main.Config.AnimateLargeLockers) return;

                var doorLeft = instance.transform.Find(LargeLockerLeftDoorPath);
                var doorRight = instance.transform.Find(LargeLockerRightDoorPath);
                if (!doorLeft || !doorRight) return;

                var rotater = instance.gameObject.EnsureComponent<LockerDoorOpener>();
                rotater.startRotation = doorRight.transform.localEulerAngles.z;
                rotater.duration = Main.Config.LockerDoorAnimationDuration;
                rotater.endRotation = 0f;
                rotater.t = 0f;
                rotater.StartCoroutine(rotater.Rotate(doorLeft, doorRight, true));
            }
            #endregion

            #region Initialization
            [HarmonyPatch(nameof(StorageContainer.Awake)), HarmonyPostfix]
            static void StorageContainer_Awake(StorageContainer __instance)
            {
                if ((!Main.Config.AnimateSmallLockers && !Main.Config.AnimateLargeLockers) || openSound != null)
                    return;

                InitializeSounds();
            }

            private static void InitializeSounds()
            {
                openSound = ScriptableObject.CreateInstance<FMODAsset>();
                openSound.path = "event:/sub/cyclops/locker_open";
                openSound.id = "{c97d1fdf-ea26-4b19-8358-7f6ea77c3763}";

                closeSound = ScriptableObject.CreateInstance<FMODAsset>();
                closeSound.path = "event:/sub/cyclops/locker_close";
                closeSound.id = "{16eb5589-e341-41cb-9c88-02cb4e3da44a}";
            }
            #endregion

            #region Door Opening/Closing
            [HarmonyPatch(nameof(StorageContainer.Open), new Type[] { typeof(Transform) }), HarmonyPostfix]
            static void StorageContainer_Open(StorageContainer __instance, Transform useTransform)
            {
                HandleLockerAnimation(__instance, true);
            }

            [HarmonyPatch(nameof(StorageContainer.OnClose)), HarmonyPostfix]
            static void StorageContainer_OnClose(StorageContainer __instance)
            {
                HandleLockerAnimation(__instance, false);
            }

            private static void HandleLockerAnimation(StorageContainer instance, bool isOpening)
            {
                if (!Main.Config.AnimateSmallLockers && !Main.Config.AnimateLargeLockers)
                    return;

                var techTag = instance.GetComponent<TechTag>() ??
                              instance.transform.parent.GetComponent<TechTag>();
                if (techTag == null) return;

                switch (techTag.type)
                {
                    case TechType.SmallLocker:
                        if (isOpening) OpenWallLocker(instance);
                        else CloseWallLocker(instance);
                        break;

                    case TechType.Locker:
                        if (isOpening) OpenLargeLocker(instance);
                        else CloseLargeLocker(instance);
                        break;
                }
            }
            #endregion
        }
        #endregion
    }
}