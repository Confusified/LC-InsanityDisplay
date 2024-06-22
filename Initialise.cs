using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using LC_InsanityDisplay.Config;
using LC_InsanityDisplay.ModCompatibility;
using LC_InsanityDisplay.UI;
using static LC_InsanityDisplay.ModCompatibility.CompatibilityList;

namespace LC_InsanityDisplay
{
    //Soft dependencies
    [CompatibleDependency(LethalConfigPatch.ModGUID, typeof(LethalConfigPatch))]
    [BepInDependency(LethalConfigPatch.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(LobbyCompatibilityPatch.ModGUID, typeof(LobbyCompatibilityPatch))]
    [BepInDependency(LobbyCompatibilityPatch.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(LCCrouchHUDCompatibility.ModGUID, typeof(LCCrouchHUDCompatibility))]
    [BepInDependency(LCCrouchHUDCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(GeneralImprovementsCompatibility.ModGUID, typeof(GeneralImprovementsCompatibility))]
    [BepInDependency(GeneralImprovementsCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(An0nPatchesCompatibility.ModGUID, typeof(An0nPatchesCompatibility))]
    [BepInDependency(An0nPatchesCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(EladsHUDCompatibility.ModGUID, typeof(EladsHUDCompatibility))]
    [BepInDependency(EladsHUDCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(HealthMetricsCompatibility.ModGUID, typeof(HealthMetricsCompatibility))]
    [BepInDependency(ModGUIDS.HealthMetrics, BepInDependency.DependencyFlags.SoftDependency)]

    //[CompatibleDependency(HealthMetricsCompatibility.ModGUID, typeof(HealthMetricsCompatibility))]
    [BepInDependency(ModGUIDS.DamageMetrics, BepInDependency.DependencyFlags.SoftDependency)]

    //[CompatibleDependency(HealthMetricsCompatibility.ModGUID, typeof(HealthMetricsCompatibility))]
    [BepInDependency(ModGUIDS.LethalCompanyVR, BepInDependency.DependencyFlags.SoftDependency)]

    //[CompatibleDependency(HealthMetricsCompatibility.ModGUID, typeof(HealthMetricsCompatibility))]
    [BepInDependency(ModGUIDS.InfectedCompany, BepInDependency.DependencyFlags.SoftDependency)]
    //Plugin
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Initialise : BaseUnityPlugin
    {

        public static readonly string configLocation = Utility.CombinePaths(Paths.ConfigPath + "\\" + MyPluginInfo.PLUGIN_GUID[4..].Replace(".", "\\"));
        public static ConfigFile modConfig = new(configLocation + ".cfg", false);

        internal new static ManualLogSource Logger { get; private set; } = null!;

        public void Awake()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
            Logger = Logger;

            ConfigHandler.InitialiseConfig();
            if (!ConfigHandler.ModEnabled.Value) { Logger.LogInfo($"Stopped loading {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION}, as it is disabled through the config file"); return; }

            CompatibleDependencyAttribute.Init(this);
            HookAll();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} loaded");
            return;
        }

        private static void HookAll()
        {
            Logger.LogDebug("Hooking...");
            On.HUDManager.SetSavedValues += HUDInjector.InjectIntoHud;
            On.GameNetcodeStuff.PlayerControllerB.SetPlayerSanityLevel += HUDBehaviour.InsanityValueChanged;
        }
    }
}