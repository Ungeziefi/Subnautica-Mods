using System.Collections.Generic;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace Ungeziefi.Custom_Hotkeys_Remake
{
    [Menu("Custom Hotkeys Remake")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        public class CommandHotkey
        {
            public string Name;
            public List<KeyCode> Keys = new();
            public List<string> Commands = new();
            public float ExecutionDelay = 0.1f;

            public CommandHotkey() { }

            public CommandHotkey(string name, List<KeyCode> keys, List<string> commands, float executionDelay = 0.1f)
            {
                Name = name;
                Keys = keys;
                Commands = commands;
                ExecutionDelay = executionDelay;
            }
        }

        public List<CommandHotkey> HotkeyConfigurations = new()
        {
            new CommandHotkey("Example - Simple",
            new List<KeyCode> { KeyCode.K },
            new List<string>
            {
                "warpforward 10",
                "spawn seamoth"
            },
            0.1f),

            new CommandHotkey("Example - Multi-Key",
            new List<KeyCode> { KeyCode.LeftControl, KeyCode.H },
            new List<string>
            {
                "warpforward 10",
                "spawn seamoth"
            },
            0.1f)
        };
    }
}