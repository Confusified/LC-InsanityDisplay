using HarmonyLib;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static FramerateSlider.Initialise;
using static FramerateSlider.Patches.SliderHandler;

namespace FramerateSlider.Patches
{
    [HarmonyPatch(typeof(IngamePlayerSettings))]
    public class IngamePlayerSettingsPatch
    {
        public static int UnsavedLimit = ModSettings.FramerateLimit.Value; //so it doesn't mess up the config whenever you launch the game
        public static IngamePlayerSettings playerSettingsInstance;


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

        private static int SetCorrespondingFramerate(int cap)
        {
            if (syncFrameCap)
            {
                ModSettings.FramerateLimit.Value = AdjustFramerateToVanilla(playerSettingsInstance.settings.framerateCapIndex);
                UnsavedLimit = ModSettings.FramerateLimit.Value;
            }
            else
            {
                ModSettings.FramerateLimit.Value = UnsavedLimit;
            }

            if (cap <= 0)
            {
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1;
                return 0; //Set vanilla setting to VSync
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                if (cap >= 501)
                {
                    Application.targetFrameRate = -1; // uncap framerate if above 500
                    return 1; //Set vanilla setting to Unlimited
                }
                else if (cap <= 500 && cap >= 144)
                {
                    Application.targetFrameRate = cap;
                    return 2; //set to 144 because it is the closest
                }
                else if (cap < 144 && cap >= 120)
                {
                    Application.targetFrameRate = cap;
                    return 3; //set to 120
                }
                else if (cap < 120 && cap >= 60)
                {
                    Application.targetFrameRate = cap;
                    return 4; //set to 60
                }
                else if (cap < 60 && cap > 0)
                {
                    Application.targetFrameRate = cap;
                    return 5; //set to 30
                }
            }
            return 0; //shouldn't be possible but oh well
        }

        [HarmonyPatch("LoadSettingsFromPrefs")]
        [HarmonyPostfix]
        private static void CheckForConfigDesync(IngamePlayerSettings __instance)
        {
            playerSettingsInstance = __instance;
            if (ModSettings.LastLoggedIndex != playerSettingsInstance.settings.framerateCapIndex)
            {
                syncFrameCap = true;
                syncDiscardChanges = true;
                return;
            }

            syncFrameCap = false;
            syncDiscardChanges = false;
            return;
        }

        [HarmonyPatch("SetFramerateCap")]
        [HarmonyPrefix]
        private static bool RewriteSetFramerateCap()
        {
            int cap = ModSettings.FramerateLimit.Value;
            int value = SetCorrespondingFramerate(cap);

            if (!syncFrameCap)
            {
                playerSettingsInstance.unsavedSettings.framerateCapIndex = value;
                playerSettingsInstance.settings.framerateCapIndex = value;
                syncFrameCap = false;
            }

            ModSettings.LastLoggedIndex = value;
            UpdatePrivateConfig();
            return false;
        }

        [HarmonyPatch("DiscardChangedSettings")]
        [HarmonyPrefix]
        private static void UpdateSliderValue()
        {
            StringBuilder displayText = new StringBuilder("Converting vanilla framerate cap into the modded framerate cap: ");
            if (syncDiscardChanges)
            {
                ignoreSliderAudio = true;
                ModSettings.FramerateLimit.Value = AdjustFramerateToVanilla(playerSettingsInstance.settings.framerateCapIndex);
                ignoreSliderAudio = false;
                displayText.Append(ModSettings.FramerateLimit.Value);
                modLogger.LogInfo(displayText.ToString());
            }

            displayText = new StringBuilder("Frame rate cap: ");
            if (ModSettings.FramerateLimit.Value > 500)
            {
                displayText = displayText.Append("Unlimited");
            }
            else if (ModSettings.FramerateLimit.Value == 0)
            {
                displayText = displayText.Append("VSync");
            }
            else
            {
                displayText = displayText.Append(ModSettings.FramerateLimit.Value);
            }

            sceneSliderText.text = displayText.ToString();

            if (syncDiscardChanges)
            {
                ignoreSliderAudio = true;
            }

            sceneSliderInSlider.GetComponent<Slider>().value = ModSettings.FramerateLimit.Value;
            if (syncDiscardChanges)
            {
                ignoreSliderAudio = false;
                syncDiscardChanges = false;
            }
            ModSettings.LastLoggedIndex = playerSettingsInstance.settings.framerateCapIndex;
            UpdatePrivateConfig();
            UnsavedLimit = ModSettings.FramerateLimit.Value;

            return;
        }

        [HarmonyPatch("SaveChangedSettings")]
        [HarmonyPostfix]
        private static void UpdateOnSave()
        {
            ES3.Save("FPSCap", ModSettings.LastLoggedIndex, "LCGeneralSaveData");
            UpdatePrivateConfig();
            return;
        }

        [HarmonyPatch("ResetSettingsToDefault")]
        [HarmonyPostfix]
        private static void ResetValues()
        {
            ModSettings.FramerateLimit.Value = DefaultConfig.FramerateLimit;
            ModSettings.LastLoggedIndex = DefaultConfig.LastLoggedIndex;
            UnsavedLimit = ModSettings.FramerateLimit.Value;
            UpdatePrivateConfig();

            Application.targetFrameRate = ModSettings.FramerateLimit.Value;
            QualitySettings.vSyncCount = 0;

            ignoreSliderAudio = true;
            sceneSliderText.text = setCorrectText();
            sceneSliderInSlider.GetComponent<Slider>().value = ModSettings.FramerateLimit.Value;
            ignoreSliderAudio = false;

            IngamePlayerSettings.Instance.DiscardChangedSettings(); //To hide the changed settings text when using the thang and the slider isn't already on default
            return;
        }
    }
}
