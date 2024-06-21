using LC_InsanityDisplay;
using LC_InsanityDisplay.Config;
using UnityEngine;

namespace LC_InsanityDisplay.ModCompatibility
{
    public class HealthMetrics_DamageMetricsCompatibility : CompatBase
    {
        public static HealthMetrics_DamageMetricsCompatibility Instance { get; private set; } = null!;
        public new const string ModGUID = "";
        private static Vector3 localPositionOffset_Damage = new Vector3(-10f, 0, 0);
        private static Vector3 localPositionOffset_Health = new Vector3(-2f, 0, 0);
        private static Vector3 localPosition = Vector3.zero;

        private static void Initialize()
        {
            Instance = new() { Installed = true };
        }

        public static void MoveDisplay(bool usingHealthMetrics)
        {

            GameObject MetricDisplay = HUDManager.Instance.PTTIcon.transform.Find("HealthHUDDisplay").gameObject;

            if (MetricDisplay == null)
            {
                string Display = usingHealthMetrics ? "HealthMetrics" : "DamageMetrics";

                Initialise.Logger.LogError($"{Display}' display wasn't found");
                return;
            }
            localPosition = localPosition == Vector3.zero ? MetricDisplay.transform.localPosition : localPosition;

            //if mod is enabled and     using health metrics and not positioned correctly or if not using health metrics and not positioned correctly
            bool HealthCompat = ConfigHandler.Compat.HealthMetrics.Value;
            bool DamageCompat = ConfigHandler.Compat.DamageMetrics.Value;
            if (ConfigHandler.ModEnabled.Value && (usingHealthMetrics && HealthCompat && MetricDisplay.transform.localPosition != localPosition + localPositionOffset_Health || !usingHealthMetrics && DamageCompat && MetricDisplay.transform.localPosition != localPosition + localPositionOffset_Damage)) //update if hud is positioned incorrectly
            {
                MetricDisplay.transform.localPosition = usingHealthMetrics ? localPosition + localPositionOffset_Health : localPosition + localPositionOffset_Damage;
            }
            else if (!ConfigHandler.ModEnabled.Value || !HealthCompat && usingHealthMetrics || !DamageCompat && !usingHealthMetrics) //can create overlap issue when mod is disabled because of the icon being centered
            {
                MetricDisplay.transform.localPosition = localPosition;
            }
        }
    }
}