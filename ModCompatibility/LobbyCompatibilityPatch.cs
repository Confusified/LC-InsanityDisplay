using LobbyCompatibility.Enums;
using LobbyCompatibility.Features;
using System;

namespace LC_InsanityDisplay.ModCompatibility
{
    public class LobbyCompatibilityPatch
    {
        internal const string ModGUID = "BMX.LobbyCompatibility";
        private static void Initialize()
        {
            PluginHelper.RegisterPlugin(guid: MyPluginInfo.PLUGIN_GUID, version: Version.Parse(MyPluginInfo.PLUGIN_VERSION), CompatibilityLevel.ClientOnly, VersionStrictness.None);
        }
    }
}
