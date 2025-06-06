### <span style="color: green;">Done</span>
- **Seamoth pushing**: Allows pushing the Seamoth when it's on land.
- **No eating underwater**.
- **No medkits underwater**.
- **Chair swiveling**. (Improved by adding acceleration, deceleration, and inertia**: Better compatibility by not using bool Prefix. Could be improved by splitting the mesh in 2 but I don't do assets.)
- **Outcrops require tool**: Harvesting outcrops requires any tool to be equipped.
- **Power cell charge from batteries**: Sets the charge level of newly crafted power cells based on the charge level of the batteries used for crafting**: (Improved by supporting any number of batteries instead of hardcoding to 2.)
- **Skip epilepsy warning**.
- **Plant rotation randomizer**.
- **Mobile Vehicle Bay beacon**.
- **Cyclops displays needs power**: Disables various screens and systems when the power is off. (Improved by covering all the missing interfaces and light sources. The full list is now: Helm HUD, sonar, compass, decoy screen, storage terminal, upgrade console, lights control panel, edit screen, and light statics.)
- **Batteries have tooltips**.
- **No PDA delay**. (Better compatibility by not using bool Prefix.)
- **Disable email box**: Disables the email box in the main menu when news are disabled. (Needs a restart to update state.)
- **Destructible Drooping Stingers**.
- **Land_tree_01 light removal**.
- **Transfer all items**: Hold a key to transfer all items between containers at once.
- **Transfer all similar items**: Hold a key to transfer all items of the same type between containers at once.
#### Miscellaneous
- **Bladderfish tooltip**: Adds a tooltip about the Bladderfish providing oxygen if consumed raw.
#### Multipliers
- **Build time multiplier**.
- **Craft time multiplier**.
- **Plant growth time multiplier**.
- **Day/night cycle speed multiplier**.

---

### <span style="color: orange;">To-Do</span>
- **Moveable Mobile Vehicle Bay**: Allows the repulsion and propulsion cannons to move the Mobile Vehicle Bay.
- **Lighter Seamoth collisions**: Reduces the mass of Spadefish and Shuttlebug to prevent them from damaging your Seamoth upon collision.
- **PRAWN Suit light toggle**.
- **Base lights toggle**.
- **Allowed items indicator**.
- **Animated locker doors**.
- **Manual torpedo selection**.
- **Passive engine overheating**: An overheated Cyclops engine can catch fire even outside of piloting.
- **Geysers fish cooking**: Lava geysers cook dead fish.
- **Geysers push objects**.
- **Openable chests in PRAWN**: The PRAWN Suit can now open supply chests.
- **Improved collision logic**: Improves the vehicle collision code.
- **No rotten food value**: Removes food value from rotten cooked fish.
- **Torpedo launcher creature decoy**: Allows the Seamoth and PRAWN Suit to launch Creature Decoys.
- **No collision with small plants**: Removes collision from the Veined Nettle, Writhing Weed, and some other unnamed plants.
- **Propulsion Cannon fast transfer**: Holding Table Coral with the Propulsion Cannon allows you to put it in your inventory directly.
- **No remote leaking notifications**: Base leak notifications are sent only if you're in that base.
- **Less Cyclops collisions**: Removed collision sounds from Brain Coral bubbles and small fish hitting the Cyclops.
- **Creepvine unlocks Fiber Mesh**: Scanning Creepvine unlocks the Fiber Mesh.
- **Full inventory harvesting**: Harvesting with a full inventory will drop the items.
- **No resources fading**: Resources spawned by breaking outcrops now don't fade in.
- **No fish release fading**: Released fish now don't fade in.
- **No base items fading**: Items placed in bases now don't fade in.
- **Universal creature decoy**: Makes the creature decoy work on all predators.
- **Propulsion Cannon can grab fruit**.
- **Smoke clears on open**: Opening the top hatch of the life pod clears the smoke inside.

---

### <span style="color: red;">Won't Do</span>
#### Not interested
- Most of what's under T&F's "Settings in the mod's options menu" section on [Nexus Mods](https://www.nexusmods.com/subnautica/mods/722).
- "Reefbacks don't surface now."
- "Key bind to select next or previous PDA tab."
- "Quickslot cycle key."
- "Brain coral now spawns 1 bubble instead of 3 but it spawns them 3 times more often."
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
- "Prawn suit lights now follow the camera when you look up or down."
- "Gaspods attracted by grav trap do not explode."
#### Gameplay balance implications
- "Wild lantern tree fruits did not respawn."
- "Wild blood oil did not respawn."
- "Heat blade now can damage lava lizards."
#### Vanilla feature
- "When in main menu you can press 'escape' key to close options menu."
#### Other Reasons
- "Reefbacks now avoid your life pod." (No reefback surfacing make this pointless.)
- "You can light flare in your hand without throwing it." ([Flare Repair (BepInEx)](https://www.nexusmods.com/subnautica/mods/452) already does that and more.)
- "When in main menu you can use mouse wheel to select next or previous savegame slot." (Adding it without keyboard support doesn't make sense, you'd still need to click with the mouse.)

---

### <span style="color: grey;">Moved to Fixes</span>
These were originally in T&F's list of "other changes" but are actually fixes:
- "No voice while loading." -> "No voice while loading"
- "All plants now have proper VFXSurface component." + Other objects that weren't documented -> "Add missing VFXSurfaces"
- "Coffee vending machine now spawns coffee properly." -> Can't reproduce
- "Precursor terminal does not prompt you to use it after you used it." -> "No used terminal prompt"
- "Light on top of cyclops cabin now works." -> Can't reproduce
- All persistence-related features:
  - "Save open wreck doors"
  - "Save closed Cyclops doors"
  - "Save Cyclops speed mode"
  - "Save Cyclops internal lights"
  - "Save Cyclops floodlights"
  - "Save last held item"
  - "Save Seaglide toggles"