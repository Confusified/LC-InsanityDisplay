using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsanityDisplay.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class PlayerControllerBPatch
    {
        public static PlayerControllerB PlayerControllerBInstance;

        [HarmonyPatch("Awake")]
        private static void Postfix(PlayerControllerB __instance)
        {
            if (__instance != GameNetworkManager.Instance.localPlayerController) { return; }
            PlayerControllerBInstance = __instance;
        }
    }
}