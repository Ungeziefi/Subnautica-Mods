### <span style="color: green;">Done</span>
- **Scanner charge indicator**: Adds the missing charge level when using the Scanner. (Improved by keeping the self-scan hint and better compatibility by not using bool Prefix.)
- **Leviathans don't attack land targets**: Stops Leviathans from trying to attack targets on land. (Improved by using GetOceanLevel instead of 1f for compatibility with mods that change water level.)
- **No Flashlight point lights**: Stops the Flashlight from lighting the environment around it.
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
- **No used terminal prompt**: Data terminals don't show the download prompt after downloading.
- **No fleeing to origin**: Makes fleeing use the direction vector by adding it to the creature's current position. (Improved by adding configurable damage/distance ratio and ceiling, which fixes excessive fleeing distances.)
  - To be more specific, the old behaviour was `destination = WhereIAm - WhereDamageCameFrom` and the new one is `destination = WhereIAm + normalized(WhereIAm - WhereDamageCameFrom) * fleeDistance`. Credits to [Mikjaw](https://next.nexusmods.com/profile/Mikjaw) for the pointer on why this happens.
- **Sulfur Plant rotation**: Fixes the rotation of 2 Sulfur Plants (`280 -40 -195` and `272 -41 -199`).
- **Reset Databank scroll**: Makes Databank entries always start at the top when opened instead of keeping the previous scroll position.
- **Treaders can attack**: Fixes the Sea Treaders not being able to attack the player due to a `onSurface` check.
- **No Cyclops pushing**: Stops Reaper Leviathans from just pushing the Cyclops instead of attacking it. (Better compatibility by not using bool Prefix.)
- **Matching Bulbo Tree LOD**: Disables the LOD system of `land_plant_middle_01` to hide the mismatched LOD texture. Might affect performance very slightly.
- **Dynamic Creepvine light**: Updates the Creepvine seed light according to the amount of remaining seeds.
- **PRAWN collision sounds**.
- **No voice while loading**.
- **Keep dead raw fish stored**: Prevents stored raw fish from disappearing when loading a save. (Not ideal but, like T&F, it resets the decay timer.)
- **PRAWN particles only on sand**: Prevents PRAWN Suit landings from spawning particles if not on sand. (Better compatibility by not using bool Prefix.)
- **Add missing VFXSurfaces**.
- **No deposit pop in**: Controls how far away resource deposits become visible. (Improved by making the distance configurable.)
- **No Jellyshroom Cave pop in**: Stalactites, mushrooms, and plants in the Jellyshroom Cave no longer pop in. (Improved by making the distance configurable.)
- **No low speed splat**: Removes the fish collision sound when hitting objects at low speed with a Seamoth.
- **Dead Peepers closed eyes**: Fixes the Peepers' eyes in LOD distance from being open when dead.
#### Persistence Fixes
**Note**: I'm not sure if T&F does this but my implementation supports multiple Cyclopses by differentiating them by ID. It also cleans up the data when a Cyclops is destroyed.
- **Save open wreck doors**.
- **Save closed Cyclops doors**.
- **Save Cyclops speed mode**.
- **Save Cyclops internal lights**.
- **Save Cyclops floodlights**.
- **Save last held item**. (Improved by supporting mid-game reloads.)
- 
---

### <span style="color: orange;">To-Do</span>
- **Save Seaglide toggles**.

---

### <span style="color: red;">Won't Do</span>
#### Can't reproduce the bug
- "Boulders that block some cave entrances in safe shallows now do not dissappear forever when you move away from them."
- "Equipped dead fish's position changed if it was in your inventory when the game was loaded."
- "Coffee vending machine now spawns coffee properly." (Probably fixed by Living Large.)
- "After reloading 1st person model was used for waterproof lockers that were not in inventory."
- "When you damaged a creature with knife 2 instances of damage particle effect spawned."
- "Remove safe spot at bottom of geyser." (Undocumented but found in the code.)
- "Light on top of cyclops cabin now works."
#### Vanilla feature
- "Eggs in your AC now disappear when they hatch."
- "Mobile Vehicle Bay now sinks when not deployed."
- "Stalkers now drop whatever they are holding in their jaws when they are attacked."
#### Other reasons
- "Cyclops hatch flaps now always close when you enter cyclops." (Inconsequential, you can't see a closed flap from inside.)
- "You could see your neck when using seaglide with high FOV." (Very complex, not worth the effort.)
- "Gasopod in stasis field does not attack you." ([Stasis Rifle Freeze Fix](https://www.nexusmods.com/subnautica/mods/1255) already does that and more.)
- "Fixed unused lava geyser particles spawning on every game load." (Does cleaning them really help with anything?)
- "When you saved game while taking poison damage, your health would not restore after reload." (Can't figure out a fix.)

---

### <span style="color: grey;">Moved to Tweaks</span>
These were originally in T&F's list of fixes but are more suited as tweaks:
- "You can now destroy drooping stinger with knife." -> "Destructible Drooping Stingers"
- "When looking at a creature, UI now tells you if it's dead." -> Won't Do
- "Creature decoy worked only for leviathans. Now it does for every predator." -> "Universal creature decoy"
- "Brain coral, planter, tiger plant, lantern fruit tree, bulbo tree now can't be moved with propulsion or repulsion cannon." -> Won't Do
- "Now when cyclops is unpowered its HUD and screens will be off." -> "Cyclops displays needs power"
- "Sulfur Plant now can't be moved with propulsion or repulsion cannon." -> Won't Do
- "Resources from harvesting nodes now don't fade in when you break nodes." -> "No resources fading"
- "Fish you release from your hand now does not fade in." -> "No fish release fading"
- "Peepers closed their eyes when near player." -> "Scared Peepers"
- "Items that you place in your base now dont fade in." -> "No base items fading"
- "Wild lantern tree fruits did not respawn." -> Won't Do
- "Wild blood oil did not respawn." -> Won't Do
- "Heat blade now can damage lava lizards." -> Won't Do