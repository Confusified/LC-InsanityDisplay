using InsanityDisplay.Config;
using UnityEngine;
using UnityEngine.UI;
using InsanityDisplay.ModCompatibility;
using GameNetcodeStuff;
using static InsanityDisplay.ModCompatibility.CompatibilityList;

namespace InsanityDisplay.UI
{
    public class UIHandler
    {
        private static Vector3 localPositionOffset = new Vector3(-3.4f, 3.7f, 0f); //-271.076 102.6285 -13.0663 = normal
        private static Vector3 localScale = new Vector3(1.4f, 1.4f, 1.4f); //SprintMeter scale is 1.6892 1.6892 1.6892
        private static Vector3 selfLocalPositionOffset = new Vector3(-6.8f, -4f, 0f); // -272.7607 112.2663 -14.2212 = normal    -279.5677f, 116.2748f, -14.2174f
        private static Color meterColor = ConfigSettings.MeterColor.Value;

        private const float accurate_MinValue = 0.2978f; //Becomes visible starting 0.298f
        private const float accurate_MaxValue = 0.9101f; //No visible changes after this value

        public static GameObject Memory_InsanityMeter;
        public static GameObject InsanityMeter;

        public static Image InsanityImage;

        private static PlayerControllerB localPlayer;

        public static void CreateInMemory()
        {
            if (Memory_InsanityMeter != null) { CreateInScene(); return; } //It already exists
            if (CompatibilityList.EladsHUD_Installed) { EnableCompatibilities(); return; }
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

            Transform meterTransform = InsanityMeter.transform;
            meterTransform.SetParent(TopLeftCornerHUD.transform);
            meterTransform.SetAsFirstSibling();
            meterTransform.localPosition += localPositionOffset;
            meterTransform.localScale = localScale;

            InsanityImage = InsanityMeter.GetComponent<Image>();
            InsanityImage.color = meterColor + new Color(0, 0, 0, 1); //Always set to completely visible regardless of config
            InsanityImage.fillAmount = GetFillAmount();
            InsanityMeter.SetActive(ConfigSettings.ModEnabled.Value);

            GameObject selfObject = TopLeftCornerHUD.transform.Find("Self").gameObject;
            selfObject.transform.localPosition += selfLocalPositionOffset;

            GameObject selfRedObject = TopLeftCornerHUD.transform.Find("SelfRed").gameObject;
            selfRedObject.transform.localPosition += selfLocalPositionOffset;

            EnableCompatibilities();
            return;
        }

        private static void EnableCompatibilities()
        {
            if (LCCrouch_Installed && ConfigSettings.LCCrouchHUDCompat.Value)
            {
                LCCrouchHUDCompatibility.MoveCrouchHUD();
            }
            if (EladsHUD_Installed && ConfigSettings.EladsHUDCompat.Value)
            {
                Memory_InsanityMeter = null;
                InsanityMeter = null;
                EladsHUDCompatibility.EditEladsHUD();
            }
            if (An0nPatches_Installed && ConfigSettings.An0nPatchesCompat.Value)
            {
                An0nPatchesCompatibility.MoveTextHUD();
            }
        }

        public static float GetFillAmount()
        {
            if (GameNetworkManager.Instance.localPlayerController == null) { return 0; } //Avoid errors
            localPlayer = GameNetworkManager.Instance.localPlayerController;
            float finalFillAmount = 0;
            if (ConfigSettings.useAccurateDisplay.Value && !EladsHUD_Installed) //if using accurate display and Elad's HUD is not present
            {
                finalFillAmount = accurate_MinValue + ((localPlayer.insanityLevel / localPlayer.maxInsanityLevel) * accurate_MaxValue);
            }
            else
            {
                finalFillAmount = localPlayer.insanityLevel / localPlayer.maxInsanityLevel;
            }
            return finalFillAmount;
        }
    }
}