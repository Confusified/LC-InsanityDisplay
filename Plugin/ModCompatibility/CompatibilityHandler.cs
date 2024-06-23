using BepInEx.Bootstrap;
using BepInEx.Configuration;
using LC_InsanityDisplay.Plugin.UI;
using System;
using TMPro;
using UnityEngine;

namespace LC_InsanityDisplay.Plugin.ModCompatibility
{
    // NOTE: Mods that do not require 'hands-on' compatibility will be here
    // With 'hands-on' I mean having to use values, methods, etc from the mod's .dll

    /// <summary>
    /// Responsible for the compatibility of CrouchHUD
    /// </summary>
    public class LCCrouchHUDCompatibility
    {
        internal const string ModGUID = "LCCrouchHUD";
        private static GameObject CrouchHUD = null!;
        private static Transform IconTransform = null!;
        private static Vector3 positionToLocal = Vector3.zero; //Essentially fixes issues that may occur with resolution related stuff
        private static Vector3 localPositionOffset = new(3f, 7f, 0); //Vector3 is a struct so no GC alloc
        private static bool DisableCrouchHUD = false;

        private static void Initialize()
        {
            DisableCrouchHUD = CompatibleDependencyAttribute.IsEladsHudPresent || CompatibleDependencyAttribute.IsLCVRPresent;
            if (DisableCrouchHUD) return;
            ConfigHandler.Compat.LCCrouchHUD.SettingChanged += UpdateIconPosition;
        }
        private static void Start()
        {
            if (DisableCrouchHUD) return;
            Transform PlayerIconTransform = HUDBehaviour.PlayerIcon.transform;
            CrouchHUD = PlayerIconTransform.GetChild(PlayerIconTransform.childCount - 1).gameObject; //CrouchHUD puts itself in the last index, this will make sure it's found
            if (!CrouchHUD) return;
            IconTransform = CrouchHUD.transform;
            if (positionToLocal == Vector3.zero) positionToLocal = IconTransform.localPosition;

            UpdateIconPosition();
        }

        private static void UpdateIconPosition(object sender = null!, EventArgs e = null!)
        {
            if (CrouchHUD == null || IconTransform == null || positionToLocal == Vector3.zero) return; //can't update it if it ain't there

            IconTransform.SetLocalPositionAndRotation(localPosition: ConfigHandler.Compat.LCCrouchHUD.Value ? positionToLocal + localPositionOffset : positionToLocal, localRotation: IconTransform.localRotation);
        }
    }
    /// <summary>
    /// Responsible for the compatibility of An0n Patches
    /// </summary>
    public class An0nPatchesCompatibility
    {
        internal const string ModGUID = "com.an0n.patch";
        private static GameObject An0nTextHUD = null!;
        private static Transform An0nTransform = null!;
        private static Vector3 localPositionOffset = new(3f, 15f, 0);
        private static Vector3 localPosition = Vector3.zero;
        private static bool DisableAn0nHud = false;

        private static void Initialize()
        {
            DisableAn0nHud = CompatibleDependencyAttribute.IsEladsHudPresent || CompatibleDependencyAttribute.IsLCVRPresent;
            if (DisableAn0nHud) return; //Don't run if Elad's HUD is present or ... or ...
            ConfigHandler.Compat.An0nPatches.SettingChanged += UpdateAn0nDisplay;
        }

        private static void Start()
        {
            if (DisableAn0nHud) return;
            Animator[] ComponentList = HUDInjector.TopLeftHUD.GetComponentsInChildren<Animator>(true);
            foreach (Animator component in ComponentList) //fetch the HitpointDisplay (is there a better for this? probably
            {
                if (component.name != "HPSP") continue; //An0nPatches' HUD has three of these in the init and four in the update
                An0nTextHUD = component.gameObject;
                break;
            }
            if (!An0nTextHUD) return;
            An0nTransform = An0nTextHUD.transform;
            if (localPosition == Vector3.zero) localPosition = An0nTransform.localPosition;

            UpdateAn0nDisplay();
        }

        private static void UpdateAn0nDisplay(object sender = null!, EventArgs e = null!)
        {
            if (An0nTextHUD == null || An0nTransform == null || localPosition == Vector3.zero) return; //can't update it if it ain't there

            An0nTransform.SetLocalPositionAndRotation(localPosition: ConfigHandler.Compat.An0nPatches.Value ? localPosition + localPositionOffset : localPosition, localRotation: An0nTransform.localRotation);
        }
    }
    /// <summary>
    /// Responsible for the compatibility of HealthMetrics 
    /// </summary>
    public class HealthMetricsCompatibility
    {
        internal const string ModGUID = "Matsuura.HealthMetrics";
        private static GameObject HealthMeter = null!;
        private static Transform MeterTransform = null!;
        //private static Vector3 localPositionOffset_Damage = new(-10f, 0, 0);
        private static Vector3 localPositionOffset = new(-2f, 0, 0);
        private static Vector3 localPosition = Vector3.zero;
        private static bool DisableHealthMetrics;

        private static void Initialize()
        {
            DisableHealthMetrics = CompatibleDependencyAttribute.IsEladsHudPresent || CompatibleDependencyAttribute.IsLCVRPresent;
            if (DisableHealthMetrics) return;

            ConfigHandler.Compat.HealthMetrics.SettingChanged += UpdateHealthMeter;
        }

        private static void Start()
        {
            if (DisableHealthMetrics) return;
            HealthMeter = Shared_FetchHUDDisplay();
            if (!HealthMeter) return;
            MeterTransform = HealthMeter.transform;
            localPosition = HealthMeter.transform.localPosition;

            UpdateHealthMeter();
        }

        internal static void UpdateHealthMeter(object sender = null!, EventArgs e = null!)
        {
            if (HealthMeter == null || MeterTransform == null || localPosition == Vector3.zero) return; //can't update it if it ain't there

            MeterTransform.SetLocalPositionAndRotation(localPosition: ConfigHandler.Compat.HealthMetrics.Value ? localPosition + localPositionOffset : localPosition, localRotation: MeterTransform.localRotation);
        }
        /// <summary>
        /// Method to be used by HealthMetrics and DamageMetrics compatibility
        /// </summary>
        /// <returns>The gameobject that belongs to HealthMetrics or DamageMetrics</returns>
        internal static GameObject Shared_FetchHUDDisplay()
        {
            TextMeshProUGUI[] ComponentList = HUDInjector.TopLeftHUD.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI component in ComponentList) //fetch the HitpointDisplay (is there a better for this? probably
            {
                if (component.gameObject.name == "HealthHUDDisplay")
                    return component.gameObject;
            }
            return null!;
        }
    }
    /// <summary>
    /// Responsible for the compatibility of DamageMetrics 
    /// </summary>
    public class DamageMetricsCompatibility
    {
        internal const string ModGUID = "Matsuura.TestAccount666.DamageMetrics";
        private static GameObject DamageMeter = null!;
        private static Transform MeterTransform = null!;
        private static Vector3 localPositionOffset = new(-10f, 0, 0);
        //private static Vector3 localPositionOffset_Health = new(-2f, 0, 0);
        private static Vector3 localPosition = Vector3.zero;
        private static bool DisableDamageMetrics;

        private static void Initialize()
        {
            DisableDamageMetrics = CompatibleDependencyAttribute.IsEladsHudPresent || CompatibleDependencyAttribute.IsLCVRPresent;
            if (DisableDamageMetrics) return;

            ConfigHandler.Compat.DamageMetrics.SettingChanged += UpdateHealthMeter;
        }

        private static void Start()
        {
            if (DisableDamageMetrics) return;
            DamageMeter = HealthMetricsCompatibility.Shared_FetchHUDDisplay();
            if (!DamageMeter) return;
            MeterTransform = DamageMeter.transform;
            localPosition = DamageMeter.transform.localPosition;

            UpdateHealthMeter();
        }

        internal static void UpdateHealthMeter(object sender = null!, EventArgs e = null!)
        {
            if (DamageMeter == null || MeterTransform == null || localPosition == Vector3.zero) return; //can't update it if it ain't there

            MeterTransform.SetLocalPositionAndRotation(localPosition: ConfigHandler.Compat.DamageMetrics.Value ? localPosition + localPositionOffset : localPosition, localRotation: MeterTransform.localRotation);
        }
    }
    /// <summary>
    /// Responsible for the compatibility of GeneralImprovements
    /// </summary>
    public class GeneralImprovementsCompatibility
    {
        //GI doesn't have config changes until restarting game, so if health ui is not found never check for it again
        internal const string ModGUID = "ShaosilGaming.GeneralImprovements";

        private static GameObject HitpointDisplay = null!;
        private static Vector3 localPosition = Vector3.zero;
        private static Vector3 localPositionOffset = new Vector3(-2f, 28f, 0);

        public static bool HitpointDisplayActive = false;

        private static void Initialize()
        {
            ConfigFile GIConfig = Chainloader.PluginInfos[ModGUID].Instance.Config;
            foreach (ConfigDefinition configDef in GIConfig.Keys)
            {
                if (configDef.Key != "ShowHitPoints") continue;
                GIConfig.TryGetEntry(configDef, out ConfigEntry<bool> ShowHitPointsEntry);
                HitpointDisplayActive = ShowHitPointsEntry.Value;
                break;
            }
            if (!HitpointDisplayActive) return;
            ConfigHandler.Compat.GeneralImprovements.SettingChanged += UpdateDisplayPosition;
        }

        private static void Start()
        {
            if (!HitpointDisplayActive) return; //Don't run if the hitpoints are disabled
            TextMeshProUGUI[] ComponentList = HUDInjector.TopLeftHUD.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI component in ComponentList) //fetch the HitpointDisplay (is there a better for this? probably
            {
                if (component.name != "HP") continue;
                HitpointDisplay = component.gameObject;
                break;
            }
            if (!HitpointDisplay) return;
            if (localPosition == Vector3.zero) localPosition = HitpointDisplay.transform.localPosition;

            UpdateDisplayPosition();
        }

        internal static void UpdateDisplayPosition(object sender = null!, EventArgs e = null!)
        {
            if (!HitpointDisplayActive || !HitpointDisplay) return; //can't update it if it's not there 
            Transform DisplayTransform = HitpointDisplay.transform;
            DisplayTransform.SetLocalPositionAndRotation(ConfigHandler.Compat.GeneralImprovements.Value ? localPosition + localPositionOffset : localPosition, DisplayTransform.localRotation);
        }
    }
    /// <summary>
    /// Responsible for the compatibility of Elad's HUD
    /// </summary>
    public class EladsHUDCompatibility
    {
        internal const string ModGUID = "me.eladnlg.customhud";

        //private static Vector3 localPositionOffset = new Vector3(0, 10, 0);
        //private static Vector3 Percentage_localPositionOffset = new Vector3(0, 28.4f, 0);
        //private static Vector3 batteryLocalPosition = Vector3.zero;

        //private static GameObject PercentageInsanityText;
        //private static Transform batteryUI;

        //public static TextMeshProUGUI InsanityInfo;

        private static void Initialize()
        {
            //if in vr would suffice
            //if (CompatibleDependencyAttribute.IsLCVRPresent && ) return;
            ConfigHandler.Compat.EladsHUD.SettingChanged += UpdateVisibility;
        }

        private static void Start()
        {
            if (CompatibleDependencyAttribute.IsLCVRPresent) return;
        }

        private static void UpdateVisibility(object sender = null!, EventArgs e = null!)
        {

        }
        /*
        public static void EditEladsHUD() //This will create a new bar, the stamina bar, and move some elements to make it not overlap (also this code is kinda all over the place damn)
        {
            GameObject EladsHUDStamina = vanillaSprintMeter?.transform.parent.transform.parent.Find("PlayerInfo(Clone)/Stamina")?.gameObject;
            if (!EladsHUDStamina) { return; }
            if (!InsanityMeter)
            {
                //Create the meter and remove unnecessary elements from it
                InsanityMeter = Object.Instantiate(EladsHUDStamina);
                InsanityMeter.name = "Insanity";
                //Unity Log Warning: Unable to add the requested character to font asset [3270-REGULAR SDF]'s atlas texture. Please make the texture [3270-REGULAR SDF Atlas] readable.
                //Most likely caused by destroying these
                Object.Destroy(InsanityMeter.transform.Find("CarryInfo")?.gameObject); //Remove CarryInfo
                Object.Destroy(InsanityMeter.transform.Find("Bar/Stamina Change FG")?.gameObject); //Remove unnecessary part of the insanity bar

                Transform EladsHUDObject = EladsHUDStamina.transform.parent;
                Transform StaminaObject = EladsHUDStamina.transform;
                batteryUI = EladsHUDObject.Find("BatteryLayout").gameObject.transform;

                //Set the position, rotation, etc
                Transform meterTransform = InsanityMeter.transform;
                meterTransform.SetParent(EladsHUDObject);
                meterTransform.localPosition = StaminaObject.localPosition;
                meterTransform.localScale = StaminaObject.localScale;
                meterTransform.rotation = StaminaObject.rotation;

                PercentageInsanityText = meterTransform.Find("StaminaInfo").gameObject;
                batteryLocalPosition = batteryLocalPosition == Vector3.zero ? batteryUI.localPosition : batteryLocalPosition;

                InsanityInfo = PercentageInsanityText.GetComponent<TextMeshProUGUI>();
                InsanityInfo.horizontalAlignment = HorizontalAlignmentOptions.Right;

                InsanityImage = meterTransform.Find("Bar/StaminaBar").gameObject.GetComponent<Image>();

                //Move with Offset (not the meter itself because without compat it wouldn't exist)
                InsanityMeter.transform.localPosition += localPositionOffset;
                PercentageInsanityText.transform.localPosition += Percentage_localPositionOffset;
            }

            UpdateMeter(imageMeter: InsanityImage, textMeter: InsanityInfo);

            bool EladsCompat = ConfigHandler.Compat.EladsHUD.Value;
            if (EladsCompat && batteryUI.localPosition != batteryLocalPosition + localPositionOffset || !EladsCompat && batteryUI.localPosition != batteryLocalPosition) //update if hud is positioned incorrectly (only the battery part of elad's hud)
            {
                batteryUI.localPosition = EladsCompat && ConfigHandler.ModEnabled.Value ? batteryLocalPosition + localPositionOffset : batteryLocalPosition;
            }

            bool MeterActive = EladsCompat && ConfigHandler.ModEnabled.Value;
            if (InsanityMeter.activeSelf != MeterActive || PercentageInsanityText.activeSelf != MeterActive) //if compat not enabled or mod not enabeld hide the meter
            {
                InsanityMeter.SetActive(MeterActive);
                PercentageInsanityText.SetActive(MeterActive);
            }
        }
        */
    }

}