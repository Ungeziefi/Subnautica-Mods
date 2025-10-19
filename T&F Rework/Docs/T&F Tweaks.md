### <span style="color: green;">Done</span>
- **Seamoth pushing**: Allows pushing the Seamoth when it's on land.
- **No eating underwater**.
- **No medkits underwater**.
- **Chair swiveling**. (Improved by adding acceleration, deceleration, and inertia**: Better compatibility by not using bool Prefix. Could be improved by splitting the mesh in 2 but I don't do assets.)
- **Outcrops require tool**: Harvesting outcrops requires any tool to be equipped.
- **Power cell charge from batteries**: Sets the charge level of newly crafted Power Cells based on the charge level of the batteries used for crafting**: (Improved by supporting any number of batteries instead of hardcoding to 2.)
- **Skip epilepsy warning**.
- **Plant rotation randomizer**.
- **Mobile Vehicle Bay beacon**.
- **Cyclops displays needs power**: Disables various screens and systems when the power is off. (Improved by covering all the missing interfaces and light sources. The full list is now: Helm HUD, sonar, compass, decoy screen, storage terminal, upgrade console, lights control panel, edit screen, and light statics.)
- **Batteries have tooltips**.
- **No PDA delay**. (Better compatibility by not using bool Prefix.)
- **Destructible Drooping Stingers**.
- **Land_tree_01 light removal**.
- **No Reefback surfacing**: Prevents Reefbacks from swimming to the surface.
- **Moveable Mobile Vehicle Bay**: Allows the repulsion and propulsion cannons to move the Mobile Vehicle Bay.
- **Creepvine unlocks Fiber Mesh**: Scanning Creepvine unlocks the Fiber Mesh.
- **Openable chests in PRAWN**: The PRAWN Suit can now open supply chests.
- **Manual torpedo selection**.
- **Animated locker doors**.
- **PRAWN Suit lights toggle**.
- **PRAWN Suit lights follows camera**.
- **PRAWN Suit arms need power**. (Undocumented but found in the code.)
- **Passive engine overheating**: The Cyclops engine can now overheat even when the throttle is not applied. (Doesn't rework the whole system like T&F does. Better compatibility by not using bool Prefix.)
- **Geysers push objects**.
- **Smoke clears on open**: Opening the top hatch of the life pod clears the smoke inside.
- **Base lights toggle**. (Far simpler than the T&F implementation. Doesn't rely on [Base Light Switch](https://www.nexusmods.com/subnautica/mods/46), but it also doesn't use a physical switch.)
#### Miscellaneous
- **Bladderfish tooltip**: Adds a tooltip about the Bladderfish providing oxygen if consumed raw.
#### Multipliers
- **Build time multiplier**.
- **Craft time multiplier**.
- **Plant growth time multiplier**.
- **Day/night cycle speed multiplier**.

---

### <span style="color: orange;">To-Do</span>
- **Improved collision logic**: Improves the vehicle collision code.
- **No rotten food value**: Removes food value from rotten cooked fish.
- **Torpedo launcher creature decoy**: Allows the Seamoth and PRAWN Suit to launch Creature Decoys.
- **No collision with small plants**: Removes collision from the Veined Nettle, Writhing Weed, and some other unnamed plants.
- **Propulsion Cannon fast transfer**: Holding Table Coral with the Propulsion Cannon allows you to put it in your inventory directly.
- **No remote leaking notifications**: Base leak notifications are sent only if you're in that base.
- **Full inventory harvesting**: Harvesting with a full inventory will drop the items.
- **Universal creature decoy**: Makes the creature decoy work on all predators.
- **Propulsion Cannon can grab fruit**.

---

### <span style="color: red;">Won't Do</span>
#### Not interested
- Most of what's under T&F's "Settings in the mod's options menu" section on [Nexus Mods](https://www.nexusmods.com/subnautica/mods/722).
- "Key bind to select next or previous PDA tab."
- "Quickslot cycle key."
- "Brain coral now spawns 1 bubble instead of 3 but it spawns them 3 times more often."
- "Reduced mass of spadefish and shuttlebug so they do not damage your seamoth when colliding with it."
- "Hull reinforcement module now reduces any physical damage, not just damage from collisions. First installed hull reinforcement module reduces incoming damage by 30%, second one by another 20%, third one by another 10%."
- "When your seamoth or prawn suit gets destroyed, items stored in it will drop."
- "New storage UI system. Now you don't have to target certain part of a container to rename it."
- "Now you can name your lockers, cyclops lockers and lockers from Decorations mod."
- "Creepvine seed cluster can now be eaten."
- "You can push your life pod with seamoth or cyclops."
- "Drooping stinger now does not collide with objects."
- "Your propulsion cannon will break outcrop when you try to grab it."
- "Simplified mobile vehicle bay UI."
- "When looking at a creature, UI now tells you if it's dead."
- "Brain coral, planter, tiger plant, lantern fruit tree, bulbo tree now can't be moved with propulsion or repulsion cannon."
- "Sulfur Plant now can't be moved with propulsion or repulsion cannon."
- "Reduced size of alien containment hatch collision box so you dont get stuck when using ladder next to it."
- "Removed collision box from railing you get in your multipurpose room when you build a hatch or a corridor."
- "Gaspods attracted by grav trap do not explode."
- "Resources from harvesting nodes now don't fade in when you break nodes."
- "Fish you release from your hand now does not fade in."
- "Items that you place in your base now dont fade in."
#### Gameplay balance implications
- "Wild lantern tree fruits did not respawn."
- "Wild blood oil did not respawn."
- "Heat blade now can damage lava lizards."
#### Vanilla feature
- "When in main menu you can press 'escape' key to close options menu."
- "Reefbacks now avoid your life pod." (They already have the AvoidObstacles component, so they should avoid it.)
- "Lava geysers now cook dead fish." (Geyser.OnTriggerStay already applies Fire damage.)
#### Other Reasons
- "You can light flare in your hand without throwing it." ([Flare Repair (BepInEx)](https://www.nexusmods.com/subnautica/mods/452) already does that and more.)
- "When in main menu you can use mouse wheel to select next or previous savegame slot." (Adding it without keyboard support doesn't make sense, you'd still need to click with the mouse.)
- Main menu email box toggle. (Undocumented but found in the code. (You can use [Minimal Main Menu](https://www.nexusmods.com/subnautica/mods/2319) instead.)
- "Key bind to transfer all items from a container." ([Container Utilities](https://www.nexusmods.com/subnautica/mods/x) already does that and more.)
- "Key bind to to transfer all items of the same type from a container." ([Container Utilities](https://www.nexusmods.com/subnautica/mods/x) already does that and more.)
- "Now when you open a container that allows to add only certain types of items, you can easily see those items in your inventory." ([Container Utilities](https://www.nexusmods.com/subnautica/mods/x) already does that and more.)

---

### <span style="color: grey;">Moved to Fixes</span>
These were originally in T&F's list of "other changes" but are actually fixes:
- "No voice while loading." -> "No voice while loading"
- "All plants now have proper VFXSurface component." + Other objects that weren't documented -> "Add missing VFXSurfaces"
- "Coffee vending machine now spawns coffee properly." -> Can't reproduce
- "Precursor terminal does not prompt you to use it after you used it." + Generic consoles -> "No used terminal prompt"
- "Light on top of cyclops cabin now works." -> Can't reproduce
- "You don't hear when brain coral bubbles or small fish collide with cyclops." -> "Soft collision sound minimum mass"
- All persistence-related features:
  - "Save open wreck doors"
  - "Save closed Cyclops doors"
  - "Save Cyclops speed mode"
  - "Save Cyclops internal lights"
  - "Save Cyclops floodlights"
  - "Save last held item"
  - "Save Seaglide toggles"