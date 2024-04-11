using DunGen;
using System.Collections;
using UnityEngine;

namespace InsanityDisplay.ModCompatibility
{
    public class GeneralImprovementsCompatibility
    {
        private static Vector3 localPositionOffset = new Vector3(0, 20.5f, 0);
        private static readonly int maxAttempts = 500; //Amount of frames it will retry (until it is found)

        public static void MoveHPHud()
        {
            GameObject HitpointDisplay = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner/HPUI")?.gameObject;

            if (HitpointDisplay == null) { CoroutineHelper.Start(RetryUntilFound()); return; }

            HitpointDisplay.transform.localPosition += localPositionOffset;
        }

        private static IEnumerator RetryUntilFound() //with a max limit of maxAttempts
        {
            bool found = false;
            for (int i = 0; i < maxAttempts; i++)
            {
                yield return null; //Wait one frame
                GameObject HitpointDisplay = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner/HPUI")?.gameObject;
                if (HitpointDisplay == null) { continue; }
                HitpointDisplay.transform.localPosition += localPositionOffset;
                found = true;
                break;
            }
            if (!found)
            {
                Initialise.modLogger.LogError("GeneralImprovements' health display wasn't found");
            }

        }
    }
}