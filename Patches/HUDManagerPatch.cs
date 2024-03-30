using HarmonyLib;
using InsanityDisplay.Config;
using InsanityDisplay.ModCompatibility;
using System;
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
            if (InsanityImage == null || InsanityMeter == null) { return; } //In case something goes wrong
            InsanityMeter.SetActive(ConfigSettings.ModEnabled.Value);
            if (CompatibilityList.EladsHUD_Installed)
            {
                if (EladsHUDCompatibility.InsanityInfo == null) { return; } //In case something goes wrong
                EladsHUDCompatibility.InsanityInfo.color = ConfigSettings.MeterColor.Value;
                EladsHUDCompatibility.InsanityInfo.text = $"{Math.Floor(GetFillAmount() * 100)}%";
            }
            InsanityImage.fillAmount = GetFillAmount();
            InsanityImage.color = ConfigSettings.MeterColor.Value;
        }
    }
}