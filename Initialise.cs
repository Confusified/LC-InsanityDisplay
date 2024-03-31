﻿using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using InsanityDisplay.Config;
using BepInEx.Bootstrap;
using static InsanityDisplay.ModCompatibility.CompatibilityList;

namespace InsanityDisplay
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency(LCCrouch_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(HealthMetrics_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(EladsHUD_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class Initialise : BaseUnityPlugin
    {
        private const string modGUID = "com.Confusified.InsanityDisplay";
        private const string modName = "InsanityDisplay";
        private const string modVersion = "1.0.0";

        private static readonly string configLocation = Utility.CombinePaths(Paths.ConfigPath + "\\" + modGUID.Replace(".", "\\"));
        public static ConfigFile modConfig = new ConfigFile(configLocation.Substring(4) + ".cfg", false);

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
            if (Chainloader.PluginInfos.ContainsKey(LCCrouch_GUID))
            {
                LCCrouch_Installed = true;
                modLogger.LogInfo("Enabling LCCrouchHUD compatibility");
            }
            if (Chainloader.PluginInfos.ContainsKey(EladsHUD_GUID))
            {
                EladsHUD_Installed = true;
                modLogger.LogInfo("Enabling Elad's HUD compatibility");
            }
            if (Chainloader.PluginInfos.ContainsKey(Oxygen_GUID))
            {
                Oxygen_Installed = true;
                modLogger.LogInfo("Enabling Oxygen compatibility");
            }
        }
    }
}