using LobbyCompatibility.Enums;
using LobbyCompatibility.Features;
using System;

namespace LC_InsanityDisplay.ModCompatibility
{
    public class LobbyCompatibilityPatch
    {
        private static void Initialize(string modGUID, string modVersion)
        {
            PluginHelper.RegisterPlugin(modGUID, Version.Parse(modVersion), CompatibilityLevel.ClientOnly, VersionStrictness.None);
        }
    }
}
