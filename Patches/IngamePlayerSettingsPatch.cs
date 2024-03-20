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

        private static bool firstTimeFrameCap = false;
        private static bool firstTimeDiscardChanges = false;

        public static void EnableFirstTime()
        {
            firstTimeFrameCap = true;
            firstTimeDiscardChanges = true;
        }

        private static int AdjustFramerateToVanilla(int value)
        {
            switch (value)
            {
                case 0:
                    return 0; //Enable VSync
                case 1:
                    return 501; //Enable Unlimited
                case 2:
                    return 144;
                case 3:
                    return 120;
                case 4:
                    return 60;
                case 5:
                    return 30;
                default:
                    return 60;
            }
        }

        [HarmonyPatch("SetFramerateCap")]
        [HarmonyPrefix]
        private static bool RewriteSetFramerateCap(IngamePlayerSettings __instance, int value)
        {
            if (firstTimeFrameCap)
            {
                Initialise.ModSettings.FramerateLimit.Value = AdjustFramerateToVanilla(__instance.settings.framerateCapIndex);
                Initialise.modLogger.LogInfo($"first time , frameratecap {Initialise.ModSettings.FramerateLimit.Value}");
                UnsavedLimit = Initialise.ModSettings.FramerateLimit.Value;
        }
            else
            {
                Initialise.ModSettings.FramerateLimit.Value = UnsavedLimit;
                Initialise.modLogger.LogInfo($"not first time , frameratecap {Initialise.ModSettings.FramerateLimit.Value}");
            }
            int cap = Initialise.ModSettings.FramerateLimit.Value;

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
                if (cap >= 501)
                {
                    Application.targetFrameRate = -1; // uncap framerate if above 500
                    value = 1;
                    __instance.settings.framerateCapIndex = value; //Set vanilla setting to Unlimited
                }
                else if (cap <= 500 && cap >= 144)
                {
                    Application.targetFrameRate = cap;
                    value = 2;
                    __instance.settings.framerateCapIndex = value; //set to 144 because it is the closest
                }
                else if (cap < 144 && cap >= 120)
                {
                    Application.targetFrameRate = cap;
                    value = 3;
                    __instance.settings.framerateCapIndex = value; //set to 120
                }
                else if (cap < 120 && cap >= 60)
                {
                    Application.targetFrameRate = cap;
                    value = 4;
                    __instance.settings.framerateCapIndex = value; //set to 60
                }
                else if (cap < 60)
                {
                    Application.targetFrameRate = cap;
                    value = 5;
                    __instance.settings.framerateCapIndex = value; //set to 30
                }
            }
            __instance.unsavedSettings.framerateCapIndex = value;

            if (firstTimeFrameCap) { firstTimeFrameCap = false; }
            return false;
        }

        [HarmonyPatch("DiscardChangedSettings")]
        [HarmonyPrefix]
        private static void UpdateSliderValue(IngamePlayerSettings __instance)
        {
            if (firstTimeDiscardChanges)
            {
                SliderHandler.ignoreSliderAudio = true;
                Initialise.ModSettings.FramerateLimit.Value = AdjustFramerateToVanilla(__instance.settings.framerateCapIndex);
                SliderHandler.ignoreSliderAudio = false;
                Initialise.modLogger.LogInfo($"Converting vanilla framerate cap into the modded framerate cap: {Initialise.ModSettings.FramerateLimit.Value}");
            }
            if (Initialise.ModSettings.FramerateLimit.Value > 500)
            {
                SliderHandler.sceneSlider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = "Frame rate cap: Unlimited";
            }
            else if (Initialise.ModSettings.FramerateLimit.Value == 0)
            {
                SliderHandler.sceneSlider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = "Frame rate cap: VSync";
            }
            else
            {
                SliderHandler.sceneSlider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = $"Frame rate cap: {Initialise.ModSettings.FramerateLimit.Value}";
            }

            if (firstTimeDiscardChanges)
            {
                SliderHandler.ignoreSliderAudio = true;
            }

            SliderHandler.sceneSlider.transform.Find("Slider").GetComponent<Slider>().value = Initialise.ModSettings.FramerateLimit.Value;
            if (firstTimeDiscardChanges)
            {
                SliderHandler.ignoreSliderAudio = false;
                firstTimeDiscardChanges = false;
            }
            UnsavedLimit = Initialise.ModSettings.FramerateLimit.Value;
        }

        [HarmonyPatch("ResetSettingsToDefault")]
        [HarmonyPostfix]
        private static void ResetValues()
        {
            Initialise.ModSettings.FramerateLimit.Value = (int)Initialise.ModSettings.FramerateLimit.DefaultValue;
            SliderHandler.sceneSlider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = $"Frame rate cap: {Initialise.ModSettings.FramerateLimit.Value}";
            SliderHandler.sceneSlider.transform.Find("Slider").GetComponent<Slider>().value = Initialise.ModSettings.FramerateLimit.Value;
        }
    }
}
