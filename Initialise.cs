using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using InsanityDisplay.Config;
using BepInEx.Bootstrap;
using static InsanityDisplay.ModCompatibility.CompatibilityList;
using System;
using System.Collections.Generic;

namespace InsanityDisplay
{
    //Soft dependencies
    [BepInDependency(ModGUIDS.LCCrouchHUD, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.An0nPatches, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.EladsHUD, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.GeneralImprovements, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.HealthMetrics, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.DamageMetrics, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.LobbyCompatibility, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.LethalConfig, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.LethalCompanyVR, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModGUIDS.InfectedCompany, BepInDependency.DependencyFlags.SoftDependency)]
    //Plugin
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Initialise : BaseUnityPlugin
    {
        private const string modGUID = "com.Confusified.InsanityDisplay";
        private const string modName = "InsanityDisplay";
        private const string modVersion = "1.2.0";

        public static readonly string configLocation = Utility.CombinePaths(Paths.ConfigPath + "\\" + modGUID.Substring(4).Replace(".", "\\"));
        public static ConfigFile modConfig = new ConfigFile(configLocation + ".cfg", false);

        private readonly Harmony _Harmony = new Harmony(modGUID);
        public static ManualLogSource modLogger;

        public void Awake()
        {
            modLogger = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            modLogger = Logger;

            ConfigHandler.InitialiseConfig();

            CheckForModCompatibility();

            _Harmony.PatchAll(Assembly.GetExecutingAssembly());
            modLogger.LogInfo($"{modName} {modVersion} loaded");
            return;
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
                { ModGUIDS.LobbyCompatibility, () => ModCompatibility.LobbyCompatibilityPatch.UseLobbyCompatibility(modGUID, modVersion) },
                { ModGUIDS.LethalConfig, () => ModCompatibility.LethalConfigPatch.SetLethalConfigEntries() },
                { ModGUIDS.LethalCompanyVR, () => ModInstalled.LethalCompanyVR = true },
                { ModGUIDS.InfectedCompany, () => ModInstalled.InfectedCompany = true }
             };

            foreach (var modAction in modActions)
            {
                if (Chainloader.PluginInfos.ContainsKey(modAction.Key))
                {
                    modAction.Value.Invoke();
                    modLogger.LogDebug($"Found {modAction.Key}");
                }
            }
        }
    }
}