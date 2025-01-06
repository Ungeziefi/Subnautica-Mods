using HarmonyLib;

namespace Ungeziefi.Fixes
{
    [HarmonyPatch(typeof(PDAScanner))]
    public class AddMissingBulbBushDataEntries
    {
        [HarmonyPatch(nameof(PDAScanner.Initialize)), HarmonyPostfix]
        public static void Initialize()
        {
            if (!Main.Config.AddMissingBulbBushDataEntries)
            {
                return;
            }

            // Add the missing mapping entries
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

        [HarmonyPatch(nameof(PDAScanner.Unlock)), HarmonyPostfix]
        public static void Unlock(PDAScanner.EntryData entryData)
        {
            if (!Main.Config.AddMissingBulbBushDataEntries)
            {
                return;
            }

            // Unlock the other entries when unlocking any of them
            if (entryData.key == TechType.MediumKoosh || entryData.key == TechType.SmallKoosh || entryData.key == TechType.LargeKoosh)
            {
                PDAScanner.complete.Add(TechType.LargeKoosh);
                PDAScanner.complete.Add(TechType.SmallKoosh);
                PDAScanner.complete.Add(TechType.MediumKoosh);
            }
        }
    }
}