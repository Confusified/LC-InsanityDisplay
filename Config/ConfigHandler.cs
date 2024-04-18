using UnityEngine;
using static InsanityDisplay.Initialise;
using static InsanityDisplay.Config.ConfigSettings;
using BepInEx.Configuration;

namespace InsanityDisplay.Config
{
    public class ConfigHandler
    {
        public static void InitialiseConfig()
        {
            ModEnabled = modConfig.Bind<bool>("Display Settings", "Meter enabled", true, "Add a bar above the stamina bar which display your insanity");
            MeterColor = modConfig.Bind<string>("Display Settings", "Color of the Meter", "7300A6FF", "The colour that the insanity meter will have\n The colour value must be in HEX\nExample: FFFFFF(FF) (White)");
            useAccurateDisplay = modConfig.Bind<bool>("Display Settings", "Accurate meter", false, "Show your insanity value more accurately, instead of showing it in the vanilla way");
            enableReverse = modConfig.Bind<bool>("Display Settings", "Sanity Meter", false, "Turn the insanity meter into a sanity meter");
            alwaysFull = modConfig.Bind<bool>("Display Settings", "Always Show", false, "Always show the insanity meter, for aesthetic purposes");

            Compat.LCCrouchHUD = modConfig.Bind<bool>("Mod Compatibility Settings", "Enable LCCrouchHUD compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.An0nPatches = modConfig.Bind<bool>("Mod Compatibility Settings", "Enable An0n Patches compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.EladsHUD = modConfig.Bind<bool>("Mod Compatibility Settings", "Enable Elads HUD compatibility", true, "Enabling this will add another bar above the stamina bar displaying your insanity level");
            Compat.GeneralImprovements = modConfig.Bind<bool>("Mod Compatibility Settings", "Enable GeneralImprovements compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.HealthMetrics = modConfig.Bind<bool>("Mod Compatibility Settings", "Enable HealthMetrics compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.DamageMetrics = modConfig.Bind<bool>("Mod Compatibility Settings", "Enable DamageMetrics compatibility", true, "Enabling this will adjust the hud to avoid overlapping");
            Compat.LethalCompanyVR = modConfig.Bind<bool>("Mod Compatibility Settings", "Enable LethalCompanyVR compatibility", true, "Enabling this will add the insanity meter to the hud in VR");
            Compat.InfectedCompany = modConfig.Bind<bool>("Mod Compatibility Settings", "Enable InfectedCompany compatibility", true, "Enabling this will hide InfectedCompany's insanity meter and use this mod's insanity meter instead");

            ConfigVersion = modConfig.Bind<byte>("z Do Not Touch z", "Config Version", 0, "The current version of your config file");

            RemoveDeprecatedSettings();
            return;
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
            Initialise.modLogger.LogDebug("Succesfully updated config file version");
            modConfig.Save();
            return;
        }
    }
}