This project is mainly aimed at porting the majority of features from [Tweaks and Fixes](https://github.com/qqqbbb/Tweaks-and-Fixes) by [qqqbbb](https://github.com/qqqbbb) to a **clean and optimized** codebase that is easier to maintain or contribute to. It is also **more compatible**, largely thanks to avoiding `bool Prefix` patches that skip the original code and by making **every feature optional**.

You can find a complete list of current, planned, and unplanned features in the [Docs folder](https://github.com/Ungeziefi/Subnautica-Mods/tree/main/Docs), along with notes on any user-facing differences in how the features are implemented.

<small>**Disclaimer**: None of the T&F code was a direct copy, assuming the feature isn't simple to the point it can't be implemented any differently.</small>

Built with [Subnautica.Templates](https://www.nuget.org/packages/Subnautica.Templates) and [Visual Studio 2022 Community Edition](https://visualstudio.microsoft.com/vs/community/).

---

### Requirements
- [BepInEx](https://www.nexusmods.com/subnautica/mods/1108)
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
- [qqqbbb](https://github.com/qqqbbb) for [Tweaks and Fixes](https://github.com/qqqbbb/Tweaks-and-Fixes).
- [MrOshaw](https://github.com/mroshaw) for the [Subnautica Modding Guide](https://mroshaw.github.io/).
- The [Subnautica Modding](https://discord.com/invite/subnautica-modding-324207629784186882) Discord server for their help and resources, especially EldritchCarMaker and Essence.