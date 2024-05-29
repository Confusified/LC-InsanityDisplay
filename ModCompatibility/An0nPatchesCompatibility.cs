using UnityEngine;
using static InsanityDisplay.UI.UIHandler;

namespace InsanityDisplay.ModCompatibility
{
    public class An0nPatchesCompatibility
    {
        private static Vector3 localPositionOffset = new Vector3(3f, 15f, 0);

        public static void MoveTextHUD()
        {

            GameObject An0nTextHUD = TopLeftCornerHUD.transform.Find("HPSP").gameObject;

            if (An0nTextHUD == null) { Initialise.modLogger.LogError("An0nTextHUD's HUD wasn't found"); return; }

            An0nTextHUD.transform.localPosition += localPositionOffset;
        }
    }
}