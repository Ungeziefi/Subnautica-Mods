# T&F Ports
### Done
- Adds a push feature to the Seamoth.
- Multiplies the build time of structures.
- Multiplies the craft time of items.
- Disables eating and using medkits underwater.
- Swivel chairs now can swivel. (Improved by adding acceleration, deceleration, and inertia. Also improved by not using bool Prefix.)
- Breaking outcrops requires a tool.
- Newly crafted power cells now match the charge of the batteries used to craft them. (Improved by supporting any number of batteries instead of hardcoding to 2.)
- Removed the epilepsy warning during startup.
- The rotation of plants in planters is now randomized.
**Minor Tweaks**
  - The bladderfish description now mentions it can be used as a source of oxygen.

### To-Do
- Reefbacks no longer surface.
- Cyclops speed mode is now saved.
- In the main menu, you can press the 'escape' key to close the options menu.
- In the main menu, you can use the mouse wheel to select the next or previous savegame slot.
- When your life pod is damaged, you can open the top hatch to let the smoke out.
- The tool you are holding in your hand is now saved when you save your game.
- You can use both the repulsion and propulsion cannons to move the mobile vehicle bay.
- You can push your life pod with the Seamoth or Cyclops.
- Gaspods attracted by the grav trap do not explode.
- Reduced the mass of spadefish and shuttlebugs so they do not damage your Seamoth when colliding with it.
- Added a key bind to transfer all items from a container.
- Added a key bind to transfer all items of the same type from a container.
- Reefbacks now avoid your life pod.
- When you get a blueprint from a databox, the light around it will be removed.
- You can now toggle the exosuit light.
- Voice notifications no longer play during loading.
- You can now toggle base lights.
- When you open a container that allows only certain types of items, you can easily see those items in your inventory.
- Locker doors now open and close.
- When piloting the Seamoth or Prawn Suit, you can change the currently selected torpedo.
- Removed the delay when opening or closing the PDA.
- Overheated Cyclops engines can catch fire anytime, not only when you are piloting. You have to turn the engine off to avoid catching fire.
- Lava geysers now cook dead fish.
- The Prawn Suit can now open supply chests.
- Improved the script that deals damage when your vehicles collide with things.
- Lava geysers now push objects up.
- The state of Seaglide, Prawn Suit, and Cyclops lights is now saved.
- The state of the Seaglide holomap is now saved.
- The Cyclops lighting state is now saved.
- Tooltips for batteries and power cells now show their description.
- Cooked rotten fish now has no food value.
- The creature decoy can now be launched from the Seamoth and Prawn Suit.
- The mobile vehicle bay now has a beacon.
- Removed collision from veined nettle, writhing weed, and some small unnamed plants.
- All plants now have the proper VFXSurface component.
- When grabbing and holding table coral with the propulsion cannon, you can put it in your inventory.
- Your propulsion cannon will break an outcrop when you try to grab it.
- Removed the unnecessary rain particle effect on damaged Seamoth.
- The propulsion cannon can grab fruits on plants.
- You get messages about leaks in your base only when you are inside that base.
- You don't hear brain coral bubbles or small fish colliding with the Cyclops.
- Fiber mesh can now be unlocked by scanning creepvine.
- The drooping stinger now does not collide with objects.
- The precursor terminal does not prompt you to use it after you've used it.
- Simplified the mobile vehicle bay UI.
- Things you harvest with a knife will spawn if your inventory is full.
- Stalactites, mushrooms, and plants in the Jellyshroom Cave no longer pop in if graphics detail is set to high.
- Drillable resources no longer pop in if graphics detail is set to high.
- You can light a flare in your hand without throwing it.

### Won't Add
- The coffee vending machine now spawns coffee properly. (Can't reproduce.)
- Added a key bind to select the next or previous PDA tab. (Not interested.)
- Added a key bind to cycle tools in your current quickslot. (Not interested.)
- Brain coral now spawns one bubble instead of three, but it spawns them three times more often. (Not interested.)
- The first installed hull reinforcement module now reduces incoming damage by 30%, the second by 20%, and the third by 10%. (Not interested.)
- When your Seamoth or Prawn Suit gets destroyed, items stored in it will drop. (Not interested.)
- New storage UI system: you no longer have to target a certain part of a container to rename it. (Not interested and the original implementation has a bug that resets the name.)
- You can now name your lockers, Cyclops lockers, and lockers from the Decorations mod. (Not interested.)
- Creepvine seed clusters can now be eaten. (Not interested.)


---

# New
### Done
- Randomizes the size of certain creatures. (I didn't like how [Random Creature Size](https://www.nexusmods.com/subnautica/mods/138) affects all creatures. Check Tweaks.cs#L81 for a list of them.)
- Harvesting plants requires a knife or Thermoblade.
- Removes the check for obstacles when sitting to avoid getting locked out of the swivel chair when you spin it too close to an object then stand up.
**Minor Tweaks**
  - "Use Trashcan" -> "Use Trash can" for consistency with its recipe.
  - Capitalized the "Use" string globally.