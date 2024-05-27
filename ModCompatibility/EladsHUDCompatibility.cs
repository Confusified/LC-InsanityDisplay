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

        private static GameObject PercentageInsanityText;
        public static TextMeshProUGUI InsanityInfo;

        public static void EditEladsHUD()
        {
            CreateCustomInsanityBar();
            MoveWithOffset();
        }

        private static void CreateInsanityBarInMemory()
        {
            if (Memory_InsanityMeter != null) { return; }
            GameObject EladsHUDStamina = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/PlayerInfo(Clone)/Stamina").gameObject;
            if (EladsHUDStamina == null) { Initialise.modLogger.LogError("EladsHUD Stamina bar wasn't found"); return; }

            Memory_InsanityMeter = GameObject.Instantiate(EladsHUDStamina);
            GameObject.DestroyImmediate(Memory_InsanityMeter.transform.Find("CarryInfo").gameObject); //Remove CarryInfo
            GameObject.DestroyImmediate(Memory_InsanityMeter.transform.Find("Bar/Stamina Change FG").gameObject); //Remove unnecessary part of the insanity bar

            GameObject.DontDestroyOnLoad(Memory_InsanityMeter);
        }

        private static void CreateCustomInsanityBar()
        {
            if (Memory_InsanityMeter == null) { CreateInsanityBarInMemory(); } //Create in memory first
            InsanityMeter = GameObject.Instantiate(Memory_InsanityMeter);
            InsanityMeter.name = "Insanity";

            Transform EladsHUDObject = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/PlayerInfo(Clone)").gameObject.transform;
            Transform StaminaObject = EladsHUDObject.Find("Stamina").gameObject.transform;

            Transform meterTransform = InsanityMeter.transform;
            meterTransform.SetParent(EladsHUDObject);
            meterTransform.localPosition = StaminaObject.localPosition;
            meterTransform.localScale = StaminaObject.localScale;
            meterTransform.rotation = StaminaObject.rotation;

            PercentageInsanityText = meterTransform.Find("StaminaInfo").gameObject;

            InsanityInfo = PercentageInsanityText.GetComponent<TextMeshProUGUI>();
            InsanityInfo.horizontalAlignment = HorizontalAlignmentOptions.Right;

            InsanityImage = meterTransform.Find("Bar/StaminaBar").gameObject.GetComponent<Image>();
            UpdateMeter(imageMeter: InsanityImage, textMeter: InsanityInfo);
        }

        private static void MoveWithOffset()
        {
            GameObject EladsHUDObject = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/PlayerInfo(Clone)").gameObject;
            if (EladsHUDObject == null) { Initialise.modLogger.LogError("EladsHUD UI wasn't found"); return; }

            Transform HUDTransform = EladsHUDObject.transform;
            HUDTransform.Find("BatteryLayout").gameObject.transform.localPosition += localPositionOffset;

            InsanityMeter.transform.localPosition += localPositionOffset;
            PercentageInsanityText.transform.localPosition += Percentage_localPositionOffset;
        }
    }
}