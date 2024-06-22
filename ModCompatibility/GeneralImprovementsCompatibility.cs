using LC_InsanityDisplay.Config;
using LC_InsanityDisplay.UI;
using System;
using TMPro;
using UnityEngine;

namespace LC_InsanityDisplay.ModCompatibility
{
    public class GeneralImprovementsCompatibility
    {
        //GI doesn't have config changes until restarting game, so if health ui is not found never check for it again
        internal const string ModGUID = "ShaosilGaming.GeneralImprovements";

        private static GameObject HitpointDisplay = null!;
        private static Vector3 localPosition = Vector3.zero;
        private static Vector3 localPositionOffset = (Vector3.left * 2f) + (Vector3.up * 28f);//new Vector3(-2f, 28f, 0);

        public static bool HitpointDisplayActive = false;

        private static void Initialize()
        {
            HitpointDisplayActive = GeneralImprovements.Plugin.ShowHitPoints.Value;
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
            if (localPosition == Vector3.zero) localPosition = HitpointDisplay.transform.localPosition;

            UpdateDisplayPosition();
        }

        internal static void UpdateDisplayPosition(object sender = null!, EventArgs e = null!)
        {
            if (!HitpointDisplayActive || !HitpointDisplay) return; //can't update it if it's not there 
            Transform DisplayTransform = HitpointDisplay.transform;
            DisplayTransform.localPosition = ConfigHandler.Compat.GeneralImprovements.Value ? localPosition + localPositionOffset : localPosition;
        }
    }
}