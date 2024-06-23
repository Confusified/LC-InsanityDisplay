using BepInEx;
using BepInEx.Bootstrap;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LC_InsanityDisplay.Plugin.ModCompatibility
{
    /// <summary>
    /// This class contains the methods that call compatibility related code
    /// </summary>
    internal class CompatibleDependencyAttribute : BepInDependency
    {
        public static bool IsEladsHudPresent = false;
        public static bool IsLCVRPresent = false;
        //public static bool IsInfectedCompanyPresent = false;
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
