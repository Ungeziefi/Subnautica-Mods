using System.Collections.Generic;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace Ungeziefi.Container_Utilities;

[Menu("Container Utilities")]
public class Config : ConfigFile
{
    [Toggle(Label = "Dim unallowed items")]
    public bool DimUnallowedItems = true;

    [Toggle(Label = "Custom item sizes")] public bool CustomItemSizes = false;

    public List<ItemSizeOverride> SizeOverrides = new()
    {
        new ItemSizeOverride(TechType.ScrapMetal, new Vector2int(2, 2)) // Example
    };

    public class ItemSizeOverride
    {
        public TechType TechType;
        public Vector2int Size;

        public ItemSizeOverride(TechType techType, Vector2int size)
        {
            TechType = techType;
            Size = size;
        }
    }

    [Toggle(Label = "All items 1x1")] public bool AllItems1x1 = false;

    [Toggle(
        "<color=#FFAC09FF>Quick transfer</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
    public bool QuickTransferDivider;

    [Toggle(Label = "Enable transfer all items",
        Tooltip = "Hold a key to transfer all items between containers at once.")]
    public bool EnableTransferAllItems = true;

    [Toggle(Label = "Enable transfer similar items",
        Tooltip = "Hold a key to transfer all items of the same type between containers at once.")]
    public bool EnableTransferSimilarItems = true;

    [Toggle(
        "<color=#FFAC09FF>Container prompts</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
    public bool ContainerPromptsDivider;

    [Toggle(Label = "Show storage information")]
    public bool ShowStorageInformation = true;

    [Toggle(Label = "Color coded prompts")]
    public bool ColorCodedPrompts = true;

    [Toggle(
        "<color=#FFAC09FF>Custom container sizes</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
    public bool CustomContainerSizesDivider;

    [Toggle(Label = "Enable custom container sizes",
        Tooltip = "Enable resizing storage containers with the settings below.")]
    public bool EnableCustomContainerSizes = false;

    [Slider(Label = "Standard locker width", DefaultValue = 6, Min = 1, Max = 8, Step = 1)]
    public int StandardLockerWidth = 6;

    [Slider(Label = "Standard locker height", DefaultValue = 8, Min = 1, Max = 8, Step = 1)]
    public int StandardLockerHeight = 8;

    [Slider(Label = "Wall locker width", DefaultValue = 5, Min = 1, Max = 8, Step = 1)]
    public int WallLockerWidth = 5;

    [Slider(Label = "Wall locker height", DefaultValue = 6, Min = 1, Max = 8, Step = 1)]
    public int WallLockerHeight = 6;

    [Slider(Label = "Waterproof locker width", DefaultValue = 3, Min = 1, Max = 8, Step = 1)]
    public int WaterproofLockerWidth = 3;

    [Slider(Label = "Waterproof locker height", DefaultValue = 6, Min = 1, Max = 8, Step = 1)]
    public int WaterproofLockerHeight = 6;

    [Slider(Label = "Seamoth storage width", DefaultValue = 4, Min = 1, Max = 8, Step = 1)]
    public int SeamothStorageWidth = 4;

    [Slider(Label = "Seamoth storage height", DefaultValue = 4, Min = 1, Max = 4, Step = 1)]
    public int SeamothStorageHeight = 4;

    [Slider(Label = "PRAWN suit storage width", DefaultValue = 6, Min = 1, Max = 8, Step = 1)]
    public int ExosuitStorageWidth = 6;

    [Slider(Label = "PRAWN suit storage height", DefaultValue = 4, Min = 1, Max = 4, Step = 1)]
    public int ExosuitStorageHeight = 4;

    [Slider(Label = "Escape pod locker width", DefaultValue = 4, Min = 1, Max = 8, Step = 1)]
    public int EscapePodLockerWidth = 4;

    [Slider(Label = "Escape pod locker height", DefaultValue = 8, Min = 1, Max = 8, Step = 1)]
    public int EscapePodLockerHeight = 8;

    [Slider(Label = "Cyclops locker width", DefaultValue = 3, Min = 1, Max = 8, Step = 1)]
    public int CyclopsLockerWidth = 3;

    [Slider(Label = "Cyclops locker height", DefaultValue = 6, Min = 1, Max = 8, Step = 1)]
    public int CyclopsLockerHeight = 6;

    [Slider(Label = "Bioreactor storage width", DefaultValue = 4, Min = 1, Max = 8, Step = 1)]
    public int BioreactorStorageWidth = 4;

    [Slider(Label = "Bioreactor storage height", DefaultValue = 4, Min = 1, Max = 8, Step = 1)]
    public int BioreactorStorageHeight = 4;

    [Toggle(
        "<color=#FFAC09FF>Water filtration machine</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
    public bool WaterFiltrationMachineDivider;

    [Slider(Label = "Storage width", DefaultValue = 2, Min = 1, Max = 6, Step = 1)]
    public int WFMStorageWidth = 2;

    [Slider(Label = "Storage height", DefaultValue = 2, Min = 1, Max = 6, Step = 1)]
    public int WFMStorageHeight = 2;

    [Slider(Label = "Max water bottles", Tooltip = "Maximum water bottles that can be stored (shares space with salt).",
        DefaultValue = 2, Min = 1, Max = 8, Step = 1)]
    public int WFMMaxWaterBottles = 2;

    [Slider(Label = "Max salt", Tooltip = "Maximum salt that can be stored (shares space with water bottles).",
        DefaultValue = 2, Min = 1, Max = 8, Step = 1)]
    public int WFMMaxSalt = 2;

    [Toggle(
        "<color=#FFAC09FF>Trashcan</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
    public bool TrashcanDivider;

    [Slider(Label = "Storage width", DefaultValue = 4, Min = 1, Max = 8, Step = 1)]
    public int TrashcanStorageWidth = 4;

    [Slider(Label = "Storage height", DefaultValue = 4, Min = 1, Max = 8, Step = 1)]
    public int TrashcanStorageHeight = 4;

    [Slider(Label = "Destruction delay (seconds)",
        Tooltip = "How long items stay in the trashcan before being destroyed.",
        DefaultValue = 5f, Min = 0f, Max = 30f, Step = 0.1f, Format = "{0:0.0}s")]
    public float TrashcanDestroyDelay = 5f;

    [Slider(Label = "Destruction interval (seconds)", Tooltip = "How frequently items are destroyed from the trashcan.",
        DefaultValue = 1f, Min = 0.1f, Max = 30f, Step = 0.1f, Format = "{0:0.0}s")]
    public float TrashcanDestroyInterval = 1f;

    [Toggle(
        "<color=#FFAC09FF>Inventory warnings</color> <alpha=#00>----------------------------------------------------------------------------</alpha>")]
    public bool InventoryWarningsDivider;

    [Toggle(Label = "Show free slots warning",
        Tooltip = "Show a message when picking up items with limited inventory space.")]
    public bool ShowFreeSlotWarnings = true;

    [Slider(Label = "Free slots threshold",
        Tooltip = "The number of remaining free slots below which the warning shows.", DefaultValue = 5, Min = 1,
        Max = 20, Step = 1)]
    public int FreeSlotWarningThreshold = 5;

    [Toggle(Label = "Show full inventory warning")]
    public bool ShowFullInventoryWarning = true;

    [Toggle(Label = "Full inventory audio cue")]
    public bool FullInventoryAudioCue = true;
}