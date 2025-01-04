# From T&F
### Done
- A beached Seamoth can now be pushed.
- Disables eating and using medkits underwater.
- Swivel chairs now can swivel. (Improved by adding acceleration, deceleration, and inertia. Better compatibility by not using bool Prefix.)
- Breaking outcrops requires a tool.
- Newly crafted power cells now match the charge of the batteries used to craft them. (Improved by supporting any number of batteries instead of hardcoding to 2.)
- Removed the epilepsy warning during startup.
- The rotation of plants in planters is now randomized.

#### Minor Tweaks
- The bladderfish description now mentions it being a source of oxygen.

#### Multipliers
- Build time.
- Craft time.
- Plant growth time.

### To-Do
- Reefbacks no longer surface.
- In the main menu, you can press the `escape` key to close the options menu.
- In the main menu, you can use the mouse wheel to select the next or previous savegame slot.
- You can now open the top hatch of your life pod to let the smoke out.
- You can now use both the repulsion and propulsion cannons to move the mobile vehicle bay.
- Gaspods attracted by the grav trap will not explode.
- Reduced the mass of spadefish and shuttlebugs so they do not damage your Seamoth when colliding with it.
- Added a key bind to transfer all items from a container.
- Added a key bind to transfer all items of the same type from a container.
- When you get a blueprint from a databox, the light around it will be removed.
- You can now toggle the exosuit light.
- You can now toggle base lights.
- Voice notifications no longer play during loading.
- When you open a container that allows only certain types of items, you can now easily see those items in your inventory.
- Locker doors are now animated.
- When piloting the Seamoth or Prawn Suit, you can now change the selected torpedo.
- Overheated Cyclops engines can catch fire anytime, not only when you are piloting. You have to turn the engine off to avoid catching fire.
- Lava geysers now cook dead fish.
- The Prawn Suit can now open supply chests.
- Improved the code that deals damage when your vehicles collide with things.
- Lava geysers now push objects up.
- Tooltips for batteries and power cells now show their description.
- Cooked rotten fish now has no food value.
- The creature decoy can now be launched from the Seamoth and Prawn Suit.
- The mobile vehicle bay now has a beacon.
- Removed collision from veined nettle, writhing weed, and some small unnamed plants.
- All plants now have the proper VFXSurface component.
- When grabbing and holding table coral with the propulsion cannon, you can now put it in your inventory.
- The propulsion cannon now breaks outcrops when grabbed.
- Removed the rain particle effect on damaged Seamoths.
- The propulsion cannon can now grab fruits on plants.
- Base leak notifications are sent only if you're in that base.
- Removed collision sounds from brain coral bubbles and small fish hitting the Cyclops.
- Fiber mesh can now be unlocked by scanning Creepvine.
- The precursor terminal does not prompt you to use it after you've used it.
- Simplified the mobile vehicle bay UI.
- Things you harvest with a knife will spawn if your inventory is full.
- Stalactites, mushrooms, and plants in the Jellyshroom Cave no longer pop in if graphics quality is set to high.
- Drillable resources no longer pop in if graphics detail is set to high.
- You can now light a flare in your hand without throwing it.

### Won't Add
- Most of what's under T&F's "Settings in the mod's options menu" section on [Nexus Mods](https://www.nexusmods.com/subnautica/mods/722).
- The coffee vending machine now spawns coffee properly. (Can't reproduce.)
- Added a key bind to select the next or previous PDA tab. (Not interested.)
- Added a key bind to cycle tools in your current quickslot. (Not interested.)
- Brain coral now spawns one bubble instead of three, but it spawns them three times more often. (Not interested.)
- The first installed hull reinforcement module now reduces incoming damage by 30%, the second by 20%, and the third by 10%. (Not interested.)
- When your Seamoth or Prawn Suit gets destroyed, items stored in it will drop. (Not interested.)
- New storage UI system: you no longer have to target a certain part of a container to rename it. (Not interested and the original implementation has a bug that resets the name.)
- You can now name your lockers, Cyclops lockers, and lockers from the Decorations mod. (Not interested.)
- Creepvine seed clusters can now be eaten. (Not interested.)
- You can now push your life pod with the Seamoth or Cyclops. (Not interested.)
- Reefbacks now avoid your life pod. (Not needed if they don't surface.)
- Removed the delay when opening or closing the PDA. (Can't reproduce.)
- The drooping stinger now does not collide with objects. (Not interested.)

---

# New
### Done
- The size of the following creatures is now randomized between configurable multipliers: (I didn't like how [Random Creature Size](https://www.nexusmods.com/subnautica/mods/138) affects all creatures.)
  - Cave Crawler,
  - Lava Larva,
  - Bleeder,
  - Rockgrub,
  - Blighter,
  - Floater.
- Harvesting plants now requires a knife or Thermoblade.
- Removed the check for obstacles when sitting.


#### Minor Tweaks
- "Use Trashcan" -> "Use Trash can" for consistency with its recipe.
- Capitalized the "Use" string globally.