### <span style="color: green;">Done</span>
- **Scanner charge indicator**: Adds the missing charge level when using the Scanner. (Improved by keeping the self-scan hint and better compatibility by not using bool Prefix.)
- **Leviathans don't attack land targets**: Stops leviathans from trying to attack targets on land. (Improved by using GetOceanLevel instead of 1f for compatibility with mods that change water level.)
- **No Flashlight point lights**: Stops the flashlight from lighting the environment around it.
- **No plant waving indoors**: Removes the waving animation from indoor plants. (T&F applied this globally instead.)
- **Nuclear waste disposal name**: Corrected the Nuclear waste disposal bin's name and added the missing space to "Trashcan".
- **Add missing Bulb Bush data entries**: Allows scanning the pygmy and large bulb bush to get the Bulb Bush data entry.
- **No prompt on cut doors**: Removes the laser cutting prompt from already cut doors.
- **Beacon faces the player**: Makes the beacon face the player when deployed.
- **No redundant climb prompt**: Removes the prompt to climb the Mobile Vehicle Bay while already standing on it. (Improved by using GetOceanLevel instead of 1f for compatibility with mods that change water level.)
- **Deadly Cyclops explosion**: Stops the player from respawning inside the Cyclops after its destruction.
- **Smooth Cyclops wheel**: Makes the Cyclops' wheel movement smooth when using a controller.
- **Silent Running no idle power drain**: Stops Silent Running from draining power when the engine is off. (Better compatibility by not using bool Prefix.)
- **Seamoth storage modules gap**: Fixes the gap between the Seamoth and its lower storage modules.
- **Docking bay sound checks**: Stops the Cyclops docking bay sounds from playing if it's already occupied. (Better compatibility by not using bool Prefix.)
- **Thermoblade dynamic particles**: Applies the correct particle effects from the Thermoblade depending on whether the player is underwater or not.
- **Frozen Gas Pods in stasis**: Prevents Gas Pods from exploding while in stasis.
#### Persistence Fixes
**Note**: I'm not sure if T&F does this but this supports multiple Cyclopses by differentiating them by ID and properly cleans data if it is destroyed.
- **Save open wreck doors.**
- **Save closed Cyclops doors.**
- **Save Cyclops speed mode.**
- **Save Cyclops internal lights.**
- **Save Cyclops floodlights**. (Cyclopses are created with the floodlight turned on, that won't be saved until you toggle it manually.)

---

### <span style="color: orange;">To-Do</span>
- **Cyclops HUD needs power**.
- **Reapers can attack Cyclops**: Allows Reaper leviathans to attack the Cyclops instead of just pushing it.
- **Destructable Drooping Stingers**: Allows destroying Drooping Stingers with a knife.
- **No low speed splat**: Removes the fish collision sound when hitting objects at low speed with a Seamoth.
- **Add missing PRAWN sounds**: Adds collision sounds to the PRAWN Suit.
- **No doubled knife particle**: Removes the 2nd particle from knife attacks on creatures.
- **Universal creature decoy**: Makes the creature decoy work on all predators.
- **Dynamic Creepvine light**: Updates the Creepvine seed light according to the amount of remaining seeds.
- **Add missing cannon items**: Allows moving the Sulfur Plant, Brain Coral, Planter, Tiger Plant, Lantern Fruit, and Bulbo Tree with the propulsion or repulsion cannon.
- **Keep drill sounds**: Prevents the drill sounds from stopping when nothing is being drilled.
- **Restore health after poison**: Allows health to be restored after reloading the game while poison was active.
- **Limited PRAWN landing particles**: Prevents PRAWN Suit landings from spawning particles if not on sand.
- **Matching Bulbo Tree LOD**.
- **Add missing VFXSurface**: All plants now have the proper VFXSurface component.
- **Sulfur Plant rotation**: Fixes the rotation of the Sulfur Plant at coordinates 280 -40 -195.
- **Keep dead raw fish stored.**
- **Save Seaglide light.**
- **Save Seaglide holomap.**
- **Save PRAWN Suit light.**
- **Save last held tool.**

---

### <span style="color: red;">Won't Do</span>
#### Can't reproduce the bug
- **No boulder despawn**: Boulders that block some cave entrances in the safe shallows now do not despawn when you move away from them.
- **Land_tree_01 less light.**
- **No dead fish shuffling**: Dead fish in your inventory now stay in the same position on reload.
- **Restore missing drill particles**: Fixes the missing particles when drilling resources.
#### Vanilla feature
- **AC eggs cleanup**: Eggs in your alien containment (AC) now disappear when they hatch.
- **Sink undeployed MVB.**
- **Stalkers drop items when attacked.**
#### Balance implications
- **Wild Lantern Tree fruits respawn.**
- **Wild Blood Oil respawn.**
- **Thermoblade can damage Lava Lizards.**
#### Other reasons
- **Always close hatch flaps**: The Cyclops hatch flaps now always close when you enter it. (Inconsequential, you can't see a closed flap from inside.)
- **Seaglide no visible neck**: Fixed the visible neck when using the Seaglide with a high FOV. (Very complex, not worth the effort.)
- **No creature attacks in stasis**. ([Use Stasis Rifle Freeze Fix](https://www.nexusmods.com/subnautica/mods/1255).)