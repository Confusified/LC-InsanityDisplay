using GameNetcodeStuff;
using HarmonyLib;
using InsanityDisplay.Config;
using static InsanityDisplay.UI.UIHandler;

namespace InsanityDisplay.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    public class HUDManagerPatch
    {
        public static PlayerControllerB PlayerControllerBInstance;

        [HarmonyPatch("Awake")]
        private static void Postfix(HUDManager __instance)
        {
            PlayerControllerBInstance = __instance.localPlayer;
            CreateInMemory(); //This will create in memory AND also in scene afterwards
        }

        [HarmonyPatch("Update")]
        private static void Postfix()
        {
            if (InsanityImage == null) { return; } //In case something goes wrong
            InsanityMeter.SetActive(ConfigSettings.ModEnabled.Value);
            InsanityImage.fillAmount = GetFillAmount();
            InsanityImage.color = ConfigSettings.MeterColor.Value;
        }
    }
}