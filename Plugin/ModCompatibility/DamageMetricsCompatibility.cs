using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LC_InsanityDisplay.Plugin.ModCompatibility
{

    /// <summary>
    /// Responsible for the compatibility of DamageMetrics
    /// </summary>
    public class DamageMetricsCompatibility
    {
        //NOTE: DamageMetrics and HealthMetrics don't work properly together, I'm not gonna fix it until there's a bug report or notice people use both at the same time
        internal const string ModGUID = "Matsuura.TestAccount666.DamageMetrics";

        private static GameObject DamageMeter = null!;
        private static Transform MeterTransform = null!;
        private static Vector3 localPositionOffset = new(-10f, 0, 0);
        private static Vector3 localPosition = Vector3.zero;
        private static bool DisableDamageMetrics;

        private static void Initialize()
        {
            DisableDamageMetrics = CompatibleDependencyAttribute.IsEladsHudPresent || CompatibleDependencyAttribute.IsLCVRPresent;
            if (DisableDamageMetrics) return;

            ConfigHandler.Compat.DamageMetrics.SettingChanged += UpdateHealthMeter;
        }

        private static void Start()
        {
            if (DisableDamageMetrics) return;
            DamageMeter = HealthMetricsCompatibility.Shared_FetchHUDDisplay();
            if (!DamageMeter) return;
            MeterTransform = DamageMeter.transform;
            localPosition = DamageMeter.transform.localPosition;

            UpdateHealthMeter();
        }

        internal static void UpdateHealthMeter(object sender = null!, EventArgs e = null!)
        {
            if (DamageMeter == null || MeterTransform == null || localPosition == Vector3.zero) return; //can't update it if it ain't there

            MeterTransform.SetLocalPositionAndRotation(localPosition: ConfigHandler.Compat.DamageMetrics.Value ? localPosition + localPositionOffset : localPosition, localRotation: MeterTransform.localRotation);
        }
    }
}
