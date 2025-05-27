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
        private static readonly float baseHeight = 1.0f;

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