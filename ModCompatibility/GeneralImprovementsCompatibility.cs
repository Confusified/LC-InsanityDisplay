using DunGen;
using InsanityDisplay.Config;
using System.Collections;
using UnityEngine;
using static InsanityDisplay.UI.UIHandler;

namespace InsanityDisplay.ModCompatibility
{
    public class GeneralImprovementsCompatibility
    {
        private static Vector3 localPositionOffset = new Vector3(-2f, 28f, 0);
        private const int maxAttempts = 600; //Amount of frames it will retry (until it is found)
        private static GameObject HitpointDisplay;

        public static void MoveHPHud()
        {

            HitpointDisplay = TopLeftCornerHUD.transform.Find("HPUI")?.gameObject;
            Initialise.modLogger.LogError(HitpointDisplay); //debug purposes
            if (HitpointDisplay == null) { CoroutineHelper.Start(RetryUntilFound()); return; }

            HitpointDisplay.transform.localPosition += localPositionOffset;
        }

        private static IEnumerator RetryUntilFound() //with a max limit of maxAttempts
        {
            for (int i = 0; i < maxAttempts; i++)
            {

                HitpointDisplay = TopLeftCornerHUD.transform.Find("HPUI")?.gameObject;
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