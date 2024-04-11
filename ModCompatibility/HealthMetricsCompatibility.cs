using UnityEngine;

namespace InsanityDisplay.ModCompatibility
{
    public class HealthMetricsCompatibility
    {
        private static Vector3 localPositionOffset = new Vector3(-2f, 0, 0);

        public static void MoveHealthHUD()
        {
            GameObject HealthDisplay = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/BottomMiddle/PTTIcon/HealthHUDDisplay").gameObject;

            if (HealthDisplay == null) { Initialise.modLogger.LogError("HealthMetrics' health display wasn't found"); return; }

            HealthDisplay.transform.localPosition += localPositionOffset;
        }
    }
}