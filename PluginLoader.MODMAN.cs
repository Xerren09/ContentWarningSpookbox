#if MODMAN
using HarmonyLib;
using UnityEngine;

namespace Spookbox
{
    public partial class PluginLoader
    {
        private static Harmony harmony = new Harmony(SpookboxPlugin.MOD_GUID);

        /*
            NOTE: The loader for Bepinex is much simpler because the mod loader itself will skip this plugin if a dependency is missing or incompatible
            It also doesn't allow runtime installs like Steam does, so we barely have to check anything. 
         */
        void Awake()
        {
            Debug.Log($"{SpookboxPlugin.MOD_GUID} loading via BepInEx mod loader.");
            // Mod settings will initialise on GameHandler.Initialize postfix patch
            SpookboxPlugin.InitialisePlugin();
            harmony.PatchAll();
            Debug.Log($"{SpookboxPlugin.MOD_GUID} loaded.");
        }
    }

    [HarmonyPatch(typeof(GameHandler))]
    internal class GameHandlerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameHandler.Initialize))]
        private static void Initialize()
        {
            SpookboxPlugin.RegisterSettings();
        }
    }
}
#endif