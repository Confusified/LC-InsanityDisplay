using LethalConfig;
using LethalConfig.ConfigItems;
using static InsanityDisplay.Config.ConfigSettings;

namespace InsanityDisplay.ModCompatibility
{
    public class LethalConfigPatch
    {
        //stuff for that mod
        public static void SetLethalConfigEntries()
        {
            //Display Settings
            var modenabledEntry = new BoolCheckBoxConfigItem(ModEnabled, false);
            var metercolorEntry = new TextInputFieldConfigItem(MeterColor, false);
            var useaccuratedisplayEntry = new BoolCheckBoxConfigItem(useAccurateDisplay, false);
            var enablereverseEntry = new BoolCheckBoxConfigItem(enableReverse, false);
            var alwaysfullEntry = new BoolCheckBoxConfigItem(alwaysFull, false);
            //Mod Compatibility Settings
            var lccrouchudEntry = new BoolCheckBoxConfigItem(Compat.LCCrouchHUD, false);
            var an0npatchesEntry = new BoolCheckBoxConfigItem(Compat.An0nPatches, false);
            var eladshudEntry = new BoolCheckBoxConfigItem(Compat.EladsHUD, false);
            var generalimprovementsEntry = new BoolCheckBoxConfigItem(Compat.GeneralImprovements, false);
            var healthmetricsEntry = new BoolCheckBoxConfigItem(Compat.HealthMetrics, false);
            var damagemetricsEntry = new BoolCheckBoxConfigItem(Compat.DamageMetrics, false);

            //Display Settings
            LethalConfigManager.AddConfigItem(modenabledEntry);
            LethalConfigManager.AddConfigItem(metercolorEntry);
            LethalConfigManager.AddConfigItem(useaccuratedisplayEntry);
            LethalConfigManager.AddConfigItem(enablereverseEntry);
            LethalConfigManager.AddConfigItem(alwaysfullEntry);
            //Mod Compatibility Settings
            LethalConfigManager.AddConfigItem(lccrouchudEntry);
            LethalConfigManager.AddConfigItem(an0npatchesEntry);
            LethalConfigManager.AddConfigItem(eladshudEntry);
            LethalConfigManager.AddConfigItem(generalimprovementsEntry);
            LethalConfigManager.AddConfigItem(healthmetricsEntry);
            LethalConfigManager.AddConfigItem(damagemetricsEntry);

            Initialise.modLogger.LogDebug("Added entries to LethalConfig");
        }
    }
}
