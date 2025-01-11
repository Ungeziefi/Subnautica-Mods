### <span style="color: green;">Done</span>
- **Scanner charge indicator**: Adds the missing charge level when using the Scanner. (Improved by keeping the self-scan hint and better compatibility by not using bool Prefix.)
- **Leviathans don't attack land targets**: Stops leviathans from trying to attack targets on land. (Improved by using GetOceanLevel instead of 1f for compatibility with mods that change water level.)
- **No Flashlight point lights**: Stops the flashlight from lighting the environment around it.
- **No plant waving indoors**: Removes the waving animation from indoor plants. (T&F applied this globally instead.)
- **Nuclear waste disposal name**: Corrected the Nuclear waste disposal bin's name and added the missing space to "Trashcan".
- **Add missing Bulb Bush data entries**: Allows scanning any type of Bulb Bush to unlock its data entry.
- **No prompt on cut doors**: Removes the laser cutting prompt from already cut doors.
- **Beacon faces the player**: Makes the beacon face the player when deployed.
- **No MVB climb on top**: Removes the prompt to climb the Mobile Vehicle Bay while already standing on it. (Improved by using GetOceanLevel instead of 1f for compatibility with mods that change water level.)
- **Deadly Cyclops explosion**: Stops the player from respawning inside the Cyclops after its destruction.
- **Smooth Cyclops wheel**: Makes the Cyclops' wheel movement smooth when using a controller.
- **Silent Running no idle power drain**: Stops Silent Running from draining power when the engine is off. (Better compatibility by not using bool Prefix.)
- **Seamoth storage modules gap**: Removes the gap between the Seamoth and its lower storage modules.
- **Docking bay sound checks**: Stops the Cyclops docking bay sounds from playing if it's already occupied. (Better compatibility by not using bool Prefix.)
- **Thermoblade dynamic particles**: Applies the correct particle effects from the Thermoblade depending on whether the player is underwater or not.
- **Frozen Gas Pods in stasis**: Prevents Gas Pods from exploding while in stasis.
- **Coffee drinking sound**: Changes the consume sound of coffee from eating to drinking.
- **Drill sound with no target**: Prevents the drill sounds from stopping when nothing is being drilled. (Better compatibility by not using bool Prefix.)
- **Keep drill particles on load**: Fixes the missing drilling particles when drilling directly after loading and while the drill was already pointed at the deposit. (Better compatibility by not using bool Prefix.)
- **No used Data Box light**. (Improved by picking the closest light instead of the first light it finds within range.)
- **No Seamoth drip particles**.
- **No used terminal prompt**: Data terminals don't show the download prompt after downloading. (Better compatibility by not using bool Prefix.)
- **No fleeing to origin**: Makes fleeing use the direction vector by adding it to the creature's current position, with configurable damage/distance ratio and ceiling (which fixes excessive fleeing distances).
  - To be more specific, the old behaviour was `destination = WhereIAm - WhereDamageCameFrom` and the new one is `destination = WhereIAm + normalized(WhereIAm - WhereDamageCameFrom) * fleeDistance`. Credits to [Mikjaw](https://next.nexusmods.com/profile/Mikjaw) for the pointer on why this happens.
- **Sulfur Plant rotation**: Fixes the rotation of 2 Sulfur Plants (`280 -40 -195` and `272 -41 -199`).
- **Reset Databank scroll**: Makes Databank entries always start at the top when opened instead of keeping the previous scroll position.
- **Cyclops HUD needs power**. (Improved by covering all the missing interfaces and light sources. The full list is: Sonar, compass, decoy screen, storage terminal, upgrade console, lights control panel, edit screen, and light statics.)
#### Persistence Fixes
**Note**: I'm not sure if T&F does this but my implementation supports multiple Cyclopses by differentiating them by ID. It also cleans up the data when a Cyclops is destroyed.
- **Save open wreck doors**.
- **Save closed Cyclops doors**.
- **Save Cyclops speed mode**.
- **Save Cyclops internal lights**.
- **Save Cyclops floodlights**.
- **Save Seaglide toggles**.

---

### <span style="color: orange;">To-Do</span>
- **Reapers can attack Cyclops**: Allows Reaper leviathans to attack the Cyclops instead of just pushing it.
- **Destructable Drooping Stingers**: Allows destroying Drooping Stingers with a knife.
- **No low speed splat**: Removes the fish collision sound when hitting objects at low speed with a Seamoth.
- **Add missing PRAWN sounds**: Adds collision sounds to the PRAWN Suit.
- **No doubled knife particle**: Removes the 2nd particle from knife attacks on creatures.
- **Dynamic Creepvine light**: Updates the Creepvine seed light according to the amount of remaining seeds.
- **Add missing cannon items**: Allows moving the Sulfur Plant, Brain Coral, Planter, Tiger Plant, Lantern Fruit, and Bulbo Tree with the propulsion or repulsion cannon.
- **Restore health after poison**: Allows health to be restored after reloading the game while poison was active.
- **Limited PRAWN landing particles**: Prevents PRAWN Suit landings from spawning particles if not on sand.
- **Matching Bulbo Tree LOD**: Makes the Bulbo Tree LOD model match its high quality model.
- **Add missing VFXSurface**: All plants now have the proper VFXSurface component.
- **Keep dead raw fish stored**.
- **Save PRAWN Suit light**.
- **Save last held tool**.

---

### <span style="color: red;">Won't Do</span>
#### Can't reproduce the bug
- **No boulder despawn**: Boulders that block some cave entrances in the safe shallows now do not despawn when you move away from them.
- **No dead fish shuffling**: Dead fish in your inventory now stay in the same position on reload.
- "The coffee vending machine now spawns coffee properly". (Possibly fixed with Living Large.)
- "After reloading, the first-person model is used for waterproof lockers that are not in the inventory."
#### Vanilla feature
- **AC eggs cleanup**: Eggs in your alien containment (AC) now disappear when they hatch.
- **Sink undeployed MVB**.
- **Stalkers drop items when attacked**.
#### Balance implications
- **Wild Lantern Tree fruits respawn**.
- **Wild Blood Oil respawn**.
- **Thermoblade can damage Lava Lizards**.
#### Other reasons
- **Always close hatch flaps**: The Cyclops hatch flaps now always close when you enter it. (Inconsequential, you can't see a closed flap from inside.)
- **Seaglide no visible neck**: Fixed the visible neck when using the Seaglide with a high FOV. (Very complex, not worth the effort.)
- **No creature attacks in stasis**. ([Stasis Rifle Freeze Fix](https://www.nexusmods.com/subnautica/mods/1255) already does it.)