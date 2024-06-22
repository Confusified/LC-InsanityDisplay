using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using BepInEx.Bootstrap;
using System;
using System.Collections.Generic;
using static LC_InsanityDisplay.ModCompatibility.CompatibilityList;
using LC_InsanityDisplay.ModCompatibility;
using LC_InsanityDisplay.Config;
using LC_InsanityDisplay.UI;

namespace LC_InsanityDisplay
{
    //Plugin
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    //Soft dependencies
    [CompatibleDependency(LethalConfigPatch.ModGUID, typeof(LethalConfigPatch))]
    [CompatibleDependency(LobbyCompatibilityPatch.ModGUID, typeof(LobbyCompatibilityPatch))]
    [CompatibleDependency(LCCrouchHUDCompatibility.ModGUID, typeof(LCCrouchHUDCompatibility))]
    [CompatibleDependency(GeneralImprovementsCompatibility.ModGUID, typeof(GeneralImprovementsCompatibility))]
    //[CompatibleDependency(EladsHUDCompatibility.ModGUID, typeof(EladsHUDCompatibility))]
    [BepInDependency(ModGUIDS.An0nPatches, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.GeneralImprovements, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.HealthMetrics, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.DamageMetrics, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.LethalCompanyVR, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.InfectedCompany, BepInDependency.DependencyFlags.SoftDependency)]
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