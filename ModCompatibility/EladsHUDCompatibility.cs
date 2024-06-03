using InsanityDisplay.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InsanityDisplay.UI.MeterHandler;

namespace InsanityDisplay.ModCompatibility
{
    public class EladsHUDCompatibility
    {
        private static Vector3 localPositionOffset = new Vector3(0, 10, 0);
        private static Vector3 Percentage_localPositionOffset = new Vector3(0, 28.4f, 0);
        private static Vector3 batteryLocalPosition = Vector3.zero;

        public static TextMeshProUGUI InsanityInfo;

        public static void EditEladsHUD() //This will create a new bar, the stamina bar, and move some elements to make it not overlap (also this code is kinda all over the place damn)
        {
            GameObject EladsHUDStamina = vanillaSprintMeter.transform.parent.transform.parent.Find("PlayerInfo(Clone)/Stamina").gameObject;
            if (EladsHUDStamina == null) { Initialise.modLogger.LogError("EladsHUD Stamina bar wasn't found"); return; }
            //Create the meter and remove unnecessary elements from it
            InsanityMeter = GameObject.Instantiate(EladsHUDStamina);
            InsanityMeter.name = "Insanity";
            GameObject.Destroy(InsanityMeter.transform.Find("CarryInfo").gameObject); //Remove CarryInfo
            GameObject.Destroy(InsanityMeter.transform.Find("Bar/Stamina Change FG").gameObject); //Remove unnecessary part of the insanity bar
            //Set the position, rotation, etc
            Transform EladsHUDObject = EladsHUDStamina.transform.parent;
            Transform StaminaObject = EladsHUDStamina.transform;
            Transform meterTransform = InsanityMeter.transform;
            Transform batteryUI = EladsHUDObject.Find("BatteryLayout").gameObject.transform;

            meterTransform.SetParent(EladsHUDObject);
            meterTransform.localPosition = StaminaObject.localPosition;
            meterTransform.localScale = StaminaObject.localScale;
            meterTransform.rotation = StaminaObject.rotation;

            GameObject PercentageInsanityText = meterTransform.Find("StaminaInfo").gameObject;

            batteryLocalPosition = batteryLocalPosition == Vector3.zero ? batteryUI.localPosition : batteryLocalPosition;

            InsanityInfo = PercentageInsanityText.GetComponent<TextMeshProUGUI>();
            InsanityInfo.horizontalAlignment = HorizontalAlignmentOptions.Right;

            InsanityImage = meterTransform.Find("Bar/StaminaBar").gameObject.GetComponent<Image>();
            UpdateMeter(imageMeter: InsanityImage, textMeter: InsanityInfo);
            //Move with Offset (not the meter itself because without compat it wouldn't exist)
            InsanityMeter.transform.localPosition += localPositionOffset;
            PercentageInsanityText.transform.localPosition += Percentage_localPositionOffset;

            bool EladsCompat = ConfigSettings.Compat.EladsHUD.Value;
            if ((EladsCompat && batteryUI.localPosition != (batteryLocalPosition + localPositionOffset)) || (!EladsCompat && batteryUI.localPosition != batteryLocalPosition)) //update if hud is positioned incorrectly (only the battery part of elad's hud)
            {
                batteryUI.localPosition = EladsCompat && ConfigSettings.ModEnabled.Value ? batteryLocalPosition + localPositionOffset : batteryLocalPosition;
            }

            bool MeterActive = EladsCompat && ConfigSettings.ModEnabled.Value;
            if (InsanityMeter.activeSelf != (MeterActive) || PercentageInsanityText.activeSelf != (MeterActive)) //if compat not enabled or mod not enabeld hide the meter
            {
                InsanityMeter.SetActive(MeterActive);
                PercentageInsanityText.SetActive(MeterActive);
            }
        }
    }
}