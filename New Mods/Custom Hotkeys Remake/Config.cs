using Nautilus.Json;
using Nautilus.Options.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Custom_Hotkeys_Remake
{
    [Menu("Custom Hotkeys Remake")]
    public class Config : ConfigFile
    {
        [Toggle(Label = "Enable feature")]
        public bool EnableFeature = true;

        [Slider(Label = "Command execution delay (seconds)",
            DefaultValue = 0.1f, Min = 0f, Max = 5.0f, Step = 0.1f, Format = "{0:0.0}")]
        public float CommandExecutionDelay = 0.1f;

        public class CommandHotkey
        {
            public string Name;
            public List<KeyCode> Keys = new List<KeyCode>();
            public List<string> Commands = new List<string>();
            public bool ExecuteInstantly = false;

            public CommandHotkey() { }

            public CommandHotkey(string name, List<KeyCode> keys, List<string> commands, bool executeInstantly = false)
            {
                Name = name;
                Keys = keys;
                Commands = commands;
                ExecuteInstantly = executeInstantly;
            }
        }

        public List<CommandHotkey> HotkeyConfigurations = new List<CommandHotkey>
        {
            new CommandHotkey("Example - Simple",
            new List<KeyCode> { KeyCode.K },
            new List<string>
            {
                "warpforward 10",
                "spawn seamoth"
            }),

            new CommandHotkey("Example - Multi-Key",
            new List<KeyCode> { KeyCode.LeftControl, KeyCode.H },
            new List<string>
            {
                "warpforward 10",
                "spawn seamoth"
            })
        };
    }
}