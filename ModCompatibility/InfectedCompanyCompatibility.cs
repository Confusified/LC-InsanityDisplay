using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LC_InsanityDisplay.ModCompatibility
{
    public class InfectedCompanyCompatibility
    {

        private static void Initialize()
        {
            //Example
            //On.StartOfRound.Start
        }


        public static Slider modInsanitySlider = null;
        public static IEnumerator ConvertInsanity()
        {
            yield return new WaitUntil(() => RoundManager.Instance.currentLevel.spawnEnemiesAndScrap); //This is when InfectedCompany starts selecting infected players 
            yield return new WaitForSeconds(5f); //The mod waits this long before selecting infected players

            if (!HUDManager.Instance.globalNotificationText.text.Contains("You are infected")) { yield break; } //if not found then player is not infected

            while (modInsanitySlider == null) //forever (bad) loop to find it
            {
                GameObject objectInsanityMeter = GameObject.Find("UI_CustomPlayerHUD(Clone)/InfectedPlayerHUD/InsanityMeter").gameObject;
                objectInsanityMeter?.TryGetComponent(out modInsanitySlider);
                if (modInsanitySlider != null)
                {
                    Initialise.Logger.LogDebug("found"); yield break;
                } //found infectedcompany's insanity meter when in use and ready

                yield return null; //Wait one frame
            }
        }

    }
}