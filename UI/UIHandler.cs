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
        private static Vector3 localPosition = new Vector3(-274.4761f, 106.3285f, -13.0663f);
        private static Vector3 localScale = new Vector3(1.4f, 1.4f, 1.4f); //SprintMeter scale is 1.6892 1.6892 1.6892
        private static Vector3 selfLocalPosition = new Vector3(-279.5677f, 116.2748f, -14.2174f);
        private static Color meterColor = ConfigSettings.MeterColor.Value;

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
            meterTransform.localPosition = localPosition;
            meterTransform.localScale = localScale;

            InsanityImage = InsanityMeter.GetComponent<Image>();
            InsanityImage.color = meterColor;
            InsanityImage.fillAmount = GetFillAmount();
            InsanityMeter.SetActive(ConfigSettings.ModEnabled.Value);

            GameObject selfObject = TopLeftCornerHUD.transform.Find("Self").gameObject;
            selfObject.transform.localPosition = selfLocalPosition;

            GameObject selfRedObject = TopLeftCornerHUD.transform.Find("SelfRed").gameObject;
            selfRedObject.transform.localPosition = selfLocalPosition;

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
            return localPlayer.insanityLevel / localPlayer.maxInsanityLevel;
        }
    }
}