﻿// Doesn't work for some reason, maybe I'll figure it out later

//using HarmonyLib;

//namespace Ungeziefi.Tweaks
//{
//    [HarmonyPatch(typeof(FruitPlant))]
//    internal class FruitGrowthTimeMultiplier
//    {
//        [HarmonyPatch("Start"), HarmonyPrefix]
//        public static void Start(FruitPlant __instance)
//        {
//            if (Main.Config.FruitGrowthTimeMultiplier == 1)
//            {
//                return;
//            }

//            __instance.fruitSpawnInterval *= Main.Config.FruitGrowthTimeMultiplier * DayNightCycle.kDayLengthSeconds;
//        }
//    }
//}