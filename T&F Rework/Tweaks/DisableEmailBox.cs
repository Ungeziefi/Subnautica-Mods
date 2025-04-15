using HarmonyLib;

namespace Ungeziefi.Tweaks
{
    [HarmonyPatch]
    public class DisableEmailBox
    {
        [HarmonyPatch(typeof(MainMenuRightSide), nameof(MainMenuRightSide.Start)), HarmonyPostfix]
        public static void MainMenuRightSide_Start(MainMenuRightSide __instance)
        {
            if (!Main.Config.DisableEmailBox || MiscSettings.newsEnabled) return;

            var emailBox = __instance.transform.Find("Home/EmailBox")?.gameObject;
            if (emailBox)
                emailBox.SetActive(false);
        }
    }
}