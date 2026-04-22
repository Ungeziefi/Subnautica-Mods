using System.Collections.Generic;
using UnityEngine;

namespace Ungeziefi.Stasis_Rifle_Freeze_Fix;

public class SRFF_Component : MonoBehaviour
{
    public Creature creature;
    private readonly Dictionary<FMOD_CustomEmitter, bool> emitterStates = new();

    private FMOD_CustomEmitter[] emitters;
    private bool isFrozen;
    private float previousAggression;

    private void Start()
    {
        emitters = GetComponentsInChildren<FMOD_CustomEmitter>(true);
    }

    private void FixedUpdate()
    {
        if (!creature.liveMixin) return;

        if (!creature.liveMixin.IsAlive())
        {
            if (creature.GetAnimator()) creature.GetAnimator().enabled = true;
            isFrozen = false;
        }
    }

    private void LateUpdate()
    {
        if (!isFrozen) return;

        creature.Aggression.Value = 0;
    }

    public void OnFreezeByStasisSphere()
    {
        previousAggression = creature.Aggression.Value;
        creature.Aggression.Value = 0;
        if (creature.GetAnimator() != null) creature.GetAnimator().enabled = false;

        isFrozen = true;

        foreach (var emitter in emitters)
        {
            if (!emitterStates.ContainsKey(emitter)) emitterStates.Add(emitter, false);

            emitterStates[emitter] = emitter.enabled;
            emitter.enabled = false;
        }
    }

    public void OnUnfreezeByStasisSphere()
    {
        isFrozen = false;
        creature.Aggression.Value = previousAggression;
        if (creature.GetAnimator()) creature.GetAnimator().enabled = true;

        creature.UpdateBehaviour(Time.time, Time.deltaTime);

        foreach (var emitter in emitters)
        {
            emitter.enabled = emitterStates[emitter];
            emitterStates.Remove(emitter);
        }
    }

    public bool IsFrozen()
    {
        return isFrozen;
    }
}