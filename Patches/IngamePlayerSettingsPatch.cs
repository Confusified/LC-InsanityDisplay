using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FramerateSlider.Patches
{
    [HarmonyPatch(typeof(IngamePlayerSettings))]
    public class IngamePlayerSettingsPatch
    {
        public static int UnsavedLimit = Initialise.ModSettings.FramerateLimit.Value; //so it doesn't mess up the config whenever you launch the game
        static GameObject Slider;

        [HarmonyPatch("SetFramerateCap")]
        [HarmonyPrefix]
        public static bool RewriteSetFramerateCap(IngamePlayerSettings __instance, int value)
        {
            Initialise.ModSettings.FramerateLimit.Value = UnsavedLimit;
            int cap = (int)Initialise.ModSettings.FramerateLimit.Value;

            if (cap <= 0)
            {
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1;
                value = 0;
                __instance.settings.framerateCapIndex = value; //set vanilla to VSync to make it more seamless when removing the mod
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                if (cap > 500)
                {
                    Application.targetFrameRate = -1; // uncap framerate if above 500
                    value = 1;
                    __instance.settings.framerateCapIndex = value; //Set vanilla setting to Unlimited
                }
                else if (cap <= 500)
                {
                    Application.targetFrameRate = cap;
                    value = 2;
                    __instance.settings.framerateCapIndex = value; //set to 144 because it is the closest
                }
                else if (cap <144)
                {
                    Application.targetFrameRate = cap;
                    value = 3;
                    __instance.settings.framerateCapIndex = value; //set to 120
                }
                else if (cap < 120)
                {
                    Application.targetFrameRate = cap;
                    value = 4;
                    __instance.settings.framerateCapIndex = value; //set to 60
                }
                else
                {
                    Application.targetFrameRate = cap;
                    value = 5;
                    __instance.settings.framerateCapIndex = value; //set to 30
                }
            }
            __instance.unsavedSettings.framerateCapIndex = value;
            return false;
        }

        [HarmonyPatch("DiscardChangedSettings")]
        [HarmonyPrefix]
        public static void UpdateSliderValue()
        {
            Slider = SliderHandler.sceneSlider;
            if (Initialise.ModSettings.FramerateLimit.Value > 500)
            {
                Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = "Frame rate cap: Unlimited";
            }
            else if (Initialise.ModSettings.FramerateLimit.Value == 0)
            {
                Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = "Frame rate cap: VSync";
            }
            else
            {
                Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = $"Frame rate cap: {Initialise.ModSettings.FramerateLimit.Value}";
            }
            Slider.transform.Find("Slider").GetComponent<Slider>().value = Initialise.ModSettings.FramerateLimit.Value;
            UnsavedLimit = Initialise.ModSettings.FramerateLimit.Value;
            Initialise.modLogger.LogInfo("Discarded any unsaved changes to the slider");
        }

        [HarmonyPatch("ResetSettingsToDefault")]
        [HarmonyPostfix]
        public static void ResetValues()
        {
            Slider = SliderHandler.sceneSlider;
            Initialise.ModSettings.FramerateLimit.Value = (int)Initialise.ModSettings.FramerateLimit.DefaultValue;
            Slider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = $"Frame rate cap: {Initialise.ModSettings.FramerateLimit.Value}";
            Slider.transform.Find("Slider").GetComponent<Slider>().value = Initialise.ModSettings.FramerateLimit.Value;
        }
    }
}
