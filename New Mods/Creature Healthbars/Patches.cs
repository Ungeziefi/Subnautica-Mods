using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Ungeziefi.Creature_Healthbars
{
    [HarmonyPatch]
    public partial class CreatureHealthbars
    {
        [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.TakeDamage)), HarmonyPostfix]
        public static void LiveMixin_TakeDamage(LiveMixin __instance, float originalDamage, GameObject dealer = null)
        {
            if (!Main.Config.EnableFeature ||
                __instance == null ||
                originalDamage <= 0 ||
                !__instance.IsAlive() ||
                __instance.GetComponent<Creature>() is not Creature creature)
                return;

            // Apparently hits from the player don't register a dealer
            bool isPlayerDamage = dealer == Player.main?.gameObject || dealer == null;

            if (Main.Config.OnlyShowForPlayerDamage && !isPlayerDamage)
                return;

            // Check creature type filter
            bool isPredator = creature.GetComponent<AggressiveWhenSeeTarget>() != null;
            if ((Main.Config.CreatureFilter == CreatureFilterOption.OnlyPredators && !isPredator) ||
                (Main.Config.CreatureFilter == CreatureFilterOption.OnlyNonPredators && isPredator))
                return;

            string id = GetCreatureId(creature.gameObject);
            if (string.IsNullOrEmpty(id)) return;

            // Init sprite and reset combat timer
            if (roundedSprite == null) CreateSprite();

            lock (timers) { timers[id] = Main.Config.DisplayDuration; }

            // Display or update the health bar
            ShowHealthBar(creature, id, __instance.GetHealthFraction());
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Update)), HarmonyPostfix]
        public static void Player_Update()
        {
            if (!Main.Config.EnableFeature || timers == null || healthbars == null) return;

            Dictionary<string, float> timersCopy;
            lock (timers) { timersCopy = new Dictionary<string, float>(timers); }

            List<string> expiredBars = new List<string>();

            // Process timers and identify expired bars
            foreach (var kvp in timersCopy)
            {
                if (string.IsNullOrEmpty(kvp.Key)) continue;

                string id = kvp.Key;
                float time = kvp.Value - Time.deltaTime;

                if (time <= 0)
                {
                    expiredBars.Add(id);
                    continue;
                }

                // Update timer
                lock (timers)
                {
                    if (timers.ContainsKey(id))
                        timers[id] = time;
                }

                // Fade effect for last second
                if (time < 1.0f && healthbars.TryGetValue(id, out GameObject bar) && bar != null)
                {
                    Transform bgTransform = bar.transform?.Find("Background");
                    if (bgTransform == null) continue;

                    Image bgImage = bgTransform.GetComponent<Image>();
                    Image healthImage = bgTransform.Find("Health")?.GetComponent<Image>();
                    if (bgImage == null || healthImage == null) continue;

                    float alpha = time;
                    bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, alpha);
                    healthImage.color = new Color(healthImage.color.r, healthImage.color.g, healthImage.color.b, alpha);
                }
            }

            // Remove expired bars
            foreach (string id in expiredBars)
            {
                if (string.IsNullOrEmpty(id)) continue;

                if (healthbars.TryGetValue(id, out GameObject bar) && bar != null)
                    GameObject.Destroy(bar);

                lock (timers) { timers.Remove(id); }
                lock (healthbars) { healthbars.Remove(id); }
            }
        }
    }
}