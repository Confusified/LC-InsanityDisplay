using BepInEx.Configuration;
using UnityEngine;

namespace InsanityDisplay.Config
{
    public class ConfigSettings
    {
        public static ConfigEntry<bool> ModEnabled;
        public static ConfigEntry<Color> MeterColor; //Default is purple
        public static ConfigEntry<bool> useAccurateDisplay;
    }
}