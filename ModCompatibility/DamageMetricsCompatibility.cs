using UnityEngine;

namespace InsanityDisplay.ModCompatibility
{
    public class DamageMetricsCompatibility
    {
        private static Vector3 localPositionOffset = new Vector3(-10f, 0, 0);

        public static void MoveDamageHUD()
        {
            GameObject DamageDisplay = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/BottomMiddle/PTTIcon/HealthHUDDisplay").gameObject;

            if (DamageDisplay == null) { Initialise.modLogger.LogError("HealthMetrics' health display wasn't found"); return; }

            DamageDisplay.transform.localPosition += localPositionOffset;
        }
    }
}