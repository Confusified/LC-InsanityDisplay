using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace InsanityDisplay.ModCompatibility
{
    public class InfectedCompanyCompatibility
    {
        public static Slider modInsanitySlider;
        private const ushort maxAttempts = 500;
        public static bool isInfected;
        public static IEnumerator ConvertInsanity()
        {
            yield return new WaitForSeconds(5); //The mod waits this long before choosing infected players
            GameObject.Find("UI_CustomPlayerHUD(Clone)/InfectedPlayerHUD/InsanityMeter")?.TryGetComponent<Slider>(out modInsanitySlider);
            if (modInsanitySlider != null) { yield break; }

            for (ushort i = 0; i > maxAttempts; i++) //loop forever (bad) until it's found
            {
                GameObject.Find("UI_CustomPlayerHUD(Clone)/InfectedPlayerHUD/InsanityMeter")?.TryGetComponent<Slider>(out modInsanitySlider);
                if (modInsanitySlider != null) { yield break; }

                yield return null; //Wait one frame
                continue;
            }
            if (!modInsanitySlider)
            {
                Initialise.modLogger.LogError("Could not find INSANITY METER!!!!!!!!! or you aren't infected");
            }
        }

    }
}