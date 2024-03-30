using UnityEngine;

namespace InsanityDisplay.ModCompatibility
{
    public class LCCrouchHUDCompatibility
    {
        private static Vector3 localPositionOffset = new Vector3(0, 4, 0);

        public static void MoveCrouchHUD()
        {
            GameObject CrouchHUDIcon = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner/Self/CrouchIcon").gameObject;

            if (CrouchHUDIcon == null) { Initialise.modLogger.LogError("LCCrouchHud's icon wasn't found"); return; }

            CrouchHUDIcon.transform.localPosition += localPositionOffset;
        }
    }
}