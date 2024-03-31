using BepInEx.Configuration;
using UnityEngine;

namespace InsanityDisplay.Config
{
    public class ConfigSettings
    {
        public static ConfigEntry<bool> ModEnabled;
        public static ConfigEntry<Color> MeterColor; //Default is purple (in file it would be HEX)
        public static ConfigEntry<bool> useAccurateDisplay;

        //Compatibility Settings

        public static ConfigEntry<bool> LCCrouchHUDCompat;
        public static ConfigEntry<bool> An0nPatchesCompat;
        public static ConfigEntry<bool> EladsHUDCompat;
    }
}