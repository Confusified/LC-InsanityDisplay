using InsanityDisplay.Config;
using UnityEngine;
using UnityEngine.UI;
using InsanityDisplay.ModCompatibility;
using static InsanityDisplay.ModCompatibility.CompatibilityList;
using GameNetcodeStuff;

namespace InsanityDisplay.UI
{
    public class UIHandler
    {
        private static Vector3 localPosition = new Vector3(-272.576f, 105.6285f, -13.0663f);
        private static Vector3 localScale = new Vector3(1.4892f, 1.4892f, 1.4892f); //SprintMeter scale is 1.6892 1.6892 1.6892
        private static Color meterColor = ConfigSettings.MeterColor.Value;

        public static GameObject Memory_InsanityMeter;
        public static GameObject InsanityMeter;

        public static Image InsanityImage;

        private static PlayerControllerB localPlayer;

        public static void CreateInMemory()
        {
            if (Memory_InsanityMeter != null) { CreateInScene(); return; } //It already exists
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

            Transform meterTransform = InsanityMeter.transform;
            meterTransform.SetParent(GameObject.Find("Systems").gameObject.transform.Find("UI").gameObject.transform.Find("Canvas").gameObject.transform.Find("IngamePlayerHUD").gameObject.transform.Find("TopLeftCorner").gameObject.transform);
            meterTransform.SetAsFirstSibling();
            meterTransform.localPosition = localPosition;
            meterTransform.localScale = localScale;

            InsanityImage = InsanityMeter.GetComponent<Image>();
            InsanityImage.color = meterColor;
            InsanityImage.fillAmount = GetFillAmount();
            InsanityMeter.SetActive(ConfigSettings.ModEnabled.Value);

            EnableCompatibilities();
        }

        private static void EnableCompatibilities()
        {
            if (LCCrouch_Installed)
            {
                LCCrouchHUDCompatibility.MoveCrouchHUD();
            }
            if (HealthMetrics_Installed)
            {
                HealthMetricsCompatibility.MoveHealthMetrics();
            }
            if (EladsHUD_Installed)
            {
                //nothing yet
            }
        }

        public static float GetFillAmount()
        {
            if (GameNetworkManager.Instance.localPlayerController == null) { return 0; } //Avoid errors
            localPlayer = GameNetworkManager.Instance.localPlayerController;
            return localPlayer.insanityLevel / localPlayer.maxInsanityLevel;
        }
    }
}