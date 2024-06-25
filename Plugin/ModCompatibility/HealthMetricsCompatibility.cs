using LC_InsanityDisplay.Plugin.UI;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace LC_InsanityDisplay.Plugin.ModCompatibility
{
    /// <summary>
    /// Responsible for the compatibility of HealthMetrics
    /// </summary>
    public class HealthMetricsCompatibility
    {
        internal const string ModGUID = "Matsuura.HealthMetrics";

        private static GameObject HealthMeter = null!;
        private static Transform MeterTransform = null!;
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
}
