using BepInEx;
using BepInEx.Bootstrap;
using System.Collections.Generic;
using System.Reflection;

namespace LC_InsanityDisplay.ModCompatibility
{
    /// <summary>
    /// This class contains the methods that call compatibility related code
    /// </summary>
    internal class CompatibleDependencyAttribute : BepInDependency
    {
        public System.Type Handler;
        private const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
        internal static IEnumerable<CompatibleDependencyAttribute> attributes = null!;
        /// <summary>
        /// Marks this BepInEx.BaseUnityPlugin as soft dependant on another plugin.
        /// The handler type must have an Initialize() method that will automatically be invoked if the compatible dependency is present.
        /// source: https://discord.com/channels/1168655651455639582/1216761387343151134/1216761387343151134
        /// I have modified it
        /// </summary>
        /// <param name="guid">The GUID of the referenced plugin.</param>
        /// <param name="handlerType">The class type that will handle this compatibility. Must contain a private method called Initialize()</param>
        public CompatibleDependencyAttribute(string guid, System.Type handlerType) : base(guid, DependencyFlags.SoftDependency)
        {
            Handler = handlerType;
        }

        /// <summary>
        /// Global initializer for this class.
        /// You must call this method from your base plugin Awake method and pass the plugin instance to the source parameter.
        /// </summary>
        /// <param name="source">The source plugin instance with the BepInPlugin attribute.</param>
        public static void Init(BepInEx.BaseUnityPlugin source)
        {
            attributes = source.GetType().GetCustomAttributes<CompatibleDependencyAttribute>();
            foreach (CompatibleDependencyAttribute attr in attributes)
            {
                RunMethodIfFound(attr, "Initialize");
            }
        }

        public static void Activate()
        {
            foreach (CompatibleDependencyAttribute attr in attributes)
            {
                RunMethodIfFound(attr, "Start");
            }
        }

        private static void RunMethodIfFound(CompatibleDependencyAttribute attribute, string methodToRun)
        {
            Initialise.Logger.LogDebug($"{attribute.DependencyGUID} => Found: {IsModPresent(attribute.DependencyGUID)}");
            if (IsModPresent(attribute.DependencyGUID))
            {
                Initialise.Logger.LogDebug("Found compatible mod: " + attribute.DependencyGUID);
                attribute.Handler.GetMethod(methodToRun, bindingFlags)?.Invoke(null, null);
            }
            //else
            //{
            // Initialise.Logger.LogDebug("Compatibility not found: " + attr.DependencyGUID);
            //}
        }

        internal static bool IsModPresent(string ModGUID)
        {
            return Chainloader.PluginInfos.ContainsKey(ModGUID);
        }
    }
}
