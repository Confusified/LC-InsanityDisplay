using BepInEx.Configuration;
using LC_InsanityDisplay.Plugin.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static LC_InsanityDisplay.Plugin.Initialise;

namespace LC_InsanityDisplay.Plugin
{
    public class ConfigHandler
    {
        // Display Settings
        public static ConfigEntry<bool> ModEnabled { get; internal set; } = null!; // If disabled the mod simply won't run
        public static ConfigEntry<string> MeterColor { get; internal set; } = null!;// Default is purple (in file it would be HEX)
        public static ConfigEntry<bool> useAccurateDisplay { get; internal set; } = null!;
        public static ConfigEntry<bool> enableReverse { get; internal set; } = null!; // Become a sanity meter instead of insanity meter
        public static ConfigEntry<bool> alwaysFull { get; internal set; } = null!; // Basically just always show the bar
        public static ConfigEntry<CenteredIconSettings> iconAlwaysCentered { get; internal set; } = null!;// The player icon that displays your health (i intend to move it whenever the insanity meter is not visible)

        public enum CenteredIconSettings
        {
            Never = 0,
            AvoidOverlap = 1,
            Always = 2
        }

        // Mod Compatibility Settings (as of 1.3.0 they are now in their own sections)
        public class Compat
        {
            public static ConfigEntry<bool> LCCrouchHUD { get; internal set; } = null!;
            public static ConfigEntry<bool> An0nPatches { get; internal set; } = null!;
            public static ConfigEntry<bool> LethalCompanyPatched { get; internal set; } = null!;
            public static ConfigEntry<bool> EladsHUD { get; internal set; } = null!;
            public static ConfigEntry<bool> GeneralImprovements { get; internal set; } = null!;
            public static ConfigEntry<bool> HealthMetrics { get; internal set; } = null!;
            public static ConfigEntry<bool> DamageMetrics { get; internal set; } = null!;
            public static ConfigEntry<bool> LethalCompanyVR { get; internal set; } = null!;
            public static ConfigEntry<bool> InfectedCompany { get; internal set; } = null!;
            public static ConfigEntry<bool> InfectedCompany_InfectedOnly { get; internal set; } = null!;
            public static ConfigEntry<bool> ShyHUD { get; internal set; } = null!;
        }

        // z Do Not Touch z (intentionally starts with a 'z', so that the config is at the bottom)
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

            Compat.LCCrouchHUD = modConfig.Bind("CrouchHUD Compatibility Settings", "Enable CrouchHUD compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.An0nPatches = modConfig.Bind("An0n Patches Compatibility Settings", "Enable An0n Patches compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.LethalCompanyPatched = modConfig.Bind("LethalCompanyPatched Compatibility Settings", "Enable LethalCompanyPatched compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.EladsHUD = modConfig.Bind("Elads HUD Compatibility Settings", "Enable Elads HUD compatibility", true, "Enabling this will add another bar above the stamina bar displaying your insanity level");
            Compat.GeneralImprovements = modConfig.Bind("GeneralImprovements Compatibility Settings", "Enable GeneralImprovements compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.HealthMetrics = modConfig.Bind("HealthMetrics Compatibility Settings", "Enable HealthMetrics compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.DamageMetrics = modConfig.Bind("DamageMetrics Compatibility Settings", "Enable DamageMetrics compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.LethalCompanyVR = modConfig.Bind("LethalCompanyVR Compatibility Settings", "Enable LethalCompanyVR compatibility", true, "Enabling this will add the insanity meter to the hud in VR");
            Compat.InfectedCompany = modConfig.Bind("InfectedCompany Compatibility Settings", "Enable InfectedCompany compatibility", true, "Enabling this will hide InfectedCompany's insanity meter and use this mod's insanity meter instead");
            Compat.InfectedCompany_InfectedOnly = modConfig.Bind("InfectedCompany Compatibility Settings", "Only show Insanity Meter when infected", false, "Enabling this will only show the insanity meter when you are the infected");
            Compat.ShyHUD = modConfig.Bind("ShyHUD Compatibility Settings", "Enable ShyHUD compatibility", true, "Enabling this will hide the insanity meter when it's full");

            ConfigVersion = modConfig.Bind<byte>("z Do Not Touch z", "Config Version", 0, "The current version of your config file");

            RemoveDeprecatedSettings();

            FixColor(); // Fix the meter being white if the user's config doesn't start with '#' (generally for older versions of the mod but, i'm keeping it here just to be safe)
        }

        internal static void SettingChanged(object sender = null!, EventArgs e = null!)
        {
            if (HUDInjector.InsanityMeter) HUDBehaviour.UpdateMeter(settingChanged: true); // Update the insanity meter if it exists
            if (HUDBehaviour.PlayerIcon && HUDBehaviour.PlayerRedIcon) HUDBehaviour.UpdateIconPosition(settingChanged: true); // Update the icon if it exists
        }

        internal static void FixColor(object obj = null!, EventArgs args = null!)
        {
            if (MeterColor.Value.StartsWith("#")) { MeterColor.Value.Substring(1); } // Remove '#' from the user's config value
            ColorUtility.TryParseHtmlString("#" + MeterColor.Value, out Color meterColor);
            if (meterColor.a != 1) { meterColor.a = 1; }
            MeterColor.Value = ColorUtility.ToHtmlStringRGBA(meterColor);
            HUDBehaviour.InsanityMeterColor = meterColor;
            if (HUDInjector.InsanityMeter) HUDBehaviour.UpdateMeter(settingChanged: true); // Update the insanity meter if it exists
        }

        public static void RemoveDeprecatedSettings()
        {
            // Could be improved by putting the settings in their own if statements as to not unnecessarily set a value just for it to be overwritten by an older value)
            // So, setting from < 2 is set and after setting from < 1 is set, this is inefficient (maybe i'll change that before 1.3.0 release)
            byte oldConfigVersion = ConfigVersion.Value;
            if (oldConfigVersion == CurrentVersion) { return; } // Don't update config values
            // I feel like there's a better way to do this but, I don't know how to
            ConfigEntry<bool> oldBoolEntry;
            ConfigEntry<Color> oldColorEntry;
            // From before ConfigVersion existed so anything below 1

            if (oldConfigVersion < 2) // Below 1.3.0 (people using the alpha versions of 1.3.0 will have their compatibility settings reset, due to an oversight)
            {
                const string DisplaySection = "Display Settings";
                const string ModCompatSection = "Mod Compatibility Settings";

                oldBoolEntry = modConfig.Bind(DisplaySection, "Always Centered Player Icon", true);
                if (oldBoolEntry.Value) iconAlwaysCentered.Value = CenteredIconSettings.Always;
                else iconAlwaysCentered.Value = CenteredIconSettings.AvoidOverlap;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind(ModCompatSection, "Enable LCCrouchHUD compatibility", true);
                Compat.LCCrouchHUD.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind(ModCompatSection, "Enable An0n Patches compatibility", true);
                Compat.An0nPatches.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind(ModCompatSection, "Enable Elads HUD compatibility", true);
                Compat.EladsHUD.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind(ModCompatSection, "Enable GeneralImprovements compatibility", true);
                Compat.GeneralImprovements.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind(ModCompatSection, "Enable HealthMetrics compatibility", true);
                Compat.HealthMetrics.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind(ModCompatSection, "Enable DamgeMetrics compatibility", true);
                Compat.DamageMetrics.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind(ModCompatSection, "Enable LethalCompanyVR compatibility", true);
                Compat.LethalCompanyVR.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind(ModCompatSection, "Enable InfectedCompany compatibility", true);
                Compat.InfectedCompany.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                oldBoolEntry = modConfig.Bind(ModCompatSection, "Only show Insanity Meter when infected", true);
                Compat.InfectedCompany_InfectedOnly.Value = oldBoolEntry.Value;
                modConfig.Remove(oldBoolEntry.Definition);

                if (oldConfigVersion < 1)
                {
                    const string CompatSection = "Compatibility Settings";
                    oldBoolEntry = modConfig.Bind(CompatSection, "Enable LCCrouchHUD compatibility", true);
                    Compat.LCCrouchHUD.Value = oldBoolEntry.Value;
                    modConfig.Remove(oldBoolEntry.Definition);

                    oldBoolEntry = modConfig.Bind(CompatSection, "Enable An0n Patches compatibility", true);
                    Compat.An0nPatches.Value = oldBoolEntry.Value;
                    modConfig.Remove(oldBoolEntry.Definition);

                    oldBoolEntry = modConfig.Bind(CompatSection, "Enable Elads HUD compatibility", true);
                    Compat.EladsHUD.Value = oldBoolEntry.Value;
                    modConfig.Remove(oldBoolEntry.Definition);

                    oldBoolEntry = modConfig.Bind(CompatSection, "Enable GeneralImprovements compatibility", true);
                    Compat.GeneralImprovements.Value = oldBoolEntry.Value;
                    modConfig.Remove(oldBoolEntry.Definition);

                    oldBoolEntry = modConfig.Bind(CompatSection, "Enable HealthMetrics compatibility", true);
                    Compat.HealthMetrics.Value = oldBoolEntry.Value;
                    modConfig.Remove(oldBoolEntry.Definition);

                    oldBoolEntry = modConfig.Bind(CompatSection, "Enable DamageMetrics compatibility", true);
                    Compat.DamageMetrics.Value = oldBoolEntry.Value;
                    modConfig.Remove(oldBoolEntry.Definition);

                    oldColorEntry = modConfig.Bind(DisplaySection, "Color of the meter", new Color(0.45f, 0, 0.65f, 1));
                    MeterColor.Value = ColorUtility.ToHtmlStringRGB(oldColorEntry.Value);
                    modConfig.Remove(oldColorEntry.Definition);
                }
            }
            // Clear orphaned entries (Unbinded/Abandoned entries)
            PropertyInfo orphanedEntriesProp = modConfig.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);
            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(modConfig, null);
            orphanedEntries.Clear();

            ConfigVersion.Value = CurrentVersion;
            Initialise.Logger.LogDebug($"Succesfully updated config file version from {oldConfigVersion} => {CurrentVersion}");
            return;
        }
    }
}