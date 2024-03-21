using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FramerateSlider.Initialise;

namespace FramerateSlider.Patches
{
    [HarmonyPatch(typeof(IngamePlayerSettings))]
    public class IngamePlayerSettingsPatch
    {
        public static int UnsavedLimit = ModSettings.FramerateLimit.Value; //so it doesn't mess up the config whenever you launch the game

        private static bool syncFrameCap;
        private static bool syncDiscardChanges;
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

        [HarmonyPatch("waitToLoadSettings")]
        [HarmonyPostfix]
        private static void CheckForConfigDesync(IngamePlayerSettings __instance)
        {
            if (ModSettings.LastLoggedIndex.Value != __instance.settings.framerateCapIndex)
            {
                syncFrameCap = true;
                syncDiscardChanges = true;
            }
            else
            {
                syncFrameCap = false;
                syncDiscardChanges = false;
            }
        }

        [HarmonyPatch("SetFramerateCap")]
        [HarmonyPrefix]
        private static bool RewriteSetFramerateCap(IngamePlayerSettings __instance, int value)
        {
            if (syncFrameCap)
            {
                ModSettings.FramerateLimit.Value = AdjustFramerateToVanilla(__instance.settings.framerateCapIndex);
                UnsavedLimit = ModSettings.FramerateLimit.Value;
            }
            else
            {
                ModSettings.FramerateLimit.Value = UnsavedLimit;
            }

            int cap = ModSettings.FramerateLimit.Value;

            if (cap <= 0)
            {
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1;
                value = 0;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                if (cap >= 501)
                {
                    Application.targetFrameRate = -1; // uncap framerate if above 500
                    value = 1; //Set vanilla setting to Unlimited
                }
                else if (cap <= 500 && cap >= 144)
                {
                    Application.targetFrameRate = cap;
                    value = 2; //set to 144 because it is the closest
                }
                else if (cap < 144 && cap >= 120)
                {
                    Application.targetFrameRate = cap;
                    value = 3; //set to 120
                }
                else if (cap < 120 && cap >= 60)
                {
                    Application.targetFrameRate = cap;
                    value = 4; //set to 60
                }
                else if (cap < 60 && cap > 0)
                {
                    Application.targetFrameRate = cap;
                    value = 5; //set to 30
                }
            }

            if (!syncFrameCap)
            {
                __instance.unsavedSettings.framerateCapIndex = value;
                __instance.settings.framerateCapIndex = value;
            }
            else
            {
                syncFrameCap = false;
                __instance.unsavedSettings.framerateCapIndex = value;
                __instance.settings.framerateCapIndex = value;
            }
            ModSettings.LastLoggedIndex.Value = value;
            return false;
        }

        [HarmonyPatch("DiscardChangedSettings")]
        [HarmonyPrefix]
        private static void UpdateSliderValue(IngamePlayerSettings __instance)
        {
            if (syncDiscardChanges)
            {
                SliderHandler.ignoreSliderAudio = true;
                ModSettings.FramerateLimit.Value = AdjustFramerateToVanilla(__instance.settings.framerateCapIndex);
                SliderHandler.ignoreSliderAudio = false;
                modLogger.LogInfo($"Converting vanilla framerate cap into the modded framerate cap: {ModSettings.FramerateLimit.Value}");
            }
            if (ModSettings.FramerateLimit.Value > 500)
            {
                SliderHandler.sceneSlider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = "Frame rate cap: Unlimited";
            }
            else if (ModSettings.FramerateLimit.Value == 0)
            {
                SliderHandler.sceneSlider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = "Frame rate cap: VSync";
            }
            else
            {
                SliderHandler.sceneSlider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = $"Frame rate cap: {ModSettings.FramerateLimit.Value}";
            }

            if (syncDiscardChanges)
            {
                SliderHandler.ignoreSliderAudio = true;
            }

            SliderHandler.sceneSlider.transform.Find("Slider").GetComponent<Slider>().value = ModSettings.FramerateLimit.Value;
            if (syncDiscardChanges)
            {
                SliderHandler.ignoreSliderAudio = false;
                syncDiscardChanges = false;
            }
            ModSettings.LastLoggedIndex.Value = __instance.settings.framerateCapIndex;
            UnsavedLimit = ModSettings.FramerateLimit.Value;
        }

        [HarmonyPatch("SaveChangedSettings")]
        [HarmonyPrefix]
        private static void UpdateOnSave(IngamePlayerSettings __instance)
        {
            __instance.unsavedSettings.framerateCapIndex = ModSettings.LastLoggedIndex.Value;
        }

        [HarmonyPatch("ResetSettingsToDefault")]
        [HarmonyPostfix]
        private static void ResetValues()
        {
            ModSettings.FramerateLimit.Value = (int)ModSettings.FramerateLimit.DefaultValue;
            ModSettings.LastLoggedIndex.Value = 4;
            SliderHandler.sceneSlider.transform.Find("Text (1)").gameObject.GetComponent<TMP_Text>().text = $"Frame rate cap: {ModSettings.FramerateLimit.Value}";
            SliderHandler.sceneSlider.transform.Find("Slider").GetComponent<Slider>().value = ModSettings.FramerateLimit.Value;
        }
    }
}
