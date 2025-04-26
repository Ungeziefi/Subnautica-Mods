using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace Ungeziefi.Console_Autocompletion
{
    [Menu("Console Autocompletion")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        [Keybind(Label = "Autocompletion key", Tooltip = "Tab key by default.")]
        public KeyCode ConsoleAutocompletionKey = KeyCode.Tab;

        [Toggle(Label = "Log matching items")]
        public bool LogMatchingItems = false;
    }
}