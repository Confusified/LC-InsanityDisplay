using InsanityDisplay.Config;
using UnityEngine;
using UnityEngine.UI;
using InsanityDisplay.ModCompatibility;
using GameNetcodeStuff;
using static InsanityDisplay.ModCompatibility.CompatibilityList;
using static InsanityDisplay.ModCompatibility.InfectedCompanyCompatibility;
using DunGen;
using TMPro;
using System;

namespace InsanityDisplay.UI
{
    public class UIHandler
    {
        public static Vector3 localPositionOffset = new Vector3(-3.4f, 3.7f, 0f); //-271.076 102.6285 -13.0663 = normal
        private static Vector3 localScale = new Vector3(1.4f, 1.4f, 1.4f); //SprintMeter scale is 1.6892 1.6892 1.6892
        public static Vector3 selfLocalPositionOffset = new Vector3(-6.8f, 4f, 0f); // -272.7607 112.2663 -14.2212 = normal    -279.5677f, 116.2748f, -14.2174f

        private const float accurate_MinValue = 0.298f; //Becomes visible starting 0.298f
        private const float accurate_MaxValue = 0.910f; //No visible changes after this value
        private static float lastInsanityValue;
        private static Color oldColorValue;

        public static GameObject InsanityMeter;

        public static Image InsanityImage;
        public static GameObject TopLeftCornerHUD;

        private static PlayerControllerB localPlayer;

        public static GameObject vanillaSprintMeter;

        public static void CreateInScene()
        {
            if (InsanityMeter != null) { return; } //Already exists

            localPlayer = GameNetworkManager.Instance.localPlayerController;
            vanillaSprintMeter = localPlayer.sprintMeterUI.gameObject;

            InsanityMeter = GameObject.Instantiate(vanillaSprintMeter);
            InsanityMeter.name = "InsanityMeter";

            TopLeftCornerHUD = vanillaSprintMeter.transform.parent.gameObject;

            Transform meterTransform = InsanityMeter.transform;
            meterTransform.SetParent(TopLeftCornerHUD.transform);
            meterTransform.SetAsFirstSibling();
            meterTransform.localPosition = vanillaSprintMeter.transform.localPosition + localPositionOffset;
            meterTransform.localScale = localScale;

            InsanityImage = InsanityMeter.GetComponent<Image>();

            UpdateMeter(imageMeter: InsanityImage);

            InsanityMeter.SetActive(ConfigSettings.ModEnabled.Value);

            GameObject selfObject = TopLeftCornerHUD.transform.Find("Self").gameObject; //Doesn't seem to have a simple variable attached to it
            selfObject.transform.localPosition += selfLocalPositionOffset;

            GameObject selfRedObject = HUDManager.Instance.selfRedCanvasGroup.gameObject;
            selfRedObject.transform.localPosition = selfObject.transform.localPosition;
            Initialise.modLogger.LogInfo("Created insanity meter succesfully");
            EnableCompatibilities();
            return;
        }

        private static void EnableCompatibility(bool condition, Action compatibilityAction, string modName)
        {
            if (condition)
            {
                compatibilityAction?.Invoke();
                Initialise.modLogger.LogDebug($"Enabled {modName} compat");
            }
        }

        private static void EnableCompatibilities()
        {
            Initialise.modLogger.LogInfo("Enabling allowed compatibilities");
            EnableCompatibility(ModInstalled.LCCrouchHUD && ConfigSettings.Compat.LCCrouchHUD.Value, LCCrouchHUDCompatibility.MoveCrouchHUD, "LCCrouchHUD");
            EnableCompatibility(
                ModInstalled.EladsHUD && ConfigSettings.Compat.EladsHUD.Value && (!ModInstalled.LethalCompanyVR || (ModInstalled.LethalCompanyVR && !ConfigSettings.Compat.LethalCompanyVR.Value)),
                () =>
                {
                    GameObject.Destroy(InsanityMeter);
                    InsanityMeter = null;
                    EladsHUDCompatibility.EditEladsHUD();
                },
                "EladsHUD");
            EnableCompatibility(ModInstalled.An0nPatches && ConfigSettings.Compat.An0nPatches.Value, An0nPatchesCompatibility.MoveTextHUD, "An0nPatches");
            EnableCompatibility(ModInstalled.GeneralImprovements && ConfigSettings.Compat.GeneralImprovements.Value, GeneralImprovementsCompatibility.MoveHPHud, "GeneralImprovements");
            EnableCompatibility(ModInstalled.HealthMetrics && ConfigSettings.Compat.HealthMetrics.Value, () => HealthMetrics_DamageMetricsCompatibility.MoveDisplay(true), "HealthMetrics");
            EnableCompatibility(ModInstalled.DamageMetrics && ConfigSettings.Compat.DamageMetrics.Value, () => HealthMetrics_DamageMetricsCompatibility.MoveDisplay(false), "DamageMetrics");
            EnableCompatibility(ModInstalled.LethalCompanyVR && ConfigSettings.Compat.LethalCompanyVR.Value, () => CoroutineHelper.Start(LethalCompanyVRCompatibility.EnableVRCompatibility()), "LethalCompanyVR");
            EnableCompatibility(ModInstalled.InfectedCompany && ConfigSettings.Compat.InfectedCompany.Value, null, "InfectedCompany"); //Handled in StartOfRoundPatch
                                                                                                                                       //    EnableCompatibility(ModInstalled.ShyHUD && ConfigSettings.Compat.ShyHUD.Value, null, "ShyHUD"); //Handled in UIHandler.UpdateColor
        }

        public static void UpdateMeter(Image imageMeter = null, TextMeshProUGUI textMeter = null)
        {
            if (!ConfigSettings.ModEnabled.Value || (imageMeter == null && textMeter == null)) { return; } //Meter isn't visible so don't update at all
            bool enableReverse = ConfigSettings.enableReverse.Value;
            bool alwaysFull = ConfigSettings.alwaysFull.Value;
            bool useAccurate = ConfigSettings.useAccurateDisplay.Value;
            bool gameHasStarted = GameNetworkManager.Instance.gameHasStarted;
            localPlayer = GameNetworkManager.Instance.localPlayerController;

            if (localPlayer == null || (!useAccurate && !alwaysFull && !enableReverse && !gameHasStarted) && ((imageMeter != null && imageMeter.fillAmount != 0) || (textMeter != null && textMeter.text != "0%"))) { SetValueForCorrectType(imageMeter, textMeter, 0); return; } //if player doesnt exist or in orbit (with certain settings disabled) set to 0
            if (alwaysFull || (!useAccurate && enableReverse && !gameHasStarted) && ((imageMeter != null && imageMeter.fillAmount != 1) || (textMeter != null && textMeter.text != "100%"))) { SetValueForCorrectType(imageMeter, textMeter, 1); return; } //if alwaysfull enabled or in orbit and reverse enabled set to 1

            float finalInsanityValue = 0;
            bool compatibleEladsHUD = ModInstalled.EladsHUD && ConfigSettings.Compat.EladsHUD.Value;

            if (ConfigSettings.Compat.InfectedCompany.Value && ModInstalled.InfectedCompany && modInsanitySlider != null)
            {
                float modInsanityValue = modInsanitySlider.value / modInsanitySlider.maxValue;

                if (!useAccurate || compatibleEladsHUD)
                {
                    finalInsanityValue = enableReverse ? 1 - modInsanityValue : modInsanityValue;
                }
                else
                {
                    finalInsanityValue = modInsanityValue * (accurate_MaxValue - accurate_MinValue);
                    finalInsanityValue = enableReverse ? accurate_MaxValue - finalInsanityValue : finalInsanityValue + accurate_MinValue;
                }

                SetValueForCorrectType(imageMeter, textMeter, finalInsanityValue);
                return;
            }

            float insanityValue = localPlayer.insanityLevel / localPlayer.maxInsanityLevel;

            if (useAccurate && (!ModInstalled.EladsHUD || !compatibleEladsHUD))
            {
                finalInsanityValue = insanityValue * (accurate_MaxValue - accurate_MinValue);
                finalInsanityValue = enableReverse ? accurate_MaxValue - finalInsanityValue : finalInsanityValue + accurate_MinValue;
            }
            else
            {
                finalInsanityValue = enableReverse ? 1 - insanityValue : insanityValue;
            }

            SetValueForCorrectType(imageMeter, textMeter, finalInsanityValue);
        }

        private static void SetValueForCorrectType(Image imageMeter, TextMeshProUGUI textMeter, float insanityValue)
        {
            if (!ConfigSettings.MeterColor.Value.StartsWith("#")) { ConfigSettings.MeterColor.Value = $"#{ConfigSettings.MeterColor.Value}"; }
            ColorUtility.TryParseHtmlString(ConfigSettings.MeterColor.Value, out Color meterColor);

            if (textMeter != null) //player is using elad's hud
            {
                string textValue = $"{Math.Floor(insanityValue * 100)}%";
                if (textMeter.text != textValue) //only update if text isn't the same
                {
                    textMeter.text = textValue;
                }
            }
            UpdateColor(textMeter, meterColor, insanityValue);

            if (imageMeter != null) //player isn't using elad's hud
            {
                if (imageMeter.fillAmount != insanityValue) //only update if fill amount isn't the same
                {
                    imageMeter.fillAmount = insanityValue;
                }
            }
            UpdateColor(imageMeter, meterColor, insanityValue);

            if (lastInsanityValue != insanityValue)
            {
                lastInsanityValue = insanityValue;
            }

            if (oldColorValue.CompareRGB(meterColor) || oldColorValue.a != meterColor.a)
            {
                oldColorValue = meterColor;
            }
        }

        private static void UpdateColor<T>(T meter, Color meterColor, float insanityValue) where T : Graphic
        {
            if (meter == null) { return; } //if it doesn't exist

            bool usingShyHUDCompat = false; //ModInstalled.ShyHUD && ConfigSettings.Compat.ShyHUD.Value; //currently this is so scuffed so i'm disabling it for now
            if (usingShyHUDCompat) //if using ShyHUD compatibility
            {
                if ((oldColorValue != meterColor && lastInsanityValue != insanityValue) || lastInsanityValue != insanityValue || !oldColorValue.CompareRGB(meterColor)) //if alpha and insanity changed or insanity changed or the colour has changed
                {
                    bool useAlwaysFull = ConfigSettings.alwaysFull.Value;
                    bool useAccurate = ConfigSettings.useAccurateDisplay.Value;
                    //If bar is full or empty and always full is not enabled
                    if ((((insanityValue >= accurate_MaxValue && useAccurate) || insanityValue >= 1) || ((insanityValue <= accurate_MinValue && useAccurate) || insanityValue <= 0)) && !useAlwaysFull)
                    {
                        meter.CrossFadeAlpha(0f, 5f, false); //Vanish the meter
                    }
                    else if (meter.color.a != meterColor.a)
                    {
                        meter.CrossFadeAlpha(meterColor.a, .5f, false); //Unvanish the meter (up to whatever the user has defined for transparency)
                    }
                    meterColor.a = meter.color.a; //Match the transparency with the fade effect

                }
            }

            if (meter.color == meterColor) { return; } //already the same colour
            meter.color = meterColor;
        }

        /* 
         * Issues with ShyHUD compat:
         * if toggled mid game when the meter is invisible it won't show up
         * if starting a fade it can't be cancelled
         * idk how but my meter just never became visible at some point
         * i'm losing my mind
         */
    }
}