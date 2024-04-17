using LCVR;
using UnityEngine;
using InsanityDisplay.UI;
using System.Collections;
using LCVR.UI;
using LCVR.Player;

namespace InsanityDisplay.ModCompatibility
{
    public class LethalCompanyVRCompatibility //CURRENT STATE: ARMHUD WORKS, NON-ARMHUD WORKS BUT NOT POSITIONED, SIZED AND ROTATED PROPERLY
    {
        private static Canvas LeftHandCanvas;
        private static VRHUD vrHUD;
        public static IEnumerator EnableVRCompatibility()
        {
            if (!VRSession.InVR) { yield break; } //Player isn't in VR so no need for the compatibility
            Transform SprintMeterTransform = null;
            UIHandler.InsanityMeter.transform.SetParent(UIHandler.vanillaSprintMeter.transform, false); //This way i won't have to manually set it's parent properly
            yield return new WaitUntil(() => VRSession.Instance != null); //wait until vr instance exists
            vrHUD = VRSession.Instance.HUD;

            if (!Plugin.Config.DisableArmHUD.Value) //Attach to left hand if ArmHUD is not disabled
            {
                LeftHandCanvas = vrHUD.LeftHandCanvas.GetComponent<Canvas>();
                UIHandler.InsanityMeter.transform.SetParent(LeftHandCanvas.transform, false);
                SprintMeterTransform = LeftHandCanvas.transform.Find("SprintMeter").gameObject.transform;
            }
            else
            {
                SprintMeterTransform = UIHandler.InsanityMeter.transform.parent;
                UIHandler.InsanityMeter.transform.SetParent(SprintMeterTransform.parent, false); //Get the parent of the stamina meter
            }
            UIHandler.InsanityMeter.transform.localPosition = SprintMeterTransform.localPosition + (UIHandler.localPositionOffset * 0.18f); //+ new Vector3(.55f, .6f, 0);

            if (Plugin.Config.DisableArmHUD.Value) { UIHandler.InsanityMeter.transform.localPosition += new Vector3(Plugin.Config.HUDOffsetX.Value, Plugin.Config.HUDOffsetY.Value, 0); } //add offset if not using armHUD
            UIHandler.InsanityMeter.transform.localScale = SprintMeterTransform.localScale * 0.86f; //roughly same size as normal
            UIHandler.InsanityMeter.transform.rotation = SprintMeterTransform.rotation;

            Initialise.modLogger.LogInfo($"LOCALPOS:\n{SprintMeterTransform.localPosition} {UIHandler.InsanityMeter.transform.localPosition}");
            Initialise.modLogger.LogInfo($"ROTATION:\n{SprintMeterTransform.rotation} {UIHandler.InsanityMeter.transform.rotation}");
        }
    }
}
