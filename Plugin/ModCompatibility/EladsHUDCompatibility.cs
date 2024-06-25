using System;
using System.Collections.Generic;
using System.Text;

namespace LC_InsanityDisplay.Plugin.ModCompatibility
{
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
            CompatibleDependencyAttribute.IsEladsHudPresent = true;
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
