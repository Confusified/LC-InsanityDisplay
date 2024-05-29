using LobbyCompatibility.Enums;
using LobbyCompatibility.Features;
using System;

namespace InsanityDisplay.ModCompatibility
{
    public class LobbyCompatibilityPatch
    {
        public static void UseLobbyCompatibility(string modGUID, string modVersion)
        {
            PluginHelper.RegisterPlugin(modGUID, Version.Parse(modVersion), CompatibilityLevel.ClientOnly, VersionStrictness.None);
        }
    }
}
