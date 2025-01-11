using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch]
    public class AddMissingBulbBushDataEntries
    {
        [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Initialize)), HarmonyPostfix]
        public static void PDAScanner_Initialize()
        {
            if (!Main.Config.AddMissingBulbBushDataEntries)
            {
                return;
            }

            // Add missing mapping entries
            if (PDAScanner.mapping.ContainsKey(TechType.MediumKoosh))
            {
                var entryData = PDAScanner.mapping[TechType.MediumKoosh];
                if (!PDAScanner.mapping.ContainsKey(TechType.LargeKoosh))
                {
                    PDAScanner.mapping.Add(TechType.LargeKoosh, entryData);
                }
                if (!PDAScanner.mapping.ContainsKey(TechType.SmallKoosh))
                {
                    PDAScanner.mapping.Add(TechType.SmallKoosh, entryData);
                }
            }
        }

        [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Unlock)), HarmonyPostfix]
        public static void PDAScanner_Unlock(PDAScanner.EntryData entryData)
        {
            if (!Main.Config.AddMissingBulbBushDataEntries)
            {
                return;
            }

            // Unlock other entries
            if (entryData.key == TechType.MediumKoosh || entryData.key == TechType.SmallKoosh || entryData.key == TechType.LargeKoosh)
            {
                PDAScanner.complete.Add(TechType.LargeKoosh);
                PDAScanner.complete.Add(TechType.SmallKoosh);
                PDAScanner.complete.Add(TechType.MediumKoosh);
            }
        }
    }
}