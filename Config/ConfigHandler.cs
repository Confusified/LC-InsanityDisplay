using UnityEngine;
using static InsanityDisplay.Initialise;

namespace InsanityDisplay.Config
{
    public class ConfigHandler
    {
        public static void InitialiseConfig()
        {
            ConfigSettings.ModEnabled = modConfig.Bind<bool>("General Settings", "Meter enabled", true, "Add a bar above the stamina bar which display your insanity");
            ConfigSettings.MeterColor = modConfig.Bind<Color>("Bar Settings", "Color of the meter", new Color(0.45f, 0, 0.65f), "The colour that the insanity meter will have\n Example: FFFFFF (White)");
            ConfigSettings.useAccurateDisplay = modConfig.Bind<bool>("Bar Settings", "Accurate Meter", false, "Show your insanity value more accurately, instead of showing it in the vanilla way");

            ConfigSettings.LCCrouchHUDCompat = modConfig.Bind<bool>("Compatibility Settings", "Enable LCCrouchHUD Compatibility", true, "Enabling this will move the Crouch HUD slightly up to avoid overlapping");
            ConfigSettings.An0nPatchesCompat = modConfig.Bind<bool>("Compatibility Settings", "Enable An0n Patches Compatibility", true, "Enabling this will move the An0n Patches HUD slightly up to avoid overlapping");
            ConfigSettings.EladsHUDCompat = modConfig.Bind<bool>("Compatibility Settings", "Enable Elad's HUD Compatibility", true, "Enabling this will add another bar above the stamina bar displaying your insanity level");
            return;
        }
    }
}