using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using LC_InsanityDisplay.ModCompatibility;
using LC_InsanityDisplay.Plugin.ModCompatibility;
using LC_InsanityDisplay.Plugin.UI;
using System.Collections.Generic;
using System.Reflection;

namespace LC_InsanityDisplay.Plugin
{
    //Soft dependencies (currently it seems CompatibleDependency doesn't also register it as soft dependency)
    [CompatibleDependency(LethalConfigCompatibility.ModGUID, typeof(LethalConfigCompatibility))]
    [BepInDependency(LethalConfigCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(LobbyCompatibility_Compatibility.ModGUID, typeof(LobbyCompatibility_Compatibility))]
    [BepInDependency(LobbyCompatibility_Compatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(CrouchHUDCompatibility.ModGUID, typeof(CrouchHUDCompatibility))]
    [BepInDependency(CrouchHUDCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(GeneralImprovementsCompatibility.ModGUID, typeof(GeneralImprovementsCompatibility))]
    [BepInDependency(GeneralImprovementsCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(An0nPatchesCompatibility.ModGUID, typeof(An0nPatchesCompatibility))]
    [BepInDependency(An0nPatchesCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    // This is for LethalCompanyPatched, it uses the exact same UI display as An0nPatches
    [CompatibleDependency(An0nPatchesCompatibility.AlternateModGUID, typeof(An0nPatchesCompatibility))]
    [BepInDependency(An0nPatchesCompatibility.AlternateModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(EladsHUDCompatibility.ModGUID, typeof(EladsHUDCompatibility))]
    [BepInDependency(EladsHUDCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(HealthMetricsCompatibility.ModGUID, typeof(HealthMetricsCompatibility))]
    [BepInDependency(HealthMetricsCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(DamageMetricsCompatibility.ModGUID, typeof(DamageMetricsCompatibility))]
    [BepInDependency(DamageMetricsCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(LethalCompanyVRCompatibility.ModGUID, typeof(LethalCompanyVRCompatibility))]
    [BepInDependency(LethalCompanyVRCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(InfectedCompanyCompatibility.ModGUID, typeof(InfectedCompanyCompatibility))]
    [BepInDependency(InfectedCompanyCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]

    [CompatibleDependency(ShyHUDCompatibility.ModGUID, typeof(ShyHUDCompatibility))]
    [BepInDependency(ShyHUDCompatibility.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]
    //Plugin
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Initialise : BaseUnityPlugin
    {
        // InsanityDisplay.Confusified.com.GitHub
        public static readonly string configLocation = Utility.CombinePaths(Paths.ConfigPath + "\\" + MyPluginInfo.PLUGIN_GUID[16..28].Replace(".", "\\") + MyPluginInfo.PLUGIN_GUID[..15]);
        public static ConfigFile modConfig = new(configLocation + ".cfg", false);

        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Initialise Instance { get; private set; } = null!;

        public void Awake()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
            Logger = Logger;
            Instance = this;

            ConfigHandler.InitialiseConfig();
            if (!ConfigHandler.ModEnabled.Value) { Logger.LogInfo($"Stopped loading {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION}, as it is disabled through the config file"); Destroy(this); return; } // I don't know if calling Destroy has any sort of effect

            CompatibleDependencyAttribute.Init(this);
            Hook();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loaded");
            return;
        }

        private static void Hook()
        {
            Logger.LogDebug("Hooking...");
            On.HUDManager.SetSavedValues += HUDInjector.InjectIntoHud;
            On.GameNetcodeStuff.PlayerControllerB.SetPlayerSanityLevel += HUDBehaviour.InsanityValueChanged;
        }
    }

    /// <summary>
    /// This class contains the methods that call compatibility related code
    /// </summary>
    internal class CompatibleDependencyAttribute : BepInDependency
    {
        public static bool IsEladsHudPresent = false;
        public static bool IsLCVRPresent = false;
        public static bool ShyHUDPresent = false;
        public System.Type Handler;

        private const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
        private static IEnumerable<CompatibleDependencyAttribute> attributes = null!;
        /// <summary>
        /// Marks this BepInEx.BaseUnityPlugin as soft dependant on another plugin.
        /// The handler type must have an Initialize() method that will automatically be invoked if the compatible dependency is present.
        /// source: https://discord.com/channels/1168655651455639582/1216761387343151134/1216761387343151134
        /// I have modified it
        /// </summary>
        /// <param name="guid">The GUID of the referenced plugin.</param>
        /// <param name="handlerType">The class type that will handle this compatibility. Must contain a private method called Initialize()</param>
        internal CompatibleDependencyAttribute(string guid, System.Type handlerType) : base(guid, DependencyFlags.SoftDependency)
        {
            Handler = handlerType;
        }

        /// <summary>
        /// Global initializer for this class.
        /// You must call this method from your base plugin Awake method and pass the plugin instance to the source parameter.
        /// This is only called at startup.
        /// </summary>
        /// <param name="source">The source plugin instance with the BepInPlugin attribute.</param>
        internal static void Init(BaseUnityPlugin source)
        {
            attributes = source.GetType().GetCustomAttributes<CompatibleDependencyAttribute>();
            //Initialise all depedencies
            foreach (CompatibleDependencyAttribute attr in attributes)
            {
                InvokeMethodIfFound(attr, "Initialize");
            }
        }
        /// <summary>
        /// Global dependency activator.
        /// This method is called before creating the insanity meter, activating all dependencies.
        /// This is only called when the player joins a lobby
        /// </summary>
        internal static void Activate()
        {
            foreach (CompatibleDependencyAttribute attr in attributes)
            {
                InvokeMethodIfFound(attr, "Start");
            }
        }
        /// <summary>
        /// This will attempt to call a compatibility mod's specific method.
        /// </summary>
        /// <param name="attribute">The target dependency.</param>
        /// <param name="methodToRun">The name of the method that will be attempted to be called.</param>
        private static void InvokeMethodIfFound(CompatibleDependencyAttribute attribute, string methodToRun)
        {
            if (attribute == null) return;
            if (IsModPresent(attribute.DependencyGUID))
            {
                //Initialise.Logger.LogDebug("Found compatible mod: " + attribute.DependencyGUID);
                attribute.Handler.GetMethod(methodToRun, bindingFlags)?.Invoke(null, null);
            }
            //else
            //{
            //Initialise.Logger.LogDebug("Compatibility not found: " + attribute.DependencyGUID);
            //}
        }
        /// <summary>
        /// A method that when called will return if a target mod is currently present.
        /// </summary>
        /// <param name="ModGUID">The GUID of the target mod</param>
        /// <returns>This returns true if the target mod is present.
        /// Returns false if the target mod is not present.</returns>
        internal static bool IsModPresent(string ModGUID)
        {
            return Chainloader.PluginInfos.ContainsKey(ModGUID);
        }
    }
}