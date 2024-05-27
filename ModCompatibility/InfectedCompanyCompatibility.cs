using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace InsanityDisplay.ModCompatibility
{
    public class InfectedCompanyCompatibility
    {
        public static Slider modInsanitySlider = null;
        public static IEnumerator ConvertInsanity()
        {
            yield return new WaitUntil(() => RoundManager.Instance.currentLevel.spawnEnemiesAndScrap); //This is when InfectedCompany starts selecting infected players 
            yield return new WaitForSeconds(5f); //The mod waits this long before selecting infected players
            if (!HUDManager.Instance.globalNotificationText.text.Contains("You are infected")) { yield break; } //if not found then player is not infected

            while (modInsanitySlider == null) //forever (bad) loop to find it
            {
                GameObject objectInsanityMeter = GameObject.Find("UI_CustomPlayerHUD(Clone)/InfectedPlayerHUD/InsanityMeter").gameObject;
                objectInsanityMeter?.SetActive(false); //to hide it in case it's visible before being ready
                objectInsanityMeter?.TryGetComponent<Slider>(out modInsanitySlider); //don't immediately set to modInsanitySlider to avoid the meter being visible when it shouldn't be yet
                GameObject objectFill = objectInsanityMeter?.transform.Find("Fill Area/Fill").gameObject;
                if (modInsanitySlider != null && objectFill?.transform.localPosition != Vector3.zero) { yield break; } //found infectedcompany's insanity meter when in use and ready

                yield return null; //Wait one frame
                continue;
            }
        }

    }
}