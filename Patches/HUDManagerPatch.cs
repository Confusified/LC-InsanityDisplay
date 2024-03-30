using GameNetcodeStuff;
using HarmonyLib;
using InsanityDisplay.Config;
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
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void SetMeterValues()
        {
            if (InsanityImage == null) { return; } //In case something goes wrong

            InsanityMeter.SetActive(ConfigSettings.ModEnabled.Value);
            InsanityImage.fillAmount = GetFillAmount();
            InsanityImage.color = ConfigSettings.MeterColor.Value;
        }
    }
}