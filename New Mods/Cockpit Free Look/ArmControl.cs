using RootMotion.FinalIK;
using System.Collections.Generic;

namespace Ungeziefi.Cockpit_Free_Look
{
    public partial class CockpitFreeLook
    {
        private static void DisableExosuitArms(Exosuit exosuit)
        {
            if (exosuit == null || !Main.Config.LockPRAWNArms) return;

            // Cache AimIK
            if (cachedAimIKComponents == null)
            {
                cachedAimIKComponents = new List<AimIK>();
                cachedAimIKComponents.AddRange(exosuit.GetComponentsInChildren<AimIK>());
            }

            // Disable
            foreach (var aimIK in cachedAimIKComponents)
            {
                if (aimIK != null) aimIK.enabled = false;
            }
        }

        private static void EnableExosuitArms(Exosuit exosuit)
        {
            if (exosuit == null || cachedAimIKComponents == null || !Main.Config.LockPRAWNArms) return;

            // Re-enable
            foreach (var aimIK in cachedAimIKComponents)
            {
                if (aimIK != null) aimIK.enabled = true;
            }
        }
    }
}