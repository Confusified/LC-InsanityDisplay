using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using InsanityDisplay.Config;

namespace InsanityDisplay
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Initialise : BaseUnityPlugin
    {
        private const string modGUID = "com.Confusified.InsanityDisplay";
        private const string modName = "InsanityDisplay";
        private const string modVersion = "1.0.0";

        private static readonly string configLocation = Utility.CombinePaths(Paths.ConfigPath + "\\" + modGUID.Replace(".", "\\"));
        public static ConfigFile modConfig = new ConfigFile(configLocation + ".cfg", false);

        private readonly Harmony _Harmony = new Harmony(modGUID);
        public static ManualLogSource modLogger;

        public void Awake()
        {
            modLogger = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            modLogger = Logger;

            ConfigHandler.InitialiseConfig();

            _Harmony.PatchAll(Assembly.GetExecutingAssembly());
            modLogger.LogInfo($"{modName} {modVersion} loaded");
            return;
        }
    }
}