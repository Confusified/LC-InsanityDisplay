using LC_InsanityDisplay;
using LC_InsanityDisplay.Config;
using UnityEngine;
using static LC_InsanityDisplay.UI.MeterHandler;

namespace LC_InsanityDisplay.ModCompatibility
{
    public class LCCrouchHUDCompatibility
    {
        private static Vector3 positionToLocal = new Vector3(-16.2386f, -20.2468f, -12.0928f); //Essentially fixes issues that may occur with resolution related stuff
        private static Vector3 localPositionOffset = new Vector3(0, 4, 0);

        public static void MoveCrouchHUD()
        {

            GameObject CrouchHUDIcon = TopLeftCornerHUD.transform.Find("Self/CrouchIcon")?.gameObject;

            if (CrouchHUDIcon == null) { Initialise.Logger.LogError("LCCrouchHud's icon wasn't found"); return; }

            bool CrouchHUDCompat = ConfigHandler.Compat.LCCrouchHUD.Value;
            if (CrouchHUDCompat && CrouchHUDIcon.transform.localPosition != positionToLocal + localPositionOffset || !CrouchHUDCompat && CrouchHUDIcon.transform.localPosition != positionToLocal) //update if hud is positioned incorrectly
            {
                CrouchHUDIcon.transform.localPosition = CrouchHUDCompat && ConfigHandler.ModEnabled.Value ? positionToLocal + localPositionOffset : positionToLocal;
            }
        }
    }
}