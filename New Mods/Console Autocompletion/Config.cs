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

        [Toggle(Label = "Log matching items")]
        public bool LogMatchingItems = false;
    }
}