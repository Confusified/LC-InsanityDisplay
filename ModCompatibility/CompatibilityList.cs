namespace InsanityDisplay.ModCompatibility
{
    public class CompatibilityList //perhaps misleading name, but here are the variables for any compatibility
    {
        public class ModGUIDS
        {
            public const string LCCrouchHUD = "LCCrouchHUD";
            public const string EladsHUD = "me.eladnlg.customhud";
            public const string An0nPatches = "com.an0n.patch";
            public const string GeneralImprovements = "ShaosilGaming.GeneralImprovements";
            public const string HealthMetrics = "Matsuura.HealthMetrics";
            public const string DamageMetrics = "Matsuura.TestAccount666.DamageMetrics";
            public const string LobbyCompatibility = "BMX.LobbyCompatibility"; //doesnt have variable in ModInstalled because i won't allow it to be toggled
            public const string LethalConfig = "ainavt.lc.lethalconfig"; //doesnt have variable in ModInstalled because i won't allow it to be toggled
            public const string LethalCompanyVR = "io.daxcess.lcvr";
            public const string InfectedCompany = "InfectedCompany.InfectedCompany";
            //    public const string ShyHUD = "ShyHUD";
        }
        public class ModInstalled
        {
            public static bool LCCrouchHUD;
            public static bool EladsHUD;
            public static bool An0nPatches;
            public static bool GeneralImprovements;
            public static bool HealthMetrics;
            public static bool DamageMetrics;
            public static bool LethalCompanyVR;
            public static bool InfectedCompany;
            //    public static bool ShyHUD;
        }

    }
}