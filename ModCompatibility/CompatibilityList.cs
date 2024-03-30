using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace InsanityDisplay.ModCompatibility
{
    public class CompatibilityList //perhaps misleading name, but here are the variables for any compatibility
    {
        public const string LCCrouch_GUID = "LCCrouchHUD";
        public static bool LCCrouch_Installed;

        public const string HealthMetrics_GUID = "Matsuura.HealthMetrics";
        public static bool HealthMetrics_Installed;

        public const string EladsHUD_GUID = "me.eladnlg.customhud";
        public static bool EladsHUD_Installed;
        //An0n patches?
    }
}