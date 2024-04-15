using HarmonyLib;
using InsanityDisplay.Config;
using InsanityDisplay.ModCompatibility;
using System;
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

            if (ConfigSettings.MeterColor.Value.StartsWith("#")) { ConfigSettings.MeterColor.Value.Substring(1); } //Remove # if user put it there
            ColorUtility.TryParseHtmlString("#" + ConfigSettings.MeterColor.Value, out Color meterColor);

            if (CompatibilityList.ModInstalled.EladsHUD && ConfigSettings.Compat.EladsHUD.Value)
            {
                if (EladsHUDCompatibility.InsanityInfo == null) { return; } //In case something goes wrong
                EladsHUDCompatibility.InsanityInfo.color = meterColor + new Color(0, 0, 0, 1); //Always set to completely visible regardless of config
                EladsHUDCompatibility.InsanityInfo.text = $"{Math.Floor(GetFillAmount() * 100)}%";
            }
            InsanityImage.fillAmount = GetFillAmount();
            InsanityImage.color = meterColor + new Color(0, 0, 0, 1); //Always set to completely visible regardless of config
        }
    }
}