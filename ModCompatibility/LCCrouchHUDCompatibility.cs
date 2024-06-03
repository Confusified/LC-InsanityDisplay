using InsanityDisplay.Config;
using UnityEngine;
using static InsanityDisplay.UI.MeterHandler;

namespace InsanityDisplay.ModCompatibility
{
    public class LCCrouchHUDCompatibility
    {
        private static Vector3 positionToLocal = new Vector3(-16.2386f, -20.2468f, -12.0928f); //Essentially fixes issues that may occur with resolution related stuff
        private static Vector3 localPositionOffset = new Vector3(0, 4, 0);

        public static void MoveCrouchHUD()
        {

            GameObject CrouchHUDIcon = TopLeftCornerHUD.transform.Find("Self/CrouchIcon")?.gameObject;

            if (CrouchHUDIcon == null) { Initialise.modLogger.LogError("LCCrouchHud's icon wasn't found"); return; }

            bool CrouchHUDCompat = ConfigSettings.Compat.LCCrouchHUD.Value;
            if ((CrouchHUDCompat && CrouchHUDIcon.transform.localPosition != (positionToLocal + localPositionOffset)) || (!CrouchHUDCompat && CrouchHUDIcon.transform.localPosition != positionToLocal)) //update if hud is positioned incorrectly
            {
                CrouchHUDIcon.transform.localPosition = CrouchHUDCompat && ConfigSettings.ModEnabled.Value ? positionToLocal + localPositionOffset : positionToLocal;
            }
        }
    }
}