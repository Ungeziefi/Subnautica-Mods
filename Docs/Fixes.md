# From T&F
### Done
- The scanner tool now shows the charge level when equipped. (Improved by keeping the self-scan hint and better compatibility by not using bool Prefix.)
- Leviathans now don't attack targets on land. (Improved by using GetOceanLevel instead of 1f for compatibility with mods that change water level.)
- The flashlight now doesn't use point lighting, preventing it from illuminating things behind.
- The Grub basket, Bulbo tree, speckled rattler, pink cap, and Ming plant now do not animate. (Improved by applying this only indoors.)
- The Nuclear waste disposal bin is now called exactly that, not "trash can".
- You can now scan pygmy and large bulb bushes.
- The laser cutting prompt now only shows for doors that are still sealed.
- The beacon now faces the player when deployed.
- Now you can't climb the Mobile Vehicle Bay when standing on it. (Improved by using GetOceanLevel instead of 1f for compatibility with mods that change water level.)
- The player now actually dies when the Cyclops is destroyed.
- The Cyclops' steering wheel now uses correct analog values with a controller.
- Silent running now does not consume power when the engine is off. (Better compatibility by not using bool Prefix.)
- Fixed the gap between the Seamoth and its lower storage modules.
- The Cyclops docking bay sounds now don't play if the bay is already occupied. (Better compatibility by not using bool Prefix.)
- The Thermoblade now only emits smoke particles when not in the water.

#### Persistence Fixes
I'm not sure if the original implementation did this but mine supports multiple Cyclopses by differentiating them by ID and properly cleans data if destroyed.

- Open wreck doors are now saved.
- Open Cyclops doors are now saved.
- The Cyclops' speed mode is now saved.
- The state of both Cyclops lights is now saved. (Cyclopses are created with the floodlight turned on, that won't be saved until you toggle it manually.)

### To-Do
- When the Cyclops is unpowered, its HUD and screens are off.
- The Reaper pushes your Cyclops instead of attacking it.
- Creatures always flee to the origin coordinates when attacked.
- You can now destroy drooping stingers with a knife.
- When your Seamoth hits any object at a very low speed, a "fish splat" sound plays.
- The prawn suit has no collision sounds.
- When you start drilling in your prawn suit, the drilling particles are not shown.
- Stalkers now drop whatever they are holding in their jaws when attacked.
- When you damage a creature with a knife, two instances of the damage particle effect spawn.
- The creature decoy works only for Leviathans. Now it works for every predator.
- The Creepvine seed light updates according to the remaining seeds.
- The brain coral, planter, tiger plant, lantern fruit tree, and Bulbo tree can't be moved with the propulsion or repulsion cannon.
- The drill arm sound effect stops when it is working but not hitting anything.
- When you save the game while taking poison damage, your health does not restore after reloading.
- When the prawn suit jumps from or lands on anything, you get particle effects as if it was sand.
- After reloading, the first-person model is used for waterproof lockers that are not in the inventory.
- The Sulfur Plant can't be moved with the propulsion or repulsion cannon.
- The Bulbo tree LOD meshes look different.
- Fixed the coffee-drinking sound.
- Fixed unused lava geyser particles spawning on every game load.
- The Gasopod in a stasis field does not attack you.
- Gasopods in a stasis field do not explode.
- Resources from harvesting nodes don't fade in when you break nodes.
- Fish you release from your hand don't fade in.
- Items that you place in your base don't fade in.
- Dead fish are removed from containers when loading a saved game.
- The state of Seaglide and Prawn Suit lights is now saved.
- The state of the Seaglide holomap is now saved.
- The tool you are holding in your hand is now saved.

### Won't Add
- When looking at a creature, the UI now tells you if it's dead. (Out of scope.)
- Eggs in your alien containment (AC) now disappear when they hatch. (Can't reproduce.)
- Wild lantern tree fruits now respawn. (Balance change.)
- Wild blood oil now respawns. (Balance change.)
- Corrected the rotation of the Sulfur Plant at coordinates (280, -40, -195). (Can't get it to spawn.)
- Boulders that block some cave entrances in the safe shallows now do not despawn when you move away from them. (Can't reproduce.)
- Removed the excessive light from Land_tree_01. (Can't reproduce.)
- The Mobile Vehicle Bay now sinks when not deployed. (Vanilla feature.)
- Peepers close their eyes when near the player. (Out of scope.)
- Dead fish in your inventory now stay in the same position on reload. (Can't reproduce.)
- The Thermoblade can now damage Lava Lizards. (Balance change.)
- The Cyclops hatch flaps now always close when you enter it. (Inconsequential, you can't see a closed flap from inside.)
- Fixed the visible neck when using the Seaglide with a high FOV. (Very complex, not worth the effort.)
- Fixed the Databank entry scroll. (Can't reproduce, what is this even referring to?)

---

# New
### Done
- Anisotropic filtering is now forced on. (I know [Anisotropic Fix](https://www.nexusmods.com/subnautica/mods/185) exists but I remade it for an in-game toggle.)
- The Cyclops power percentage is now clamped between 0 and 100 to fix an underflow when no cells are inserted.