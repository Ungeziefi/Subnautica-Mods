using HarmonyLib;
using System.Collections.Generic;
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
            // Check if feature is enabled
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
            if (!Main.Config.EnableFeature) return;

            Dictionary<string, float> timersCopy;
            lock (timers) { timersCopy = new Dictionary<string, float>(timers); }

            List<string> expiredBars = new List<string>(timersCopy.Count / 4);

            foreach (var kvp in timersCopy)
            {
                string id = kvp.Key;
                float time = kvp.Value - Time.deltaTime;

                // Mark for removal if expired
                if (time <= 0)
                {
                    expiredBars.Add(id);
                    continue;
                }

                // Update timer
                lock (timers) { timers[id] = time; }

                // Last second fade out
                if (time >= 1.0f || !healthbars.TryGetValue(id, out GameObject bar))
                    continue;

                // Get UI components
                Transform bgTransform = bar.transform.Find("Background");
                if (bgTransform == null) continue;

                Image bgImage = bgTransform.GetComponent<Image>();
                Image healthImage = bgTransform.Find("Health")?.GetComponent<Image>();

                if (bgImage == null || healthImage == null) continue;

                // Alpha fade based on remaining time
                Color bgColor = Main.Config.BackgroundColor;
                Color healthColor = Main.Config.HealthColor;

                float alpha = time;
                bgColor.a *= alpha;
                healthColor.a *= alpha;

                bgImage.color = bgColor;
                healthImage.color = healthColor;
            }

            // Skip cleanup if nothing to remove
            if (expiredBars.Count == 0) return;

            // Clean up expired health bars
            foreach (string id in expiredBars)
            {
                // Destroy GameObject if it exists
                if (healthbars.TryGetValue(id, out GameObject bar) && bar != null)
                    GameObject.Destroy(bar);

                // Remove from tracking dictionaries
                lock (timers) { timers.Remove(id); }
                lock (healthbars) { healthbars.Remove(id); }
            }
        }
    }
}