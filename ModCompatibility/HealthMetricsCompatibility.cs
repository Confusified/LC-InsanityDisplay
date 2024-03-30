using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InsanityDisplay.ModCompatibility
{
    public class HealthMetricsCompatibility
    {
        private static Vector3 localPositionOffset = new Vector3(-2, 0, 0);

        public static void MoveHealthMetrics()
        {
            GameObject HealthMetricsDisplay = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/BottomMiddle/PTTIcon/HealthHUDDisplay").gameObject;

            if (HealthMetricsDisplay == null) { Initialise.modLogger.LogError("LCCrouchHud's icon wasn't found"); return; }

            HealthMetricsDisplay.transform.localPosition += localPositionOffset;
        }
    }
}