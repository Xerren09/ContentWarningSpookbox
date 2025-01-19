#if MODMAN
using HarmonyLib;

namespace Spookbox
{
    [HarmonyPatch(typeof(GameHandler))]
    internal class GameHandlerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameHandler.Initialize))]
        private static void Initialize()
        {
            SpookboxPlugin.InitialiseSettings();
        }
    }
}
#endif