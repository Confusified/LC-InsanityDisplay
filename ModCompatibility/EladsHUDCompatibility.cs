using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InsanityDisplay.UI.UIHandler;

namespace InsanityDisplay.ModCompatibility
{
    public class EladsHUDCompatibility
    {
        private static Vector3 localPositionOffset = new Vector3(0, 10, 0);
        private static Vector3 Percentage_localPositionOffset = new Vector3(0, 28.4f, 0);

        public static TextMeshProUGUI InsanityInfo;

        public static void EditEladsHUD() //This will create a new bar, the stamina bar, and move some elements to make it not overlap
        {
            GameObject EladsHUDStamina = GameNetworkManager.Instance.localPlayerController.sprintMeterUI.transform.parent.transform.parent.Find("PlayerInfo(Clone)/Stamina").gameObject;
            if (EladsHUDStamina == null) { Initialise.modLogger.LogError("EladsHUD Stamina bar wasn't found"); return; }
            //Create the meter and remove unnecessary elements from it
            InsanityMeter = GameObject.Instantiate(EladsHUDStamina);
            InsanityMeter.name = "Insanity";
            GameObject.DestroyImmediate(InsanityMeter.transform.Find("CarryInfo").gameObject); //Remove CarryInfo
            GameObject.DestroyImmediate(InsanityMeter.transform.Find("Bar/Stamina Change FG").gameObject); //Remove unnecessary part of the insanity bar
            //Set the position, rotation, etc
            Transform EladsHUDObject = EladsHUDStamina.transform.parent;
            Transform StaminaObject = EladsHUDStamina.transform;
            Transform meterTransform = InsanityMeter.transform;

            meterTransform.SetParent(EladsHUDObject);
            meterTransform.localPosition = StaminaObject.localPosition;
            meterTransform.localScale = StaminaObject.localScale;
            meterTransform.rotation = StaminaObject.rotation;

            GameObject PercentageInsanityText = meterTransform.Find("StaminaInfo").gameObject;

            InsanityInfo = PercentageInsanityText.GetComponent<TextMeshProUGUI>();
            InsanityInfo.horizontalAlignment = HorizontalAlignmentOptions.Right;

            InsanityImage = meterTransform.Find("Bar/StaminaBar").gameObject.GetComponent<Image>();
            UpdateMeter(imageMeter: InsanityImage, textMeter: InsanityInfo);
            //Move with Offset
            EladsHUDObject.Find("BatteryLayout").gameObject.transform.localPosition += localPositionOffset;
            InsanityMeter.transform.localPosition += localPositionOffset;
            PercentageInsanityText.transform.localPosition += Percentage_localPositionOffset;
        }
    }
}