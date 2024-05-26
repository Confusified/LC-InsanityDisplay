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
        private static Color fullVisibility = new Color(0, 0, 0, 1);

        private const float accurate_MinValue = 0.2978f; //Becomes visible starting 0.298f
        private const float accurate_MaxValue = 0.9101f; //No visible changes after this value

        public static GameObject Memory_InsanityMeter;
        public static GameObject InsanityMeter;

        public static Image InsanityImage;

        private static PlayerControllerB localPlayer;

        public static GameObject vanillaSprintMeter;

        public static void CreateInMemory()
        {
            if (Memory_InsanityMeter != null) { CreateInScene(); return; } //It already exists
            if (CompatibilityList.ModInstalled.EladsHUD) { EnableCompatibilities(); return; }
            Memory_InsanityMeter = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner/SprintMeter").gameObject;
            Memory_InsanityMeter = GameObject.Instantiate(Memory_InsanityMeter);
            GameObject.DontDestroyOnLoad(Memory_InsanityMeter);
            CreateInScene();
            return;
        }

        private static void CreateInScene()
        {
            if (Memory_InsanityMeter == null) { CreateInMemory(); return; }
            if (InsanityMeter != null) { return; } //Already exists

            InsanityMeter = GameObject.Instantiate(Memory_InsanityMeter);
            InsanityMeter.name = "InsanityMeter";

            GameObject TopLeftCornerHUD = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner").gameObject;

            vanillaSprintMeter = TopLeftCornerHUD.transform.Find("SprintMeter").gameObject;

            Transform meterTransform = InsanityMeter.transform;
            meterTransform.SetParent(TopLeftCornerHUD.transform);
            meterTransform.SetAsFirstSibling();
            meterTransform.localPosition = vanillaSprintMeter.transform.localPosition + localPositionOffset;
            meterTransform.localScale = localScale;

            InsanityImage = InsanityMeter.GetComponent<Image>();
            try
            {
                UpdateMeter(imageMeter: InsanityImage);
            }
            catch
            {
                Initialise.modLogger.LogError("Error while setting insanity meter's color and fill, most likely could not find the meter\nThe mod will attempt to automatically resolve this");
            }

            InsanityMeter.SetActive(ConfigSettings.ModEnabled.Value);

            GameObject selfObject = TopLeftCornerHUD.transform.Find("Self").gameObject;
            selfObject.transform.localPosition += selfLocalPositionOffset;

            GameObject selfRedObject = TopLeftCornerHUD.transform.Find("SelfRed").gameObject;
            selfRedObject.transform.localPosition = selfObject.transform.localPosition;
            Initialise.modLogger.LogInfo("Created insanity meter succesfully");
            EnableCompatibilities();
            return;
        }

        private static void EnableCompatibilities()
        {
            Initialise.modLogger.LogInfo("Enabling allowed compatibilities");
            if (ModInstalled.LCCrouchHUD && ConfigSettings.Compat.LCCrouchHUD.Value)
            {
                LCCrouchHUDCompatibility.MoveCrouchHUD();
                Initialise.modLogger.LogDebug("Enabled LCCrouchHUD compat");
            }
            if (ModInstalled.EladsHUD && ConfigSettings.Compat.EladsHUD.Value && (!ModInstalled.LethalCompanyVR || ModInstalled.LethalCompanyVR && !ConfigSettings.Compat.LethalCompanyVR.Value)) //only do this if LCVR isn't also there
            {
                GameObject.Destroy(Memory_InsanityMeter);
                GameObject.Destroy(InsanityMeter);
                Memory_InsanityMeter = null;
                InsanityMeter = null;
                EladsHUDCompatibility.EditEladsHUD();
                Initialise.modLogger.LogDebug("Enabled EladsHUD compat");
            }
            if (ModInstalled.An0nPatches && ConfigSettings.Compat.An0nPatches.Value)
            {
                An0nPatchesCompatibility.MoveTextHUD();
                Initialise.modLogger.LogDebug("Enabled An0nPatches compat");
            }
            if (ModInstalled.GeneralImprovements && ConfigSettings.Compat.GeneralImprovements.Value)
            {
                GeneralImprovementsCompatibility.MoveHPHud();
                Initialise.modLogger.LogDebug("Enabled GeneralImprovements compat");
            }
            if (ModInstalled.HealthMetrics && ConfigSettings.Compat.HealthMetrics.Value)
            {
                HealthMetricsCompatibility.MoveHealthHUD();
                Initialise.modLogger.LogDebug("Enabled HealthMetrics compat");
            }
            if (ModInstalled.DamageMetrics && ConfigSettings.Compat.DamageMetrics.Value)
            {
                DamageMetricsCompatibility.MoveDamageHUD();
                Initialise.modLogger.LogDebug("Enabled DamageMetrics compat");
            }
            if (ModInstalled.LethalCompanyVR && ConfigSettings.Compat.LethalCompanyVR.Value)
            {
                CoroutineHelper.Start(LethalCompanyVRCompatibility.EnableVRCompatibility());
                Initialise.modLogger.LogDebug("Enabled LethalCompanyVR compat");
            }
            if (ModInstalled.InfectedCompany && ConfigSettings.Compat.InfectedCompany.Value)
            {
                //Handled in StartOfRoundPatch
                Initialise.modLogger.LogDebug("Enabled InfectedCompany compat");
            }
        }

        public static void UpdateMeter(Image imageMeter = null, TextMeshProUGUI textMeter = null)
        {
            if (GameNetworkManager.Instance.localPlayerController == null || (!ConfigSettings.alwaysFull.Value && !ConfigSettings.enableReverse.Value && !GameNetworkManager.Instance.gameHasStarted) && (imageMeter != null && imageMeter.fillAmount != 0 || textMeter != null && textMeter.text != "100%")) { SetValueForCorrectType(imageMeter, textMeter, 0); return; } //if player doesnt exist or in orbit (with certain settings disabled) set to 0
            if (ConfigSettings.alwaysFull.Value || (ConfigSettings.enableReverse.Value && !GameNetworkManager.Instance.gameHasStarted) && imageMeter.fillAmount != 1) { SetValueForCorrectType(imageMeter, textMeter, 1); return; } //if alwaysfull enabled or in orbit and reverse enabled set to 1

            localPlayer = GameNetworkManager.Instance.localPlayerController;

            float finalInsanityValue = 0;
            if (ConfigSettings.Compat.InfectedCompany.Value && ModInstalled.InfectedCompany && modInsanitySlider != null) //if using infectedcompany's compat (and it all works)
            {
                float modInsanityValue = modInsanitySlider.value / modInsanitySlider.maxValue;
                if (!ConfigSettings.useAccurateDisplay.Value) //NOT using accurate meter
                {

                    if (ConfigSettings.enableReverse.Value)
                    {
                        finalInsanityValue = 1 - modInsanityValue;
                    }
                    else
                    {
                        finalInsanityValue = modInsanityValue;
                    }

                    SetValueForCorrectType(imageMeter, textMeter, finalInsanityValue);
                    return;
                }
                else
                {
                    finalInsanityValue = modInsanityValue * (accurate_MaxValue - accurate_MinValue);
                    if (ConfigSettings.enableReverse.Value)

                    {
                        finalInsanityValue = accurate_MaxValue - finalInsanityValue;
                    }
                    else
                    {
                        finalInsanityValue += accurate_MinValue;
                    }

                    SetValueForCorrectType(imageMeter, textMeter, finalInsanityValue);
                    return;
                }
            }

            float insanityValue = localPlayer.insanityLevel / localPlayer.maxInsanityLevel;
            if (ConfigSettings.useAccurateDisplay.Value && (!ModInstalled.EladsHUD || (ModInstalled.EladsHUD && !ConfigSettings.Compat.EladsHUD.Value))) //Start from ~0.2 to ~0.91
            {
                finalInsanityValue = insanityValue * (accurate_MaxValue - accurate_MinValue);
                if (ConfigSettings.enableReverse.Value) //Start from ~0.91 to ~0.2 instead

                {
                    finalInsanityValue = accurate_MaxValue - finalInsanityValue;
                }
                else
                {
                    finalInsanityValue += accurate_MinValue;
                }

                SetValueForCorrectType(imageMeter, textMeter, finalInsanityValue);
                return;
            }
            else
            {
                finalInsanityValue = insanityValue;
                if (ConfigSettings.enableReverse.Value) //Start from 100 to 0 instead

                {
                    finalInsanityValue = 1 - finalInsanityValue;
                }
                SetValueForCorrectType(imageMeter, textMeter, finalInsanityValue);
                return;
            }
        }


        private static void SetValueForCorrectType(Image imageMeter, TextMeshProUGUI textMeter, float insanityValue)
        {
            if (ConfigSettings.MeterColor.Value.StartsWith("#")) { ConfigSettings.MeterColor.Value.Substring(1); }
            ColorUtility.TryParseHtmlString(ConfigSettings.MeterColor.Value, out Color meterColor);
            Color finalMeterColor = meterColor + fullVisibility;
            if (imageMeter == null) //assume player is using Elad's hud (probably gonna cause errors in the future but that's a problem for future me)
            {
                string textValue = $"{Math.Floor(insanityValue * 100)}%";
                if (textMeter.text != textValue) //only update if text isn't the same
                {
                    textMeter.text = textValue;
                }
                if (textMeter.color != finalMeterColor)
                {
                    textMeter.color = finalMeterColor;
                }
                return;
            }
            if (textMeter == null) //assume player isn't using elad's hud 
            {
                if (imageMeter.fillAmount != insanityValue) //only update if fill amount isn't the same
                {
                    imageMeter.fillAmount = insanityValue;
                }
                if (textMeter.color != finalMeterColor)
                {
                    textMeter.color = finalMeterColor;
                }
                return;
            }
        }
    }
}