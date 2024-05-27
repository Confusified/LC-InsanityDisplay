using BepInEx.Configuration;

namespace InsanityDisplay.Config
{
    public class ConfigSettings
    {
        //Display Settings
        public static ConfigEntry<bool> ModEnabled;
        public static ConfigEntry<string> MeterColor; //Default is purple (in file it would be HEX)
        public static ConfigEntry<bool> useAccurateDisplay;
        public static ConfigEntry<bool> enableReverse; //Become a sanity meter instead of insanity meter
        public static ConfigEntry<bool> alwaysFull; //Basically just always show the bar

        //Mod Compatibility Settings
        public class Compat
        {
            public static ConfigEntry<bool> LCCrouchHUD;
            public static ConfigEntry<bool> An0nPatches;
            public static ConfigEntry<bool> EladsHUD;
            public static ConfigEntry<bool> GeneralImprovements;
            public static ConfigEntry<bool> HealthMetrics;
            public static ConfigEntry<bool> DamageMetrics;
            public static ConfigEntry<bool> LethalCompanyVR;
            public static ConfigEntry<bool> InfectedCompany;
            //   public static ConfigEntry<bool> ShyHUD;
        }

        //_DontTouch
        public static ConfigEntry<byte> ConfigVersion;
        public static byte CurrentVersion = 1;
    }
}