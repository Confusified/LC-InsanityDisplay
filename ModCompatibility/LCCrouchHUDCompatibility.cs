using UnityEngine;

namespace InsanityDisplay.ModCompatibility
{
    public class LCCrouchHUDCompatibility
    {
        private static Vector3 positionToLocal = new Vector3(-16.2386f, -20.2468f, -12.0928f);
        private static Vector3 localPositionOffset = new Vector3(0, 4, 0);

        public static void MoveCrouchHUD()
        {
            GameObject CrouchHUDIcon = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner/Self/CrouchIcon").gameObject;

            if (CrouchHUDIcon == null) { Initialise.modLogger.LogError("LCCrouchHud's icon wasn't found"); return; }
            //translate position offset to localPosition (-16.2386 -20.2468 -12.0928)
            CrouchHUDIcon.transform.localPosition = positionToLocal + localPositionOffset;
        }
    }
}