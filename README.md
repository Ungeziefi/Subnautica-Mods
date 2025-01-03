This project is aimed at porting the majority of features from [Tweaks and Fixes](https://github.com/qqqbbb/Tweaks-and-Fixes) by [qqqbbb](https://github.com/qqqbbb) to a **clean and optimized codebase** that is easier to maintain or contribute to. It is also **more compatible**, largely thanks to avoiding `bool Prefix` patches that skip the original code and by making **every feature optional**.

You can find more information on what this mod currently does and how it differs from T&F in [Tweaks.md](https://github.com/Ungeziefi/Subnautica-Mods/blob/main/Tweaks/Tweaks%20Progress.md) and [Fixes.md](https://github.com/Ungeziefi/Subnautica-Mods/blob/main/Fixes/Fixes%20Progress.md), including notes about features that should not be included.

Built with [Subnautica.Templates](https://www.nuget.org/packages/Subnautica.Templates) and [Visual Studio 2022 Community Edition](https://visualstudio.microsoft.com/vs/community/).

**Disclaimer**: Any of the code ported from T&F was reviewed and improved. User-facing differences worth noting are mentioned in the 2 files linked above.

---

### Requirements
- [BepInEx](https://www.nexusmods.com/subnautica/mods/1108)
- [Nautilus](https://www.nexusmods.com/subnautica/mods/1262)

---

### Notes
- Features involving text are only enabled when the game is in English. Feel free to PR localization improvements.
- All features are enabled by default and multipliers are set to 1.

---

### Contact
@ungeziefi on Discord, you can find me in the [Subnautica Modding](https://discord.com/invite/subnautica-modding-324207629784186882) server.

---

  ### Credits
- [qqqbbb](https://github.com/qqqbbb) for [Tweaks and Fixes](https://github.com/qqqbbb/Tweaks-and-Fixes).
- [MrOshaw](https://github.com/mroshaw) for the [Subnautica Modding Guide](https://mroshaw.github.io/).
- The [Subnautica Modding](https://discord.com/invite/subnautica-modding-324207629784186882) Discord server for their help and resources, especially EldritchCarMaker and Essence.