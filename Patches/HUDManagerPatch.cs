using HarmonyLib;
using InsanityDisplay.Config;
using InsanityDisplay.ModCompatibility;
using UnityEngine;
using UnityEngine.UI;
using static InsanityDisplay.UI.UIHandler;

namespace InsanityDisplay.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    public class HUDManageratch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void CreateMeter()
        {
            CreateInMemory(); //This will create in memory AND also in scene afterwards
            return;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void SetMeterValues()
        {
            if (InsanityImage == null) { InsanityImage = InsanityMeter?.GetComponent<Image>(); }
            if (InsanityImage == null || InsanityMeter == null) { return; } //In case something goes wrong

            if (InsanityMeter.activeSelf != ConfigSettings.ModEnabled.Value)
            {
                InsanityMeter.SetActive(ConfigSettings.ModEnabled.Value);
            }

            if (CompatibilityList.ModInstalled.InfectedCompany && InfectedCompanyCompatibility.modInsanitySlider != null) //if mod is found 
            {
                bool setToActive;
                GameObject infectedCompanySlider = InfectedCompanyCompatibility.modInsanitySlider.gameObject;
                if (!ConfigSettings.ModEnabled.Value || ConfigSettings.alwaysFull.Value || !ConfigSettings.Compat.InfectedCompany.Value) //if compat is disabled, meter is disabled, or meter always full
                {
                    setToActive = true;
                }
                else
                {
                    setToActive = false;
                }

                if (infectedCompanySlider.activeSelf != setToActive)
                {
                    infectedCompanySlider.SetActive(setToActive);
                }
            }
            UpdateMeter(imageMeter: InsanityImage, textMeter: EladsHUDCompatibility.InsanityInfo); //elad's will be null if not present
        }
    }
}