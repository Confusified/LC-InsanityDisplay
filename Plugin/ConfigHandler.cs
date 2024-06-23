using BepInEx.Configuration;
using LC_InsanityDisplay.Plugin.UI;
using System;
using UnityEngine;
using static LC_InsanityDisplay.Plugin.Initialise;

namespace LC_InsanityDisplay.Plugin
{
    public class ConfigHandler
    {
        //Display Settings
        public static ConfigEntry<bool> ModEnabled { get; internal set; } = null!; //If disabled the mod simply won't run
        public static ConfigEntry<string> MeterColor { get; internal set; } = null!;//Default is purple (in file it would be HEX)
        public static ConfigEntry<bool> useAccurateDisplay { get; internal set; } = null!;
        public static ConfigEntry<bool> enableReverse { get; internal set; } = null!; //Become a sanity meter instead of insanity meter
        public static ConfigEntry<bool> alwaysFull { get; internal set; } = null!; //Basically just always show the bar
        public static ConfigEntry<CenteredIconSettings> iconAlwaysCentered { get; internal set; } = null!;//The player icon that displays your health (i intend to move it whenever the insanity meter is not visible)

        public enum CenteredIconSettings
        {
            Never = 0,
            AvoidOverlap = 1,
            Always = 2
        }

        //Mod Compatibility Settings
        public class Compat
        {
            public static ConfigEntry<bool> LCCrouchHUD { get; internal set; } = null!;
            public static ConfigEntry<bool> An0nPatches { get; internal set; } = null!;
            public static ConfigEntry<bool> EladsHUD { get; internal set; } = null!;
            public static ConfigEntry<bool> GeneralImprovements { get; internal set; } = null!;
            public static ConfigEntry<bool> HealthMetrics { get; internal set; } = null!;
            public static ConfigEntry<bool> DamageMetrics { get; internal set; } = null!;
            public static ConfigEntry<bool> LethalCompanyVR { get; internal set; } = null!;
            public static ConfigEntry<bool> InfectedCompany { get; internal set; } = null!;
            public static ConfigEntry<bool> InfectedCompany_InfectedOnly { get; internal set; } = null!;
        }

        //_DontTouch
        public static ConfigEntry<byte> ConfigVersion { get; internal set; } = null!;
        public static byte CurrentVersion = 3;

        public static void InitialiseConfig()
        {
            ModEnabled = modConfig.Bind("Display Settings", "Meter enabled", true, "Add a meter which displays your current insanity level?");
            MeterColor = modConfig.Bind("Display Settings", "Color of the Meter", "7300A6FF", "The colour that the insanity meter will have\n The colour value must be in HEX\nExample: FFFFFF(FF) (White)");
            useAccurateDisplay = modConfig.Bind("Display Settings", "Accurate meter", true, "Show your insanity value more accurately, instead of showing it in the vanilla way");
            enableReverse = modConfig.Bind("Display Settings", "Sanity Meter", false, "Turn the insanity meter into a sanity meter");
            alwaysFull = modConfig.Bind("Display Settings", "Always Show", false, "Always show the insanity meter, for aesthetic purposes");
            iconAlwaysCentered = modConfig.Bind("Display Settings", "Center Player Icon", CenteredIconSettings.AvoidOverlap, "Always have the player icon centered, instead of it moving to it's vanilla position when the insanity meter is not visible");

            Compat.LCCrouchHUD = modConfig.Bind("Mod Compatibility Settings", "Enable LCCrouchHUD compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.An0nPatches = modConfig.Bind("Mod Compatibility Settings", "Enable An0n Patches compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.EladsHUD = modConfig.Bind("Mod Compatibility Settings", "Enable Elads HUD compatibility", true, "Enabling this will add another bar above the stamina bar displaying your insanity level");
            Compat.GeneralImprovements = modConfig.Bind("Mod Compatibility Settings", "Enable GeneralImprovements compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.HealthMetrics = modConfig.Bind("Mod Compatibility Settings", "Enable HealthMetrics compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.DamageMetrics = modConfig.Bind("Mod Compatibility Settings", "Enable DamageMetrics compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.LethalCompanyVR = modConfig.Bind("Mod Compatibility Settings", "Enable LethalCompanyVR compatibility", true, "Enabling this will add the insanity meter to the hud in VR");
            Compat.InfectedCompany = modConfig.Bind("Mod Compatibility Settings", "Enable InfectedCompany compatibility", true, "Enabling this will hide InfectedCompany's insanity meter and use this mod's insanity meter instead");
            Compat.InfectedCompany_InfectedOnly = modConfig.Bind("Mod Compatibility Settings", "Only show Insanity Meter when infected", false, "Enabling this will only show the insanity meter when you are the infected");

            ConfigVersion = modConfig.Bind<byte>("z Do Not Touch z", "Config Version", 0, "The current version of your config file");

            RemoveDeprecatedSettings();

            FixColor(); //Fix the meter being white if the user's config doesn't start with '#'
        }

        internal static void SettingChanged(object sender, EventArgs e)
        {
            if (HUDInjector.InsanityMeter) HUDBehaviour.UpdateMeter(settingChanged: true); //Update the insanity meter if it exists
            if (HUDBehaviour.PlayerIcon && HUDBehaviour.PlayerRedIcon) HUDBehaviour.UpdateIconPosition(settingChanged: true); //Update the icon if it exists
        }

        internal static void FixColor(object obj = null!, EventArgs args = null!)
        {
            if (MeterColor.Value.StartsWith("#")) { MeterColor.Value.Substring(1); } //Remove '#' from the user's config value
            ColorUtility.TryParseHtmlString("#" + MeterColor.Value, out Color meterColor);
            Color newColor = meterColor + Color.black;
            MeterColor.Value = ColorUtility.ToHtmlStringRGBA(newColor);
            HUDBehaviour.InsanityMeterColor = newColor;
            if (HUDInjector.InsanityMeter) HUDBehaviour.UpdateMeter(settingChanged: true); //Update the insanity meter if it exists
        }

        public static void RemoveDeprecatedSettings()
        {
            byte oldConfigVersion = ConfigVersion.Value;
            if (oldConfigVersion == CurrentVersion) { return; } //Don't update config values

            ConfigEntry<bool> oldBoolEntry;
            ConfigEntry<Color> oldColorEntry;
            //From before ConfigVersion existed so anything below 1
            if (oldConfigVersion < 1)
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

                oldColorEntry = modConfig.Bind("Display Settings", "Color of the meter", new Color(0.45f, 0, 0.65f, 1));
                MeterColor.Value = ColorUtility.ToHtmlStringRGB(oldColorEntry.Value);
                modConfig.Remove(oldColorEntry.Definition);

            }
            if (oldConfigVersion < 2) //below 1.3.0
            {
                oldBoolEntry = modConfig.Bind("Display Settings", "Always Centered Player Icon", true);
                if (oldBoolEntry.Value) iconAlwaysCentered.Value = CenteredIconSettings.Always;
                else iconAlwaysCentered.Value = CenteredIconSettings.AvoidOverlap;
                modConfig.Remove(oldBoolEntry.Definition);
            }
            //remove orphaned entries
            /*
            PropertyInfo orphanedEntriesProp = modConfig.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);
            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(modConfig, null);
            orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
            */

            ConfigVersion.Value = CurrentVersion;
            Initialise.Logger.LogDebug($"Succesfully updated config file version from {oldConfigVersion} => {CurrentVersion}");
            return;
        }
    }
}