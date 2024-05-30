using InsanityDisplay.Config;
using UnityEngine;

namespace InsanityDisplay.ModCompatibility
{
    public class HealthMetrics_DamageMetricsCompatibility
    {
        private static Vector3 localPositionOffset_Damage = new Vector3(-10f, 0, 0);
        private static Vector3 localPositionOffset_Health = new Vector3(-2f, 0, 0);

        public static void MoveDisplay(bool usingHealthMetrics)
        {

            GameObject MetricDisplay = HUDManager.Instance.PTTIcon.transform.Find("HealthHUDDisplay").gameObject;

            if (MetricDisplay == null)
            {
                string Display = usingHealthMetrics ? "HealthMetrics" : "DamageMetrics";

                Initialise.modLogger.LogError($"{Display}' display wasn't found");
                return;
            }

            MetricDisplay.transform.localPosition += usingHealthMetrics ? localPositionOffset_Health : localPositionOffset_Damage;
        }
    }
}