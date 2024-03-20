using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using FramerateSlider.Patches;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace FramerateSlider
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Initialise : BaseUnityPlugin
    {
        private const string modGUID = "Confusified.FramerateSlider";
        private const string modName = "Framerate Slider";
        private const string modVersion = "1.0.2";

        private readonly ConfigFile modConfig = new ConfigFile(Utility.CombinePaths(Paths.ConfigPath + "\\" + modGUID.Replace(".", "\\") + ".cfg"), false);
        public class ModSettings
        {
            public static ConfigEntry<bool> ModEnabled;
            public static ConfigEntry<int> FramerateLimit;
        }

        private readonly Harmony _Harmony = new Harmony(modGUID);
        public static ManualLogSource modLogger;
        public void Awake()
        {
            modLogger = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            modLogger = Logger;

            SetDefaultConfigValues();
            if (ModSettings.ModEnabled.Value) { 
                _Harmony.PatchAll(Assembly.GetExecutingAssembly()); 
                modLogger.LogInfo($"{modName} {modVersion} loaded"); 
            }
            else {
                modLogger.LogInfo($"{modName} {modVersion} did nothing, it is disabled in the config");
            }
        }

        private void SetDefaultConfigValues()
        {
            ModSettings.ModEnabled = modConfig.Bind<bool>("Mod Settings", "Enabled", true, "Change the Framerate selector from a dropdown into a slider");
            ModSettings.FramerateLimit = modConfig.Bind<int>("Mod Settings", "FramerateCap", 60, "The maximum amount of frames that your game will render");
        }

        public static string setCorrectText(GameObject Slider)
        {
            IngamePlayerSettingsPatch.UnsavedLimit = (int)Slider.transform.Find("Slider").GetComponent<Slider>().value;
            if ((int)Slider.transform.Find("Slider").GetComponent<Slider>().value > 500)
            {
                return "Frame rate cap: Unlimited";
            }
            else if ((int)Slider.transform.Find("Slider").GetComponent<Slider>().value <= 0)
            {
                return "Frame rate cap: VSync";
            }
            else
            {
                return $"Frame rate cap: {IngamePlayerSettingsPatch.UnsavedLimit}";
            }
        }

        public static void SliderValueChanged(GameObject SettingsPanel,GameObject Slider)
        {
            Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = Initialise.setCorrectText(Slider);
            SettingsPanel.transform.Find("Headers").gameObject.transform.Find("ChangesNotApplied").gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
            if (Slider == MenuManagerPatch.Slider)
            {
                SettingsPanel.transform.Find("BackButton").gameObject.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = "DISCARD"; //In Main Menu
            }
            else
            {
                SettingsPanel.transform.Find("BackButton").gameObject.transform.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = "Discard changes"; //In Quick Menu
            }
            IngamePlayerSettings.Instance.SettingsAudio.PlayOneShot(GameNetworkManager.Instance.buttonTuneSFX);
            IngamePlayerSettings.Instance.changesNotApplied = true;
        }
    }
}
