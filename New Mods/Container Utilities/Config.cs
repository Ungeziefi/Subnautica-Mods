using Nautilus.Json;
using Nautilus.Options.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ungeziefi.Container_Utilities
{
    [Menu("Container Utilities")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Dim unallowed items")]
        public bool DimUnallowedItems = true;

        [Toggle(Label = "Custom slot sizes")]
        public bool CustomSlotSizes = false;

        [Toggle(Label = "All items 1x1")]
        public bool AllItems1x1 = false;

        [Toggle("<color=#FFAC09FF>Quick transfer</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool QuickTransferDivider;

        [Toggle(Label = "Enable transfer all items", Tooltip = "Hold a key to transfer all items between containers at once.")]
        public bool EnableTransferAllItems = true;

        [Toggle(Label = "Enable transfer similar items", Tooltip = "Hold a key to transfer all items of the same type between containers at once.")]
        public bool EnableTransferSimilarItems = true;

        [Toggle("<color=#FFAC09FF>Custom container sizes</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool CustomContainerSizesDivider;

        [Toggle(Label = "Enable custom container sizes", Tooltip = "Enable resizing storage containers with the settings below.")]
        public bool EnableCustomContainerSizes = false;

        [Slider(Label = "Standard locker width", DefaultValue = 6, Min = 1, Max = 8, Step = 1)]
        public int StandardLockerWidth = 6;

        [Slider(Label = "Standard locker height", DefaultValue = 8, Min = 1, Max = 10, Step = 1)]
        public int StandardLockerHeight = 8;

        [Slider(Label = "Wall locker width", DefaultValue = 5, Min = 1, Max = 8, Step = 1)]
        public int WallLockerWidth = 5;

        [Slider(Label = "Wall locker height", DefaultValue = 6, Min = 1, Max = 10, Step = 1)]
        public int WallLockerHeight = 6;

        [Slider(Label = "Waterproof locker width", DefaultValue = 3, Min = 1, Max = 8, Step = 1)]
        public int WaterproofLockerWidth = 3;

        [Slider(Label = "Waterproof locker height", DefaultValue = 6, Min = 1, Max = 10, Step = 1)]
        public int WaterproofLockerHeight = 6;

        [Slider(Label = "Seamoth storage width", DefaultValue = 4, Min = 1, Max = 8, Step = 1)]
        public int SeamothStorageWidth = 4;

        [Slider(Label = "Seamoth storage height", DefaultValue = 4, Min = 1, Max = 10, Step = 1)]
        public int SeamothStorageHeight = 4;

        [Slider(Label = "PRAWN suit storage width", DefaultValue = 6, Min = 1, Max = 8, Step = 1)]
        public int ExosuitStorageWidth = 6;

        [Slider(Label = "PRAWN suit storage height", DefaultValue = 4, Min = 1, Max = 10, Step = 1)]
        public int ExosuitStorageHeight = 4;

        [Slider(Label = "Escape pod locker width", DefaultValue = 4, Min = 1, Max = 8, Step = 1)]
        public int EscapePodLockerWidth = 4;

        [Slider(Label = "Escape pod locker height", DefaultValue = 8, Min = 1, Max = 10, Step = 1)]
        public int EscapePodLockerHeight = 8;

        [Slider(Label = "Cyclops locker width", DefaultValue = 3, Min = 1, Max = 8, Step = 1)]
        public int CyclopsLockerWidth = 3;

        [Slider(Label = "Cyclops locker height", DefaultValue = 6, Min = 1, Max = 10, Step = 1)]
        public int CyclopsLockerHeight = 6;

        [Slider(Label = "Bioreactor width", DefaultValue = 4, Min = 1, Max = 8, Step = 1)]
        public int BioreactorWidth = 4;

        [Slider(Label = "Bioreactor height", DefaultValue = 4, Min = 1, Max = 8, Step = 1)]
        public int BioreactorHeight = 4;

        [Toggle("<color=#FFAC09FF>Water filtration machine</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool WaterFiltrationMachineDivider;

        [Slider(Label = "Storage width", DefaultValue = 2, Min = 1, Max = 6, Step = 1)]
        public int FiltrationWidth = 2;

        [Slider(Label = "Storage height", DefaultValue = 2, Min = 1, Max = 6, Step = 1)]
        public int FiltrationHeight = 2;

        [Slider(Label = "Max water bottles", Tooltip = "Maximum water bottles that can be stored (shares space with salt).", DefaultValue = 2, Min = 1, Max = 10, Step = 1)]
        public int FiltrationMaxWater = 2;

        [Slider(Label = "Max salt", Tooltip = "Maximum salt that can be stored (shares space with water bottles).", DefaultValue = 2, Min = 1, Max = 10, Step = 1)]
        public int FiltrationMaxSalt = 2;

        [Toggle("<color=#FFAC09FF>Trashcan</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool TrashcanDivider;

        [Slider(Label = "Trashcan width", DefaultValue = 4, Min = 1, Max = 8, Step = 1)]
        public int TrashcanWidth = 4;

        [Slider(Label = "Trashcan height", DefaultValue = 4, Min = 1, Max = 8, Step = 1)]
        public int TrashcanHeight = 4;

        [Slider(Label = "Destruction delay (seconds)", Tooltip = "How long items stay in the trashcan before being destroyed.", DefaultValue = 5, Min = 0, Max = 30, Step = 1)]
        public float TrashcanDestroyDelay = 5f;

        [Slider(Label = "Destruction interval (seconds)", Tooltip = "How frequently items are destroyed from the trashcan.", DefaultValue = 1, Min = 0.1f, Max = 5, Step = 0.1f)]
        public float TrashcanDestroyInterval = 1f;

        //[Toggle("<color=#FFAC09FF>Custom inventory size</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        //public bool CustomInventorySizeDivider;

        //[Toggle(Label = "Enable custom inventory size", Tooltip = "Enable resizing the player's personal inventory.")]
        //public bool EnableCustomInventorySize = true;

        //[Slider(Label = "Inventory width", DefaultValue = 6, Min = 1, Max = 6, Step = 1)]
        //public int InventoryWidth = 6;

        //[Slider(Label = "Inventory height", DefaultValue = 8, Min = 1, Max = 16, Step = 1)]
        //public int InventoryHeight = 8;

        [Toggle("<color=#FFAC09FF>Inventory warnings</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
        public bool InventoryWarningsDivider;

        [Toggle(Label = "Show free slots warning", Tooltip = "Show a message when picking up items with limited inventory space.")]
        public bool ShowFreeSlotWarnings = true;

        [Slider(Label = "Free slots threshold", Tooltip = "The number of remaining free slots below which the warning shows.", DefaultValue = 5, Min = 1, Max = 20, Step = 1)]
        public int FreeSlotWarningThreshold = 3;

        [Toggle(Label = "Show full inventory warning")]
        public bool ShowFullInventoryWarning = true;

        [Toggle(Label = "Full inventory audio cue")]
        public bool FullInventoryAudioCue = true;

        #region JSON Converter
        [JsonConverter(typeof(CompactItemSizeOverrideConverter))]
        public class ItemSizeOverride
        {
            public TechType TechType;
            public Vector2int Size;

            public ItemSizeOverride() { }

            public ItemSizeOverride(TechType techType, Vector2int size)
            {
                TechType = techType;
                Size = size;
            }
        }

        public List<ItemSizeOverride> SizeOverrides = new()
        {
            new(TechType.Seaglide, new Vector2int(1, 2)),
            new(TechType.ScrapMetal, new Vector2int(1, 1))
        };
        #endregion
    }
}