using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using System.Text;
using System.IO;

namespace FramerateSlider
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Initialise : BaseUnityPlugin
    {
        private const string modGUID = "Confusified.FramerateSlider";
        private const string modName = "Framerate Slider";
        private const string modVersion = "1.0.4";

        private static readonly string configLocation = Utility.CombinePaths(Paths.ConfigPath + "\\" + modGUID.Replace(".", "\\"));
        public static string privateConfigLocation = configLocation + ".private";
        private static ConfigFile modConfig = new ConfigFile(configLocation + ".cfg", false);

        public class DefaultConfig
        {
            //User customisable
            public static bool ModEnabled = true;
            public static int FramerateLimit = 60;

            //Private
            public static int LastLoggedIndex = -1;
            public static int ConfigVersion = 1;
            public static bool FirstTime = true;
        }

        public class ModSettings
        {
            //User customisable
            public static ConfigEntry<bool> ModEnabled;
            public static ConfigEntry<int> FramerateLimit;

            //Private
            public static int LastLoggedIndex;
            public static int ConfigVersion;
            public static bool FirstTime;
        }

        private readonly Harmony _Harmony = new Harmony(modGUID);
        public static ManualLogSource modLogger;
        public void Awake()
        {
            modLogger = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            modLogger = Logger;

            ES3.Init();
            SetDefaultConfigValues();

            if (ModSettings.FirstTime)
            {
                ModSettings.ConfigVersion = 0;
                ModSettings.FirstTime = false;
            }

            UpdateConfig();

            if (ModSettings.ModEnabled.Value == true)
            {
                _Harmony.PatchAll(Assembly.GetExecutingAssembly());
                modLogger.LogInfo($"{modName} {modVersion} loaded");
                return;
            }

            modLogger.LogInfo($"{modName} {modVersion} did not load, it is disabled in the config");
            return;
        }

        private void SetDefaultConfigValues()
        {
            LoadPrivateConfig(); //Loads or creates private config
            ModSettings.ModEnabled = modConfig.Bind<bool>("Mod Settings", "Enabled", DefaultConfig.ModEnabled, "Change the framerate selector from a dropdown into a slider");
            ModSettings.FramerateLimit = modConfig.Bind<int>("Mod Settings", "Framerate Limit", DefaultConfig.FramerateLimit, "The maximum amount of frames that your game is allowed to render");
            ES3.CacheFile(privateConfigLocation); //makes it faster (I THINK)
            return;
        }

        private void UpdateConfig()
        {
            if (ModSettings.ConfigVersion == DefaultConfig.ConfigVersion) { return; }
            int[] oldInts = { ModSettings.FramerateLimit.Value, ModSettings.LastLoggedIndex, ModSettings.ConfigVersion };
            bool oldModEnabled = ModSettings.ModEnabled.Value;

            //Clear files and variables
            modConfig.Clear();
            modConfig = null;
            File.WriteAllText(configLocation + ".cfg", ""); //Clears config (private one is unnecessary because you can't edit it manually
            modConfig = new ConfigFile(configLocation + ".cfg", false);

            ModSettings.FramerateLimit = null; //set to null otherwise it'll cause issues
            ModSettings.ModEnabled = null;
            ModSettings.LastLoggedIndex = -999; //can't be set to null so just set to anything (doesn't matter)
            ModSettings.ConfigVersion = -999;

            //Set default values
            SetDefaultConfigValues();

            //Restore old values
            ModSettings.FramerateLimit.Value = oldInts[0];
            ModSettings.LastLoggedIndex = oldInts[1];
            ModSettings.ModEnabled.Value = oldModEnabled;
            ModSettings.ConfigVersion = DefaultConfig.ConfigVersion;
            ES3.Save("ConfigVersion", ModSettings.ConfigVersion, privateConfigLocation); //not really needed because it won't change mid-game
            ES3.Save("FirstTime", ModSettings.FirstTime, privateConfigLocation);

            StringBuilder updatedConfigOutput = new StringBuilder("Updated config file from version 0 to 1");
            updatedConfigOutput.Replace("0", oldInts[2].ToString());
            updatedConfigOutput.Replace("1", ModSettings.ConfigVersion.ToString());
            modLogger.LogInfo(updatedConfigOutput);
            return;
        }

        public static void UpdatePrivateConfig()
        {
            ES3.Save("LastLoggedIndex", ModSettings.LastLoggedIndex, privateConfigLocation);
            LoadPrivateConfig(); //reload the config
            return;
        }

        private static void LoadPrivateConfig()
        {
            ModSettings.LastLoggedIndex = ES3.Load("LastLoggedIndex", privateConfigLocation, DefaultConfig.LastLoggedIndex);
            ModSettings.ConfigVersion = ES3.Load("ConfigVersion", privateConfigLocation, DefaultConfig.ConfigVersion);
            ModSettings.FirstTime = ES3.Load("FirstTime", privateConfigLocation, DefaultConfig.FirstTime);
            return;
        }
    }
}
