using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Creature_Healthbars
{
    [HarmonyPatch]
    public partial class CreatureHealthbars
    {
        private static readonly Dictionary<string, GameObject> healthbars = new Dictionary<string, GameObject>();
        private static readonly Dictionary<string, float> timers = new Dictionary<string, float>();
        private static Sprite roundedSprite;
        private static readonly float worldToUIScale = 10f; // Conversion factor
        private static readonly float baseWidth = 4.0f;

        [HarmonyPatch(typeof(Player), nameof(Player.Awake)), HarmonyPostfix]
        public static void Player_Awake()
        {
            // Change events
            Config.OnVisualSettingsChanged += UpdateAllHealthbars;
            Config.OnSpriteSettingsChanged += ResetSpriteAndUpdateHealthbars;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.OnDestroy)), HarmonyPostfix]
        public static void Player_OnDestroy()
        {
            // Does this even happen? If so, cleanup
            Config.OnVisualSettingsChanged -= UpdateAllHealthbars;
            Config.OnSpriteSettingsChanged -= ResetSpriteAndUpdateHealthbars;
        }

        private static void ResetSpriteAndUpdateHealthbars()
        {
            roundedSprite = null;
            CreateSprite();
            UpdateAllHealthbars();
        }

        private static void UpdateAllHealthbars()
        {
            if (!Main.Config.EnableFeature) return;

            lock (healthbars)
            {
                foreach (var kvp in new Dictionary<string, GameObject>(healthbars))
                {
                    string id = kvp.Key;
                    GameObject healthbar = kvp.Value;

                    if (healthbar == null) continue;

                    // Find the creature
                    Transform parent = healthbar.transform.parent;
                    if (parent == null) continue;

                    Creature creature = parent.GetComponent<Creature>();
                    if (creature == null) continue;

                    // Get LiveMixin for health
                    LiveMixin liveMixin = creature.GetComponent<LiveMixin>();
                    if (liveMixin == null) continue;

                    // Update the health bar
                    ShowHealthBar(creature, id, liveMixin.GetHealthFraction());
                }
            }
        }

        private static Vector3 CalculateHealthBarPosition(GameObject creature)
        {
            Bounds bounds = GetCreatureBounds(creature);

            // Position above creature's center
            return new Vector3(0, bounds.size.y, 0);
        }

        private static Bounds GetCreatureBounds(GameObject creature)
        {
            Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

            Collider collider = creature.GetComponent<Collider>();
            if (collider != null)
            {
                bounds = collider.bounds;
                // Convert to local space
                bounds.center = creature.transform.InverseTransformPoint(bounds.center);
                return bounds;
            }

            // If no Collider
            Renderer renderer = creature.GetComponent<Renderer>();
            if (renderer != null)
            {
                bounds = renderer.bounds;
                // Convert to local space
                bounds.center = creature.transform.InverseTransformPoint(bounds.center);
                return bounds;
            }

            // Fallback
            bounds.center = Vector3.zero;
            bounds.size = new Vector3(1f, 1f, 1f);
            return bounds;
        }

        private static string GetCreatureId(GameObject creature)
        {
            UniqueIdentifier id = creature.GetComponent<UniqueIdentifier>();
            return id != null && !string.IsNullOrEmpty(id.Id) ? id.Id : creature.GetInstanceID().ToString();
        }
    }
}