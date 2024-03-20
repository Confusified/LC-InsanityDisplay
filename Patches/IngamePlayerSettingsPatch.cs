using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameCapSlider.Patches
{
    [HarmonyPatch(typeof(IngamePlayerSettings))]
    public class IngamePlayerSettingsPatch
    {
        public static int UnsavedLimit = Initialize.ModSettings.FramerateLimit.Value; //so it doesn't mess up the config whenever you launch the game

        [HarmonyPatch("SetFramerateCap")]
        [HarmonyPrefix]
        public static bool RewriteSetFramerateCap(IngamePlayerSettings __instance, int value)
        {
            Initialize.ModSettings.FramerateLimit.Value = UnsavedLimit;
            int cap = (int)Initialize.ModSettings.FramerateLimit.Value;

            if (cap <= 0)
            {
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1;
                value = 0;
                __instance.settings.framerateCapIndex = value; //set vanilla to VSync to make it more seamless when removing the mod
                Initialize.modLogger.LogInfo("Framerate cap was set to VSync");
                return false;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                if (cap > 500)
                {
                    Application.targetFrameRate = -1; // uncap framerate if above 500
                    value = 1;
                    __instance.settings.framerateCapIndex = value; //Set vanilla setting to Unlimited
                    Initialize.modLogger.LogInfo("Framerate cap was set above 500, setting it to -1 (Unlimited)");
                    return false;
                }
                else
                {
                    Application.targetFrameRate = cap;
                    value = 4;
                    __instance.settings.framerateCapIndex = value; //set to 60 because idk!!!!!!!! (maybe in the future i'll change it to set it to whichever is closest to the selected number (a fix for the future i suppose)
                    return false;
                }
            }
        }

        [HarmonyPatch("DiscardChangedSettings")]
        [HarmonyPrefix]
        public static void UpdateSliderValue()
        {
            if (SceneManager.GetSceneByName("SampleSceneRelay").isLoaded) //Use QuickMenuManager
            {
                GameObject Slider = QuickMenuManagerPatch.Slider;
            }
            else //Use MenuManager
            {
                GameObject Slider = MenuManagerPatch.Slider;
            }

            if (Initialize.ModSettings.FramerateLimit.Value > 500)
            {
                Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = "Frame rate cap: Unlimited";
            }
            else if (Initialize.ModSettings.FramerateLimit.Value == 0)
            {
                Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = "Frame rate cap: VSync";
            }
            else
            {
                Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = $"Frame rate cap: {Initialize.ModSettings.FramerateLimit.Value}";
            }
            Slider.transform.Find("Slider").GetComponent<Slider>().value = Initialize.ModSettings.FramerateLimit.Value;
            Initialize.modLogger.LogInfo("Reverted any unsaved changes to the slider");
        }

        [HarmonyPatch("ResetSettingsToDefault")]
        [HarmonyPostfix]
        public static void ResetValues()
        {
            if (SceneManager.GetSceneByName("SampleSceneRelay").isLoaded) //Use QuickMenuManager
            {
                GameObject Slider = QuickMenuManagerPatch.Slider;
            }
            else //Use MenuManager
            {
                GameObject Slider = MenuManagerPatch.Slider;
            }
            Initialize.ModSettings.FramerateLimit.Value = (int)Initialize.ModSettings.FramerateLimit.DefaultValue;
            Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = $"Frame rate cap: {Initialize.ModSettings.FramerateLimit.Value}";
            Slider.transform.Find("Slider").GetComponent<Slider>().value = Initialize.ModSettings.FramerateLimit.Value;
        }
    }
}
