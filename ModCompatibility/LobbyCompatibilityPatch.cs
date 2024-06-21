using LobbyCompatibility.Enums;
using LobbyCompatibility.Features;
using System;

namespace LC_InsanityDisplay.ModCompatibility
{
    public class LobbyCompatibilityPatch : CompatBase
    {
        internal static LobbyCompatibilityPatch Instance { get; private set; } = null!;
        internal new const string ModGUID = "BMX.LobbyCompatibility";
        private static void Initialize()
        {
            Instance = new() { Installed = true };
            PluginHelper.RegisterPlugin(guid: MyPluginInfo.PLUGIN_GUID, version: Version.Parse(MyPluginInfo.PLUGIN_VERSION), CompatibilityLevel.ClientOnly, VersionStrictness.None);
        }
    }
}
