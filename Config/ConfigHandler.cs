using UnityEngine;
using static InsanityDisplay.Initialise;

namespace InsanityDisplay.Config
{
    public class ConfigHandler
    {
        public static void InitialiseConfig()
        {
            ConfigSettings.ModEnabled = modConfig.Bind<bool>("General Settings", "Meter enabled", true, "Add a bar above the stamina bar which display your insanity");
            ConfigSettings.MeterColor = modConfig.Bind<Color>("Bar Settings", "Color of the meter", new Color(0.45f, 0, 0.65f), "The colour that the insanity meter will have");
            ConfigSettings.useAccurateDisplay = modConfig.Bind<bool>("Bar Settings", "Accurate Meter", false, "Show your insanity value more accurately, instead of showing it in the vanilla way");
            return;
        }
    }
}