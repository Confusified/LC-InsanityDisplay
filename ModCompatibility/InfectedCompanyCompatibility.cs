using DunGen;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace InsanityDisplay.ModCompatibility
{
    public class InfectedCompanyCompatibility
    {
        public static Slider modInsanitySlider;
        private const int maxAttempts = 500;
        public static bool isInfected;
        public static void ConvertInsanity()
        {
            GameObject.Find("UI_CustomPlayerHUD(Clone)/InfectedPlayerHUD/InsanityMeter")?.TryGetComponent<Slider>(out modInsanitySlider);
            if (modInsanitySlider == null)
            {
                CoroutineHelper.Start(LocateMeter());
            }
        }

        public static IEnumerator LocateMeter()
        {
            for (int i = 0; i > maxAttempts; i++) //loop forever (bad) until it's found
            {
                GameObject.Find("UI_CustomPlayerHUD(Clone)/InfectedPlayerHUD/InsanityMeter")?.TryGetComponent<Slider>(out modInsanitySlider);
                if (modInsanitySlider == null)
                {
                    yield return null; //Wait one frame
                    continue;
                }
                break;
            }
            if (!modInsanitySlider)
            {
                Initialise.modLogger.LogError("Could not find INSANITY METER!!!!!!!!! or you aren't infected");
            }
        }

    }
}