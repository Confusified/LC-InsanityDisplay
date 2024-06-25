using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using static LC_InsanityDisplay.Plugin.ConfigHandler;

namespace LC_InsanityDisplay.Plugin.ModCompatibility
{
    /// <summary>
    /// Handles behaviour related to LethalConfig
    /// </summary>
    public class LethalConfigCompatibility
    {
        internal const string ModGUID = "ainavt.lc.lethalconfig";
        private static void Initialize()
        {
            //TODO merge many variables into one as they are only used to add a config item 
            //did that and DAMN it looks TERRIBLE

            //Skip auto-generating a config
            LethalConfigManager.SkipAutoGen();

            //Variables to be used
            BoolCheckBoxConfigItem entryBoolCheckBox;
            TextInputFieldConfigItem entryTextInputField;
            EnumDropDownConfigItem<CenteredIconSettings> entryIconDropDownConfigItem;

            //Display Settings
            entryTextInputField = new TextInputFieldConfigItem(MeterColor, new TextInputFieldOptions { CharacterLimit = 9, RequiresRestart = false });
            LethalConfigManager.AddConfigItem(entryTextInputField);
            entryBoolCheckBox = new BoolCheckBoxConfigItem(useAccurateDisplay, false);
            LethalConfigManager.AddConfigItem(entryBoolCheckBox);
            entryBoolCheckBox = new BoolCheckBoxConfigItem(enableReverse, false);
            LethalConfigManager.AddConfigItem(entryBoolCheckBox);
            entryBoolCheckBox = new BoolCheckBoxConfigItem(alwaysFull, false);
            LethalConfigManager.AddConfigItem(entryBoolCheckBox);
            entryIconDropDownConfigItem = new EnumDropDownConfigItem<CenteredIconSettings>(iconAlwaysCentered, false);
            LethalConfigManager.AddConfigItem(entryIconDropDownConfigItem);
            //Mod Compatibility Settings
            entryBoolCheckBox = new BoolCheckBoxConfigItem(Compat.LCCrouchHUD, false);
            LethalConfigManager.AddConfigItem(entryBoolCheckBox);
            entryBoolCheckBox = new BoolCheckBoxConfigItem(Compat.An0nPatches, false);
            LethalConfigManager.AddConfigItem(entryBoolCheckBox);
            entryBoolCheckBox = new BoolCheckBoxConfigItem(Compat.EladsHUD, false);
            LethalConfigManager.AddConfigItem(entryBoolCheckBox);
            entryBoolCheckBox = new BoolCheckBoxConfigItem(Compat.GeneralImprovements, false);
            LethalConfigManager.AddConfigItem(entryBoolCheckBox);
            entryBoolCheckBox = new BoolCheckBoxConfigItem(Compat.HealthMetrics, false);
            LethalConfigManager.AddConfigItem(entryBoolCheckBox);
            entryBoolCheckBox = new BoolCheckBoxConfigItem(Compat.DamageMetrics, false);
            LethalConfigManager.AddConfigItem(entryBoolCheckBox);
            entryBoolCheckBox = new BoolCheckBoxConfigItem(Compat.LethalCompanyVR, true); //NOT FINISHED YET (did i never fully implement this? damn)
            LethalConfigManager.AddConfigItem(entryBoolCheckBox);
            entryBoolCheckBox = new BoolCheckBoxConfigItem(Compat.InfectedCompany, false);
            LethalConfigManager.AddConfigItem(entryBoolCheckBox);
            entryBoolCheckBox = new BoolCheckBoxConfigItem(Compat.InfectedCompany_InfectedOnly, false);
            LethalConfigManager.AddConfigItem(entryBoolCheckBox);
            entryBoolCheckBox = new BoolCheckBoxConfigItem(Compat.ShyHUD, false);
            LethalConfigManager.AddConfigItem(entryBoolCheckBox);

            LethalConfigManager.SetModDescription("Adds an insanity meter to the hud in vanilla style");

            Initialise.Logger.LogDebug("Added entries to LethalConfig");

            MeterColor.SettingChanged += FixColor;
            alwaysFull.SettingChanged += SettingChanged;
            enableReverse.SettingChanged += SettingChanged;
            iconAlwaysCentered.SettingChanged += SettingChanged;
        }
    }
}
