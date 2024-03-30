using UnityEngine;
using static InsanityDisplay.Initialise;

namespace InsanityDisplay.Config
{
    public class ConfigHandler
    {
        public static void InitialiseConfig()
        {
            ConfigSettings.ModEnabled = modConfig.Bind<bool>("Mod Settings", "Meter enabled", true, "Add a bar underneath the stamina bar which display your insanity");
            ConfigSettings.MeterColor = modConfig.Bind<Color>("Mod Settings", "Color of the meter", new Color(0.35f, 0, 0.65f), "The colour that the insanity meter will have");
            return;
        }
    }
}