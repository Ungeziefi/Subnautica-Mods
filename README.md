### Fixes - Tweaks
A rework of [Tweaks and Fixes](https://www.nexusmods.com/subnautica/mods/722), the main differences are:
- Compatibility, thanks to the use of Transpilers and by avoiding `bool Prefix` patches that replace the original method completely.
- Modularity, each feature can be toggled off. This also helps compatibility.
- Maintainability, each feature is in its own source file and is easier to read.
- New bug fixes and new tweaks.
- And more...
 
You can find complete documentation in the [Docs folder](https://github.com/Ungeziefi/Subnautica-Mods/tree/main/T%26F%20Rework/Docs).

<small>**Disclaimer**: None of the T&F code was a direct copy, assuming the feature isn't simple to the point it can't be implemented any differently.</small>

---

### New Mods
These are separate from the T&F rework because they don't fit its scope. More information in the respective Nexus Mods description.
- [Seamoth Barrel Roll](https://www.nexusmods.com/subnautica/mods/2012)
- [Camera Zoom](https://www.nexusmods.com/subnautica/mods/2013)
- [Custom Sunbeam Countdown](https://www.nexusmods.com/subnautica/mods/2014)
- [Rotatable Ladders](https://www.nexusmods.com/subnautica/mods/2015)
- [PDA Movement and Bobbing](https://www.nexusmods.com/subnautica/mods/2017)
- [Cockpit Free Look](https://www.nexusmods.com/subnautica/mods/2026)

**Note**: I realized after release that [Mikjaw](https://next.nexusmods.com/profile/Mikjaw) already created mods with the same goal as Seamoth Barrel Roll and Cockpit Free Look. Check the originals and make your own choice!
- [Roll Control](https://www.nexusmods.com/subnautica/mods/515)
- [Free Look](https://www.nexusmods.com/subnautica/mods/517)

---

### Remakes
These are just for **personal use** and to have public source code.
- [Anisotropic Fix](https://www.nexusmods.com/subnautica/mods/185) (Credits to [WhoTnT](https://next.nexusmods.com/profile/WhoTnT).)
- [Dynamic Scanner Blips](https://www.nexusmods.com/subnautica/mods/1160) (Optimized and moved the settings to the Mod menu. Credits to [WhoTnT](https://next.nexusmods.com/profile/WhoTnT).)
- [Abandon Ship During Cyclops Fire](https://www.nexusmods.com/subnautica/mods/1265) (Ported to Nautilus and added an in-game toggle. Credits to [Aishsh506](https://next.nexusmods.com/profile/Aishsh506).)

---

### Requirements
- [Tobey's BepInEx Pack](https://www.nexusmods.com/subnautica/mods/1108)
- [Nautilus](https://www.nexusmods.com/subnautica/mods/1262)

---

### Notes
- Features involving text use hardcoded strings in English. Feel free to PR localization.
- By default, all fixes are enabled and tweaks are disabled. You can change this in the respective config file or through the in-game Mod menu.

---

### Contact
@ungeziefi on Discord, you can find me in the [Subnautica Modding](https://discord.com/invite/subnautica-modding-324207629784186882) server.

---

  ### Credits
- [qqqbbb](https://next.nexusmods.com/profile/qqqbbb) for [Tweaks and Fixes](https://www.nexusmods.com/subnautica/mods/722).
- [MrOshaw](https://github.com/mroshaw) for the [Subnautica Modding Guide](https://mroshaw.github.io/).
- The [Subnautica Modding](https://discord.com/invite/subnautica-modding-324207629784186882) Discord server for their help and resources, especially EldritchCarMaker and Essence.
- [WhoTnT](https://next.nexusmods.com/profile/WhoTnT) for [Anisotropic Fix](https://www.nexusmods.com/subnautica/mods/185) and [Dynamic Scanner Blips](https://www.nexusmods.com/subnautica/mods/1160).

---

Built with [Subnautica.Templates](https://www.nuget.org/packages/Subnautica.Templates) and [Visual Studio 2022 Community Edition](https://visualstudio.microsoft.com/vs/community/).
