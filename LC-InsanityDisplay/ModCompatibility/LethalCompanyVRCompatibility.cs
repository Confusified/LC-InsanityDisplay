// using LCVR;
using UnityEngine;
using System.Collections;
// using LCVR.Player;
using LC_InsanityDisplay.Plugin;

namespace LC_InsanityDisplay.ModCompatibility
{
    public class LethalCompanyVRCompatibility
    {
        internal const string ModGUID = "io.daxcess.lcvr";

        private static void Initialize()
        {
            CompatibleDependencyAttribute.IsLCVRPresent = true;
        }

        /*
                public static IEnumerator EnableVRCompatibility()
                {
                    if (!VRSession.InVR) { yield break; } //Player isn't in VR so no need for the compatibility
                    Transform SprintMeterTransform = null;
                    var meterTransform = MeterHandler.InsanityMeter.transform;
                    meterTransform.SetParent(MeterHandler.vanillaSprintMeter.transform, false); //This way i won't have to manually set it's parent properly
                    yield return new WaitUntil(() => VRSession.Instance != null); //wait until vr instance exists

                    SprintMeterTransform = meterTransform.parent;
                    meterTransform.SetParent(SprintMeterTransform.parent, false); //Get the parent of the stamina meter, to set the position of the insanity meter properly

                    meterTransform.localScale = SprintMeterTransform.localScale * 0.86f; //roughly same size as normal
                    meterTransform.rotation = SprintMeterTransform.rotation;

                    meterTransform.parent.Find("SelfRed").gameObject.transform.localPosition =
                    meterTransform.parent.Find("Self").gameObject.transform.localPosition += new Vector3(0, IconHandler.selfLocalPositionOffset.y / 2, IconHandler.selfLocalPositionOffset.x / 2);

                    meterTransform.localPosition = SprintMeterTransform.localPosition + new Vector3(0, MeterHandler.localPositionOffset.y, MeterHandler.localPositionOffset.x); //why is it like this? i don't know. does it work? yes
                    if (!Plugin.Config.DisableArmHUD.Value) //fix position of meter & self when using arm hud
                    {
                        meterTransform.localPosition -= new Vector3(0, MeterHandler.localPositionOffset.y / 2, MeterHandler.localPositionOffset.x);
                        meterTransform.parent.Find("SelfRed").gameObject.transform.localPosition =
                        meterTransform.parent.Find("Self").gameObject.transform.localPosition -= new Vector3(0, IconHandler.selfLocalPositionOffset.y / 4, IconHandler.selfLocalPositionOffset.x / 4);
                    }
                    meterTransform.SetParent(SprintMeterTransform, true); //shouldn't have any negative effects(?) and will hide when LCVR's hud hides

                }
        */
    }
}
