using BepInEx.Configuration;
using UnityEngine;

namespace InsanityDisplay.Config
{
    public class ConfigSettings
    {
        public static ConfigEntry<bool> ModEnabled;
        public static ConfigEntry<Color> MeterColor; //Default is purple (in file it would be HEX)
        public static ConfigEntry<bool> useAccurateDisplay;
        public static ConfigEntry<bool> enableReverse; //Become a sanity meter instead of insanity meter
        public static ConfigEntry<bool> alwaysFull; //Basically just always show the bar

        //Compatibility Settings

        public static ConfigEntry<bool> LCCrouchHUDCompat;
        public static ConfigEntry<bool> An0nPatchesCompat;
        public static ConfigEntry<bool> EladsHUDCompat;
        public static ConfigEntry<bool> GeneralImprovementsCompat;
    }
}