using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using InsanityDisplay.Config;
using BepInEx.Bootstrap;
using static InsanityDisplay.ModCompatibility.CompatibilityList;

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
    // [BepInDependency(ModGUIDS.ShyHUD, BepInDependency.DependencyFlags.SoftDependency)]
    //Plugin
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Initialise : BaseUnityPlugin
    {
        private const string modGUID = "com.Confusified.InsanityDisplay";
        private const string modName = "InsanityDisplay";
        private const string modVersion = "1.1.5";

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

        private static void CheckForModCompatibility() //is there a better way for this? this is nasty
        {
            if (Chainloader.PluginInfos.ContainsKey(ModGUIDS.LCCrouchHUD))
            {
                ModInstalled.LCCrouchHUD = true;
                modLogger.LogDebug("Found LCCrouchHUD");
            }
            if (Chainloader.PluginInfos.ContainsKey(ModGUIDS.EladsHUD))
            {
                ModInstalled.EladsHUD = true;
                modLogger.LogDebug("Found Elad's HUD");
            }
            if (Chainloader.PluginInfos.ContainsKey(ModGUIDS.An0nPatches))
            {
                ModInstalled.An0nPatches = true;
                modLogger.LogDebug("Found An0n Patches");
            }
            if (Chainloader.PluginInfos.ContainsKey(ModGUIDS.GeneralImprovements))
            {
                ModInstalled.GeneralImprovements = true;
                modLogger.LogDebug("Found GeneralImprovements");
            }
            if (Chainloader.PluginInfos.ContainsKey(ModGUIDS.HealthMetrics))
            {
                ModInstalled.HealthMetrics = true;
                modLogger.LogDebug("Found HealthMetrics");
            }
            if (Chainloader.PluginInfos.ContainsKey(ModGUIDS.DamageMetrics))
            {
                ModInstalled.DamageMetrics = true;
                modLogger.LogDebug("Found DamageMetrics");
            }
            if (Chainloader.PluginInfos.ContainsKey(ModGUIDS.LobbyCompatibility))
            {
                ModCompatibility.LobbyCompatibilityPatch.UseLobbyCompatibility(modGUID, modVersion);
                modLogger.LogDebug("Found LobbyCompatibility");
            }
            if (Chainloader.PluginInfos.ContainsKey(ModGUIDS.LethalConfig))
            {
                ModCompatibility.LethalConfigPatch.SetLethalConfigEntries();
                modLogger.LogDebug("Found LethalConfig");
            }
            if (Chainloader.PluginInfos.ContainsKey(ModGUIDS.LethalCompanyVR))
            {
                ModInstalled.LethalCompanyVR = true;
                modLogger.LogDebug("Found LCVR");
            }
            if (Chainloader.PluginInfos.ContainsKey(ModGUIDS.InfectedCompany))
            {
                ModInstalled.InfectedCompany = true;
                modLogger.LogDebug("Found InfectedCompany");
            }
            /* if (Chainloader.PluginInfos.ContainsKey(ModGUIDS.ShyHUD))
             {
                 ModInstalled.ShyHUD = true;
                 modLogger.LogDebug("Found ShyHUD");
             }
            */
        }
    }
}