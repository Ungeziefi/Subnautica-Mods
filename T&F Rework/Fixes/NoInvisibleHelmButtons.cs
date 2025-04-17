// To-Do: Fix the speed selector not matching engine state when doing helmHUDVisuals.SetActive(true)

//using System.Collections;
//using HarmonyLib;
//using UnityEngine;

//namespace Ungeziefi.Fixes
//{
//    [HarmonyPatch]
//    public class NoInvisibleHelmButtons
//    {
//        [HarmonyPatch(typeof(CyclopsHelmHUDManager), nameof(CyclopsHelmHUDManager.Update)), HarmonyPostfix]
//        public static void CyclopsHelmHUDManager_Update_2(CyclopsHelmHUDManager __instance)
//        {
//            if (!Main.Config.NoInvisibleHelmButtons) return;

//            var helmHUDVisuals = __instance.transform.Find("HelmHUDVisuals")?.gameObject;
//            if (helmHUDVisuals != null)
//            {
//                bool hudActive = (bool)AccessTools.Field(typeof(CyclopsHelmHUDManager), "hudActive")
//                    .GetValue(__instance);

//                // Start fade coroutine
//                if (!hudActive && helmHUDVisuals.activeSelf)
//                {
//                    __instance.StartCoroutine(DelayedHUDDisable(helmHUDVisuals));
//                }
//                else if (hudActive)
//                {
//                    helmHUDVisuals.SetActive(true);
//                }
//            }
//        }

//        // Preserve fade out
//        private static IEnumerator DelayedHUDDisable(GameObject hud)
//        {
//            yield return new WaitForSeconds(1f);
//            hud.SetActive(false);
//        }
//    }
//}