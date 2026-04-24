- **Cyclops power percentage clamp**: Fixes an underflow (very low negative number) when no cells are inserted.
- **Force engine shutdown**: Automatically shuts down the Cyclops engine when the power is off.
- **Delay Aurora reply**: Prevents the Aurora reply from being received before sending the distress signal.
- **Oxygen Pipes across sub-biomes**: Allows Oxygen Pipes to be placed across sub-biomes such as caves or plateaus.
- **Disable ghost helm buttons**: Disables the invisible (but still clickable) Cyclops helm HUD buttons when the HUD is
  off.
- **Fire extinguisher fuel rounding**: Makes the Fire Extinguisher's fuel percentage display round to the nearest
  integer instead of flooring, allowing it to reach 0% instead of stopping at 1%.
- **Disable dead telemetry**: Stops connection attempts to the inactive analytics
  server (https://analytics.unknownworlds.com/api).
- **Air Bladder requires power**: The Air Bladder will only refill inside habitats with active power.
- **Reset camera on death**: Resets the camera angle when the player dies.