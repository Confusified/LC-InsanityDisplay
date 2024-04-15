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

            ConfigVersion = modConfig.Bind<int>("z Do Not Touch z", "Config Version", 0, "The current version of your config file");

            RemoveDeprecatedSettings();
            return;
        }

        public static void RemoveDeprecatedSettings()
        {
            if (ConfigVersion.Value == CurrentVersion) { return; } //Don't update config values

            //Old config entries
            //From before ConfigVersion existed so anything below 1
            if (ConfigVersion.Value < 1)
            {
                modConfig.Bind("Compatibility Settings", Compat.LCCrouchHUD.Definition.Key, true);
                modConfig.Bind("Compatibility Settings", Compat.An0nPatches.Definition.Key, true);
                modConfig.Bind("Compatibility Settings", Compat.EladsHUD.Definition.Key, true);
                modConfig.Bind("Compatibility Settings", Compat.GeneralImprovements.Definition.Key, true);
                modConfig.Bind("Compatibility Settings", Compat.HealthMetrics.Definition.Key, true);
                modConfig.Bind("Compatibility Settings", Compat.DamageMetrics.Definition.Key, true);
                Initialise.modLogger.LogInfo("test");
                modConfig.Bind<Color>(MeterColor.Definition.Section, "Color of the meter", new Color(0.45f, 0, 0.65f));
                Initialise.modLogger.LogInfo("after");
            }

            foreach (ConfigDefinition cDef in modConfig.Keys)
            {
                if (cDef.Section == "Compatibility Settings")
                {
                    if (cDef.Key == Compat.LCCrouchHUD.Definition.Key)
                    {
                        Initialise.modLogger.LogDebug("Removing old LCCrouchHUD config value");
                        modConfig.TryGetEntry(cDef, out ConfigEntry<bool> entry);
                        Compat.LCCrouchHUD.Value = entry.Value;
                        modConfig.Remove(cDef);
                        continue;
                    }
                    else if (cDef.Key == Compat.An0nPatches.Definition.Key)
                    {
                        Initialise.modLogger.LogDebug("Removing old An0n Patches config value");
                        modConfig.TryGetEntry(cDef, out ConfigEntry<bool> entry);
                        Compat.An0nPatches.Value = entry.Value;
                        modConfig.Remove(cDef);
                        continue;
                    }
                    else if (cDef.Key == Compat.EladsHUD.Definition.Key)
                    {
                        Initialise.modLogger.LogDebug("Removing old Elad's HUD config value");
                        modConfig.TryGetEntry(cDef, out ConfigEntry<bool> entry);
                        Compat.EladsHUD.Value = entry.Value;
                        modConfig.Remove(cDef);
                        continue;
                    }
                    else if (cDef.Key == Compat.GeneralImprovements.Definition.Key)
                    {
                        Initialise.modLogger.LogDebug("Removing old GeneralImprovements config value");
                        modConfig.TryGetEntry(cDef, out ConfigEntry<bool> entry);
                        Compat.GeneralImprovements.Value = entry.Value;
                        modConfig.Remove(cDef);
                        continue;
                    }
                    else if (cDef.Key == Compat.HealthMetrics.Definition.Key)
                    {
                        Initialise.modLogger.LogDebug("Removing old HealthMetrics config value");
                        modConfig.TryGetEntry(cDef, out ConfigEntry<bool> entry);
                        Compat.HealthMetrics.Value = entry.Value;
                        modConfig.Remove(cDef);
                        continue;
                    }
                    else if (cDef.Key == Compat.DamageMetrics.Definition.Key)
                    {
                        Initialise.modLogger.LogDebug("Removing old DamageMetrics config value");
                        modConfig.TryGetEntry(cDef, out ConfigEntry<bool> entry);
                        Compat.DamageMetrics.Value = entry.Value;
                        modConfig.Remove(cDef);
                        continue;
                    }
                }
                else if (cDef.Section == "Display Settings") //also make sure it is the old one
                {
                    if (cDef.Key == "Color of the meter")
                    {
                        Initialise.modLogger.LogDebug("Removing old MeterColor config value");
                        modConfig.TryGetEntry<Color>(cDef, out ConfigEntry<Color> entry);
                        MeterColor.Value = ColorUtility.ToHtmlStringRGB(entry.Value);
                        modConfig.Remove(cDef);
                        continue;
                    }
                }
            }
            ConfigVersion.Value = CurrentVersion;
            Initialise.modLogger.LogDebug("Succesfully updated config file version");
            modConfig.Save();
            return;
        }
    }
}