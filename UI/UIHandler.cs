using InsanityDisplay.Config;
using UnityEngine;
using UnityEngine.UI;
using InsanityDisplay.ModCompatibility;
using GameNetcodeStuff;
using static InsanityDisplay.ModCompatibility.CompatibilityList;
using static InsanityDisplay.ModCompatibility.InfectedCompanyCompatibility;
using DunGen;

namespace InsanityDisplay.UI
{
    public class UIHandler
    {
        public static Vector3 localPositionOffset = new Vector3(-3.4f, 3.7f, 0f); //-271.076 102.6285 -13.0663 = normal
        private static Vector3 localScale = new Vector3(1.4f, 1.4f, 1.4f); //SprintMeter scale is 1.6892 1.6892 1.6892
        public static Vector3 selfLocalPositionOffset = new Vector3(-6.8f, 4f, 0f); // -272.7607 112.2663 -14.2212 = normal    -279.5677f, 116.2748f, -14.2174f

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
                ColorUtility.TryParseHtmlString(ConfigSettings.MeterColor.Value, out Color meterColor);
                if (meterColor == null) { ColorUtility.TryParseHtmlString("#" + (string)ConfigSettings.MeterColor.DefaultValue, out Color defaultColor); Initialise.modLogger.LogError("Unable to find the color for the meter, setting color to default"); meterColor = defaultColor; }
                InsanityImage.color = meterColor + new Color(0, 0, 0, 1); //Always set to completely visible regardless of config
                InsanityImage.fillAmount = GetFillAmount();
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
        //this needs some refactoring lol  this has to be killing performance
        public static float GetFillAmount()
        {
            if (GameNetworkManager.Instance.localPlayerController == null) { return 0; } //Avoid errors
            localPlayer = GameNetworkManager.Instance.localPlayerController;

            if (ConfigSettings.alwaysFull.Value) { return 1; }

            if (ConfigSettings.Compat.InfectedCompany.Value && ModInstalled.InfectedCompany && modInsanitySlider != null)
            {
                if (!ConfigSettings.useAccurateDisplay.Value) //NOT using accurate meter
                {
                    if (ConfigSettings.enableReverse.Value)
                    {
                        return 1 - (modInsanitySlider.value / modInsanitySlider.maxValue);
                    }

                    return (modInsanitySlider.value / modInsanitySlider.maxValue);
                }
                else
                {
                    if (ConfigSettings.enableReverse.Value)

                    {
                        return accurate_MaxValue - ((modInsanitySlider.value / modInsanitySlider.maxValue) * (accurate_MaxValue - accurate_MinValue));
                    }

                    return accurate_MinValue + ((modInsanitySlider.value / modInsanitySlider.maxValue) * (accurate_MaxValue - accurate_MinValue));
                }
            }

            if (ConfigSettings.useAccurateDisplay.Value && (!ModInstalled.EladsHUD || (ModInstalled.EladsHUD && !ConfigSettings.Compat.EladsHUD.Value))) //Start from ~0.2 to ~0.91
            {
                if (ConfigSettings.enableReverse.Value) //Start from ~0.91 to ~0.2 instead

                {
                    return accurate_MaxValue - ((localPlayer.insanityLevel / localPlayer.maxInsanityLevel) * (accurate_MaxValue - accurate_MinValue));
                }

                return accurate_MinValue + ((localPlayer.insanityLevel / localPlayer.maxInsanityLevel) * (accurate_MaxValue - accurate_MinValue));
            }
            else
            {

                if (ConfigSettings.enableReverse.Value) //Start from 100 to 0 instead

                {
                    return 1 - (localPlayer.insanityLevel / localPlayer.maxInsanityLevel);
                }

                return (localPlayer.insanityLevel / localPlayer.maxInsanityLevel);
            }
        }
    }
}