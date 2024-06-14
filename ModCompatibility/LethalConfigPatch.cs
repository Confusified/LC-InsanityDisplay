﻿using LethalConfig;
using LethalConfig.ConfigItems;
using static InsanityDisplay.Config.ConfigSettings;
using LethalConfig.ConfigItems.Options;

namespace InsanityDisplay.ModCompatibility
{
    public class LethalConfigPatch
    {
        public static void SetLethalConfigEntries()
        {
            //Skip auto-generating a config 
            LethalConfigManager.SkipAutoGen();

            //Display Settings
            var modenabledEntry = new BoolCheckBoxConfigItem(ModEnabled, false);
            var metercolorEntry = new TextInputFieldConfigItem(MeterColor, new TextInputFieldOptions { CharacterLimit = 9, RequiresRestart = false });
            var useaccuratedisplayEntry = new BoolCheckBoxConfigItem(useAccurateDisplay, false);
            var enablereverseEntry = new BoolCheckBoxConfigItem(enableReverse, false);
            var alwaysfullEntry = new BoolCheckBoxConfigItem(alwaysFull, false);
            var iconAlwaysCenteredEntry = new BoolCheckBoxConfigItem(iconAlwaysCentered, false);
            //Mod Compatibility Settings
            var lccrouchudEntry = new BoolCheckBoxConfigItem(Compat.LCCrouchHUD, false);
            var an0npatchesEntry = new BoolCheckBoxConfigItem(Compat.An0nPatches, false);
            var eladshudEntry = new BoolCheckBoxConfigItem(Compat.EladsHUD, false);
            var generalimprovementsEntry = new BoolCheckBoxConfigItem(Compat.GeneralImprovements, false);
            var healthmetricsEntry = new BoolCheckBoxConfigItem(Compat.HealthMetrics, false);
            var damagemetricsEntry = new BoolCheckBoxConfigItem(Compat.DamageMetrics, false);
            var lcvrEntry = new BoolCheckBoxConfigItem(Compat.LethalCompanyVR, true); //NOT FINISHED YET (did i never fully implement this? damn)
            var infectedcompanyEntry = new BoolCheckBoxConfigItem(Compat.InfectedCompany, false);

            //Display Settings
            LethalConfigManager.AddConfigItem(modenabledEntry);
            LethalConfigManager.AddConfigItem(metercolorEntry);
            LethalConfigManager.AddConfigItem(useaccuratedisplayEntry);
            LethalConfigManager.AddConfigItem(enablereverseEntry);
            LethalConfigManager.AddConfigItem(alwaysfullEntry);
            LethalConfigManager.AddConfigItem(iconAlwaysCenteredEntry);
            //Mod Compatibility Settings
            LethalConfigManager.AddConfigItem(lccrouchudEntry);
            LethalConfigManager.AddConfigItem(an0npatchesEntry);
            LethalConfigManager.AddConfigItem(eladshudEntry);
            LethalConfigManager.AddConfigItem(generalimprovementsEntry);
            LethalConfigManager.AddConfigItem(healthmetricsEntry);
            LethalConfigManager.AddConfigItem(damagemetricsEntry);
            LethalConfigManager.AddConfigItem(lcvrEntry); //NOT FINISHED YET
            LethalConfigManager.AddConfigItem(infectedcompanyEntry);

            LethalConfigManager.SetModDescription("Adds an insanity meter to the hud in vanilla style");

            Initialise.modLogger.LogDebug("Added entries to LethalConfig");
        }
    }
}
