using InsanityDisplay.Config;
using UnityEngine;

namespace InsanityDisplay.ModCompatibility
{
    public class HealthMetrics_DamageMetricsCompatibility
    {
        private static Vector3 localPositionOffset_Damage = new Vector3(-10f, 0, 0);
        private static Vector3 localPositionOffset_Health = new Vector3(-2f, 0, 0);
        private static Vector3 localPosition = Vector3.zero;

        public static void MoveDisplay(bool usingHealthMetrics)
        {

            GameObject MetricDisplay = HUDManager.Instance.PTTIcon.transform.Find("HealthHUDDisplay").gameObject;

            if (MetricDisplay == null)
            {
                string Display = usingHealthMetrics ? "HealthMetrics" : "DamageMetrics";

                Initialise.modLogger.LogError($"{Display}' display wasn't found");
                return;
            }
            localPosition = localPosition == Vector3.zero ? MetricDisplay.transform.localPosition : localPosition;

            //if mod is enabled and     using health metrics and not positioned correctly or if not using health metrics and not positioned correctly
            bool HealthCompat = ConfigSettings.Compat.HealthMetrics.Value;
            bool DamageCompat = ConfigSettings.Compat.DamageMetrics.Value;
            if (ConfigSettings.ModEnabled.Value && ((usingHealthMetrics && HealthCompat && MetricDisplay.transform.localPosition != (localPosition + localPositionOffset_Health)) || (!usingHealthMetrics && DamageCompat && MetricDisplay.transform.localPosition != (localPosition + localPositionOffset_Damage)))) //update if hud is positioned incorrectly
            {
                MetricDisplay.transform.localPosition = usingHealthMetrics ? localPosition + localPositionOffset_Health : localPosition + localPositionOffset_Damage;
            }
            else if (!ConfigSettings.ModEnabled.Value || (!HealthCompat && usingHealthMetrics) || (!DamageCompat && !usingHealthMetrics)) //can create overlap issue when mod is disabled because of the icon being centered
            {
                MetricDisplay.transform.localPosition = localPosition;
            }
        }
    }
}