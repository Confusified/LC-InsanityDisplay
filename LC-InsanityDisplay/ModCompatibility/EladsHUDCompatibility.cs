using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using LC_InsanityDisplay.Plugin.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LC_InsanityDisplay.Plugin.ModCompatibility
{
    /// <summary>
    /// Responsible for the compatibility of Elad's HUD
    /// </summary>
    public class EladsHUDCompatibility
    {
        internal const string ModGUID = "me.eladnlg.customhud";
        internal const string OxygenGUID = "consequential.Oxygen";
        internal static GameObject InsanityPercentageObject { get; private set; } = null!;

        private static bool EladsHUDEnabled;
        private static GameObject EladsHUDObject { get; set; } = null!;
        private static TextMeshProUGUI InsanityPercentageText { get; set; } = null!;
        private static Transform BatteryLayoutTransform { get; set; } = null!;
        private static GameObject InsanityMeterShadow { get; set; } = null!;
        private static Vector3 InsanityBarOffset = new(0, 8f, 0);
        private static Vector3 PercentageObjectOffset = new(0, 26.4f, 0);
        private static Vector3 BatteryLayoutOffset = new(-14f, 4f, 0);
        private static Vector3 PTTOffset = new(120, 20, 0);

        private static int oldBarFill = -1;

        private static void Initialize()
        {
            if (CompatibleDependencyAttribute.IsLCVRPresent)
            {
                Initialise.Logger.LogDebug("Elad's HUD and LCVR are both found, they are not compatible with each other");
                return; // Elad's HUD isn't compatible with LCVR
            }
            CompatibleDependencyAttribute.IsEladsHudPresent = true;
            ConfigHandler.Compat.EladsHUD.SettingChanged += UpdateVisibility;
        }

        private static void Start()
        {
            if (CompatibleDependencyAttribute.IsLCVRPresent || !ConfigHandler.Compat.EladsHUD.Value) return; // Elad's HUD isn't compatible with LCVR
            GameObject? StaminaObject = null;
            foreach (CanvasGroup component in HUDInjector.TopLeftHUD.transform.parent.GetComponentsInChildren<CanvasGroup>(true))
            {

                if (component.name == "PlayerInfo(Clone)")
                {
                    StaminaObject = component.transform.Find("Stamina").gameObject;
                    EladsHUDObject = component.gameObject;
                }

                if (StaminaObject != null) break;
            }

            if (StaminaObject == null)
            {
                Initialise.Logger.LogDebug("Could not find Elad's Hud Stamina bar");
                return;
            }

            BatteryLayoutTransform = EladsHUDObject.transform.GetChild(2);
            // Disable the text elements (so they won't cause a warning in the console when they're copied)
            StaminaObject.transform.GetChild(1).gameObject.SetActive(false); // CarryInfo
            StaminaObject.transform.GetChild(2).gameObject.SetActive(false); // StaminaInfo

            HUDInjector.InsanityMeter = GameObject.Instantiate(StaminaObject, EladsHUDObject.transform);
            HUDInjector.InsanityMeter.SetActive(false);
            HUDInjector.InsanityMeter.name = HUDInjector.ModName;

            // Re-enable the text elements as they're part of the Stamina bar
            StaminaObject.transform.GetChild(1).gameObject.SetActive(true); // CarryInfo
            StaminaObject.transform.GetChild(2).gameObject.SetActive(true); // StaminaInfo

            Initialise.Logger.LogDebug("Created insanity bar");


            InsanityMeterShadow = HUDInjector.InsanityMeter.transform.GetChild(0).gameObject;
            InsanityMeterShadow.name = "Insanity BG";
            GameObject unusedCarryInfo = HUDInjector.InsanityMeter.transform.GetChild(1).gameObject;
            InsanityPercentageObject = HUDInjector.InsanityMeter.transform.GetChild(2).gameObject;

            InsanityPercentageObject.name = "InsanityInfo";
            if (CompatibleDependencyAttribute.IsModPresent(OxygenGUID))
            {
                InsanityPercentageObject.transform.localPosition += InsanityBarOffset;
            }

            InsanityPercentageText = InsanityPercentageObject.GetComponent<TextMeshProUGUI>();
            InsanityPercentageText.horizontalAlignment = HorizontalAlignmentOptions.Right;
            foreach (Image component in HUDInjector.InsanityMeter.gameObject.GetComponentsInChildren<Image>())
            {
                if (component.gameObject.GetComponent<Mask>())
                {
                    HUDInjector.InsanityMeterComponent = component;
                    break;
                }
            }

            if (HUDInjector.InsanityMeterComponent == null)
            {
                Initialise.Logger.LogDebug("InsanityMeterComponent is null (BAD)");
                return;
            }

            // Destroy CarryInfo from the Insanity bar as it's unused
            GameObject.Destroy(unusedCarryInfo);

            // Move the elements to avoid overlapping
            HUDInjector.InsanityMeter.transform.localPosition += InsanityBarOffset;
            InsanityPercentageObject.transform.localPosition += PercentageObjectOffset;
            BatteryLayoutTransform.localPosition += BatteryLayoutOffset;

            // Get the HUD Scale from Elad's Hud to scale the PTT Icon up (or down)
            PluginInfo EladsHudInfo;
            Chainloader.PluginInfos.TryGetValue(ModGUID, out EladsHudInfo);
            ConfigFile EladsHudConfig = EladsHudInfo.Instance.Config;
            float EladsHudScale = 1;

            foreach (var configDefinition in EladsHudConfig.Keys)
            {
                if (configDefinition.Key == "HUDScale")
                {
                    if (EladsHudConfig.TryGetEntry(configDefinition, out ConfigEntry<float> configEntry))
                    {
                        EladsHudScale = configEntry.Value;
                    }
                }
            }

            Transform PTTIconObject = HUDInjector.HUDManagerInstance.PTTIcon.transform;
            // Disable the PTT object to reduce any calls made
            PTTIconObject.gameObject.SetActive(false);
            // Reposition the PTT Icon so it doesn't overlap with anything (and subjectively looks better)
            Transform originalParent = PTTIconObject.parent;
            PTTIconObject.SetParent(InsanityPercentageObject.transform);
            PTTIconObject.position = InsanityPercentageObject.transform.position;
            PTTIconObject.localPosition += PTTOffset;
            PTTIconObject.localScale *= EladsHudScale;

            PTTIconObject.SetParent(originalParent, worldPositionStays: true);
            PTTIconObject.gameObject.SetActive(true);

            // The insanity meter has been set up so it can be re-enabled
            oldBarFill = -1;

            InsanityPercentageObject.SetActive(true); // Causes a harmless warning to show up in the console (could probably be hidden with a try catch)
            HUDInjector.InsanityMeter.SetActive(true);
        }

        private static void UpdateVisibility(object sender = null!, EventArgs e = null!)
        {
            EladsHUDEnabled = ConfigHandler.Compat.EladsHUD.Value;
            if (EladsHUDEnabled)
            {

            }
        }

        internal static void UpdatePercentageText()
        {
            int newBarFill = (int)MathF.Round(HUDInjector.InsanityMeterComponent.fillAmount * 100);
            if (oldBarFill == newBarFill) return; // The text is already the same as previously, so don't update
            oldBarFill = newBarFill;
            InsanityPercentageText.text = $"{newBarFill}%";
        }

        internal static void UpdateColour()
        {
            InsanityPercentageText.color = HUDBehaviour.InsanityMeterColor;
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
