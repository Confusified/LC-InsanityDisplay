using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using BepInEx.Bootstrap;
using System;
using System.Collections.Generic;
using static LC_InsanityDisplay.ModCompatibility.CompatibilityList;
using LC_InsanityDisplay.ModCompatibility;
using LC_InsanityDisplay.Config;
using LC_InsanityDisplay.UI;

namespace LC_InsanityDisplay
{
    //Soft dependencies
    [CompatibleDependency(ModGUIDS.LethalConfig, typeof(LethalConfigPatch))] //New system to use for dependencies
    [CompatibleDependency(ModGUIDS.LobbyCompatibility, typeof(LobbyCompatibilityPatch))]
    [BepInDependency(ModGUIDS.LCCrouchHUD, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.An0nPatches, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.EladsHUD, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.GeneralImprovements, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.HealthMetrics, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.DamageMetrics, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.LethalCompanyVR, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.InfectedCompany, BepInDependency.DependencyFlags.SoftDependency)]
    //Plugin
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Initialise : BaseUnityPlugin
    {

        public static readonly string configLocation = Utility.CombinePaths(Paths.ConfigPath + "\\" + MyPluginInfo.PLUGIN_GUID[4..].Replace(".", "\\"));
        public static ConfigFile modConfig = new ConfigFile(configLocation + ".cfg", false);

        internal new static ManualLogSource Logger { get; private set; } = null!;

        public void Awake()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
            Logger = Logger;

            ConfigHandler.InitialiseConfig();
            if (!ConfigHandler.ModEnabled.Value) { Logger.LogInfo($"Stopped loading {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION}, as it is disabled through the config file"); return; }
            //eventually get rid of checkformodcompatibility in favour of the attribute one (incredibly easy to be fair)
            CheckForModCompatibility();
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

        private static void CheckForModCompatibility()
        {
            var modActions = new Dictionary<string, Action>
            {
                { ModGUIDS.LCCrouchHUD, () => ModInstalled.LCCrouchHUD = true },
                { ModGUIDS.EladsHUD, () => ModInstalled.EladsHUD = true },
                { ModGUIDS.An0nPatches, () => ModInstalled.An0nPatches = true },
                { ModGUIDS.GeneralImprovements, () => ModInstalled.GeneralImprovements = true },
                { ModGUIDS.HealthMetrics, () => ModInstalled.HealthMetrics = true },
                { ModGUIDS.DamageMetrics, () => ModInstalled.DamageMetrics = true },
                { ModGUIDS.LethalCompanyVR, () => ModInstalled.LethalCompanyVR = true },
                { ModGUIDS.InfectedCompany, () => ModInstalled.InfectedCompany = true }
             };

            foreach (var modAction in modActions)
            {
                if (Chainloader.PluginInfos.ContainsKey(modAction.Key))
                {
                    modAction.Value.Invoke();
                    Logger.LogDebug($"Found {modAction.Key}");
                }
            }
        }
    }
}