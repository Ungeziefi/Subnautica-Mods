using HarmonyLib;

namespace Ungeziefi.Fixes
{
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
}