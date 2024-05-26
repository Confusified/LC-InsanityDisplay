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

            InsanityMeter.SetActive(ConfigSettings.ModEnabled.Value);
            if (CompatibilityList.ModInstalled.InfectedCompany && InfectedCompanyCompatibility.modInsanitySlider != null) //if mod is found 
            {
                if (!ConfigSettings.ModEnabled.Value || ConfigSettings.alwaysFull.Value || !ConfigSettings.Compat.InfectedCompany.Value) //if compat is disabled, meter is disabled, or meter always full
                {
                    InfectedCompanyCompatibility.modInsanitySlider.gameObject.SetActive(true); //enable infectedcompany's insanity meter
                }
                else
                {
                    InfectedCompanyCompatibility.modInsanitySlider.gameObject.SetActive(false); //disable infectedcompany's insanity meter
                }
            }

            if (ConfigSettings.MeterColor.Value.StartsWith("#")) { ConfigSettings.MeterColor.Value.Substring(1); } //Remove # if user put it there
            ColorUtility.TryParseHtmlString("#" + ConfigSettings.MeterColor.Value, out Color meterColor);

            if (CompatibilityList.ModInstalled.EladsHUD && ConfigSettings.Compat.EladsHUD.Value && (!CompatibilityList.ModInstalled.LethalCompanyVR || CompatibilityList.ModInstalled.LethalCompanyVR && !ConfigSettings.Compat.LethalCompanyVR.Value))
            {
                if (EladsHUDCompatibility.InsanityInfo == null) { return; } //In case something goes wrong
                EladsHUDCompatibility.InsanityInfo.color = meterColor + new Color(0, 0, 0, 1); //Always set to completely visible regardless of config
                UpdateFillAmount(textMeter: EladsHUDCompatibility.InsanityInfo);
            }
            UpdateFillAmount(imageMeter: InsanityImage);
            InsanityImage.color = meterColor + new Color(0, 0, 0, 1); //Always set to completely visible regardless of config
        }
    }
}