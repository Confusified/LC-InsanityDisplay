using UnityEngine;
using static InsanityDisplay.Initialise;

namespace InsanityDisplay.Config
{
    public class ConfigHandler
    {
        public static void InitialiseConfig()
        {
            ConfigSettings.ModEnabled = modConfig.Bind<bool>("Display Settings", "Meter enabled", true, "Add a bar above the stamina bar which display your insanity");
            ConfigSettings.MeterColor = modConfig.Bind<Color>("Display Settings", "Color of the meter", new Color(0.45f, 0, 0.65f, 1), "The colour that the insanity meter will have\n The colour value must be in HEX\nExample: FFFFFF(FF) (White)");
            ConfigSettings.useAccurateDisplay = modConfig.Bind<bool>("Display Settings", "Accurate meter", false, "Show your insanity value more accurately, instead of showing it in the vanilla way");
            ConfigSettings.enableReverse = modConfig.Bind<bool>("Display Settings", "Sanity Meter", false, "Turn the insanity meter into a sanity meter");
            ConfigSettings.alwaysFull = modConfig.Bind<bool>("Display Settings", "Always Show", false, "Always show the insanity meter, for aesthetic purposes");

            ConfigSettings.LCCrouchHUDCompat = modConfig.Bind<bool>("Compatibility Settings", "Enable LCCrouchHUD compatibility", true, "Enabling this will move the Crouch HUD slightly up to avoid overlapping");
            ConfigSettings.An0nPatchesCompat = modConfig.Bind<bool>("Compatibility Settings", "Enable An0n Patches compatibility", true, "Enabling this will move the An0n Patches HUD slightly up to avoid overlapping");
            ConfigSettings.EladsHUDCompat = modConfig.Bind<bool>("Compatibility Settings", "Enable Elads HUD compatibility", true, "Enabling this will add another bar above the stamina bar displaying your insanity level");
            return;
        }
    }
}