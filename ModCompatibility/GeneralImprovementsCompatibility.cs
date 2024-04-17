using DunGen;
using System.Collections;
using UnityEngine;

namespace InsanityDisplay.ModCompatibility
{
    public class GeneralImprovementsCompatibility
    {
        private static Vector3 localPositionOffset = new Vector3(-2f, 28f, 0);
        private const int maxAttempts = 500; //Amount of frames it will retry (until it is found)
        private static GameObject HitpointDisplay;

        public static void MoveHPHud()
        {
            HitpointDisplay = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner/HPUI")?.gameObject;

            if (HitpointDisplay == null) { CoroutineHelper.Start(RetryUntilFound()); return; }

            HitpointDisplay.transform.localPosition += localPositionOffset;
        }

        private static IEnumerator RetryUntilFound() //with a max limit of maxAttempts
        {
            for (int i = 0; i < maxAttempts; i++)
            {

                HitpointDisplay = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner/HPUI")?.gameObject;
                if (HitpointDisplay == null)
                {
                    yield return null; //Wait one frame
                    continue;
                }
                HitpointDisplay.transform.localPosition += localPositionOffset;
                break;
            }
            if (HitpointDisplay == null)
            {
                Initialise.modLogger.LogError("GeneralImprovements' health display wasn't found");
            }

        }
    }
}