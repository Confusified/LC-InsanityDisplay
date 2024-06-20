using UnityEngine;
using static LC_InsanityDisplay.Initialise;
using BepInEx.Configuration;
using System;
using LC_InsanityDisplay;

namespace LC_InsanityDisplay.Config
{
    public class ConfigHandler
    {
        //Display Settings
        public static ConfigEntry<bool> ModEnabled { get; internal set; }
        public static ConfigEntry<string> MeterColor { get; internal set; } //Default is purple (in file it would be HEX)
        public static ConfigEntry<bool> useAccurateDisplay { get; internal set; }
        public static ConfigEntry<bool> enableReverse { get; internal set; } //Become a sanity meter instead of insanity meter
        public static ConfigEntry<bool> alwaysFull { get; internal set; } //Basically just always show the bar
        public static ConfigEntry<bool> iconAlwaysCentered { get; internal set; } //The player icon that displays your health (i intend to move it whenever the insanity meter is not visible)
        // ^ change into an Enum, so user can choose between Always, AvoidOverlap, Never (haven't found a good name for the default behaviour yet)

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
        }

        //_DontTouch
        public static ConfigEntry<byte> ConfigVersion { get; internal set; }
        public static byte CurrentVersion = 1;

        public static void InitialiseConfig()
        {
            ModEnabled = modConfig.Bind("Display Settings", "Meter enabled", true, "Add a meter which displays your current insanity level?");
            MeterColor = modConfig.Bind("Display Settings", "Color of the Meter", "7300A6FF", "The colour that the insanity meter will have\n The colour value must be in HEX\nExample: FFFFFF(FF) (White)");
            useAccurateDisplay = modConfig.Bind("Display Settings", "Accurate meter", true, "Show your insanity value more accurately, instead of showing it in the vanilla way");
            enableReverse = modConfig.Bind("Display Settings", "Sanity Meter", false, "Turn the insanity meter into a sanity meter");
            alwaysFull = modConfig.Bind("Display Settings", "Always Show", false, "Always show the insanity meter, for aesthetic purposes");
            iconAlwaysCentered = modConfig.Bind("Display Settings", "Always Centered Player Icon", false, "Always have the player icon centered, instead of it moving to it's vanilla position when the insanity meter is not visible");

            Compat.LCCrouchHUD = modConfig.Bind("Mod Compatibility Settings", "Enable LCCrouchHUD compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.An0nPatches = modConfig.Bind("Mod Compatibility Settings", "Enable An0n Patches compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.EladsHUD = modConfig.Bind("Mod Compatibility Settings", "Enable Elads HUD compatibility", true, "Enabling this will add another bar above the stamina bar displaying your insanity level");
            Compat.GeneralImprovements = modConfig.Bind("Mod Compatibility Settings", "Enable GeneralImprovements compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.HealthMetrics = modConfig.Bind("Mod Compatibility Settings", "Enable HealthMetrics compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.DamageMetrics = modConfig.Bind("Mod Compatibility Settings", "Enable DamageMetrics compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.LethalCompanyVR = modConfig.Bind("Mod Compatibility Settings", "Enable LethalCompanyVR compatibility", true, "Enabling this will add the insanity meter to the hud in VR");
            Compat.InfectedCompany = modConfig.Bind("Mod Compatibility Settings", "Enable InfectedCompany compatibility", true, "Enabling this will hide InfectedCompany's insanity meter and use this mod's insanity meter instead");

            ConfigVersion = modConfig.Bind<byte>("z Do Not Touch z", "Config Version", 0, "The current version of your config file");

            RemoveDeprecatedSettings();

            FixColor(); //Fix the meter being white if the user's config doesn't start with '#'
            MeterColor.SettingChanged += FixColor;
            return;
        }

        private static void FixColor(object obj = null!, EventArgs args = null!)
        {
            if (MeterColor.Value.StartsWith("#")) { MeterColor.Value.Substring(1); } //Remove '#' from the user's config value
            ColorUtility.TryParseHtmlString("#" + MeterColor.Value, out Color meterColor);
            MeterColor.Value = ColorUtility.ToHtmlStringRGBA(meterColor + Color.black);
            Initialise.Logger.LogInfo($"{meterColor} {meterColor + Color.black}");
        }

        public static void RemoveDeprecatedSettings()
        {
            if (ConfigVersion.Value == CurrentVersion) { return; } //Don't update config values

            ConfigEntry<bool> oldBoolEntry;
            //From before ConfigVersion existed so anything below 1
            if (ConfigVersion.Value < 1)
            {
                oldBoolEntry = modConfig.Bind("Compatibility Settings", "Enable LCCrouchHUD compatibility", true);
                Compat.LCCrouchHUD.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind("Compatibility Settings", "Enable An0n Patches compatibility", true);
                Compat.An0nPatches.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind("Compatibility Settings", "Enable Elads HUD compatibility", true);
                Compat.EladsHUD.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind("Compatibility Settings", "Enable GeneralImprovements compatibility", true);
                Compat.GeneralImprovements.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind("Compatibility Settings", "Enable HealthMetrics compatibility", true);
                Compat.HealthMetrics.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind("Compatibility Settings", "Enable DamageMetrics compatibility", true);
                Compat.DamageMetrics.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                ConfigEntry<Color> oldColorEntry = modConfig.Bind("Display Settings", "Color of the meter", new Color(0.45f, 0, 0.65f, 1));
                MeterColor.Value = ColorUtility.ToHtmlStringRGB(oldColorEntry.Value);
                modConfig.Remove(oldColorEntry.Definition);

            }

            ConfigVersion.Value = CurrentVersion;
            Initialise.Logger.LogDebug("Succesfully updated config file version");
            modConfig.Save();
            return;
        }
    }
}