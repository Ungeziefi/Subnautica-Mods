using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Ungeziefi.Container_Utilities
{
    [HarmonyPatch]
    public class CustomSlotSizes
    {

        private static readonly Dictionary<TechType, Vector2int> customItemSizes = new();
        private static bool initialized = false;

        private static void InitializeFromConfig()
        {
            if (initialized) return;

            customItemSizes.Clear();

            foreach (var override_ in Main.Config.SizeOverrides)
            {
                if (!customItemSizes.ContainsKey(override_.TechType))
                {
                    customItemSizes.Add(override_.TechType, override_.Size);
                }
            }

            initialized = true;
        }

        public static bool HasCustomSizeFor(TechType techType)
        {
            InitializeFromConfig();
            return customItemSizes.ContainsKey(techType);
        }

        public static void SetCustomSize(TechType techType, Vector2int size)
        {
            InitializeFromConfig();

            if (customItemSizes.ContainsKey(techType))
                customItemSizes[techType] = size;
            else
                customItemSizes.Add(techType, size);
        }

        public static bool RemoveCustomSize(TechType techType)
        {
            InitializeFromConfig();
            return customItemSizes.Remove(techType);
        }

        public static void ClearCustomSizes()
        {
            customItemSizes.Clear();
            initialized = false;
        }

        [HarmonyPatch(typeof(TechData), nameof(TechData.GetItemSize)), HarmonyPrefix]
        public static bool TechData_GetItemSize(TechType techType, ref Vector2int __result)
        {
            InitializeFromConfig();

            if (!Main.Config.CustomSlotSizes) return true;

            if (customItemSizes.TryGetValue(techType, out Vector2int customSize))
            {
                __result = customSize;
                return false;
            }

            return true;
        }

        public static Dictionary<TechType, Vector2int> GetAllCustomSizes()
        {
            InitializeFromConfig();
            return new Dictionary<TechType, Vector2int>(customItemSizes);
        }

        public static void AddSizeOverrideToConfig(TechType techType, Vector2int size)
        {
            SetCustomSize(techType, size);

            bool exists = Main.Config.SizeOverrides.Exists(o => o.TechType == techType);
            if (!exists)
            {
                Main.Config.SizeOverrides.Add(new Config.ItemSizeOverride(techType, size));
            }
        }

        public static bool RemoveSizeOverrideFromConfig(TechType techType)
        {
            RemoveCustomSize(techType);

            int index = Main.Config.SizeOverrides.FindIndex(o => o.TechType == techType);
            if (index >= 0)
            {
                Main.Config.SizeOverrides.RemoveAt(index);
                return true;
            }
            return false;
        }
    }

    #region JSON Converter
    public class CompactItemSizeOverrideConverter : JsonConverter<Config.ItemSizeOverride>
    {
        public override void WriteJson(JsonWriter writer, Config.ItemSizeOverride value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("TechType");
            writer.WriteValue(value.TechType.ToString());
            writer.WritePropertyName("Size");
            writer.WriteValue($"{value.Size.x},{value.Size.y}");
            writer.WriteEndObject();
        }

        public override Config.ItemSizeOverride ReadJson(JsonReader reader, Type objectType, Config.ItemSizeOverride existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            TechType techType = TechType.None;
            Vector2int size = new Vector2int(1, 1);

            reader.Read(); // Start object

            while (reader.TokenType != JsonToken.EndObject)
            {
                string propertyName = reader.Value.ToString();
                reader.Read();

                if (propertyName == "TechType")
                {
                    if (Enum.TryParse(reader.Value.ToString(), out TechType parsedTechType))
                        techType = parsedTechType;
                }
                else if (propertyName == "Size")
                {
                    string sizeStr = reader.Value.ToString();
                    string[] parts = sizeStr.Split(',');

                    if (parts.Length == 2 &&
                        int.TryParse(parts[0], out int x) &&
                        int.TryParse(parts[1], out int y))
                    {
                        size = new Vector2int(x, y);
                    }
                }

                reader.Read();
            }

            return new Config.ItemSizeOverride(techType, size);
        }
    }
    #endregion
}