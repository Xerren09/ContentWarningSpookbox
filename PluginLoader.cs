using System.Collections;
using UnityEngine;
#if MODMAN
using ContentWarningShop;
using BepInEx;
using HarmonyLib;
#endif

namespace Spookbox
{
    [ContentWarningPlugin(SpookboxPlugin.MOD_GUID, SpookboxPlugin.MOD_VER, false)]
#if MODMAN
    [BepInPlugin(SpookboxPlugin.MOD_GUID, SpookboxPlugin.MOD_NAME, SpookboxPlugin.MOD_VER)]
    [BepInDependency(ShopApiPlugin.MOD_GUID, "1.1")]
#endif
    public class PluginLoader
#if MODMAN
    : BaseUnityPlugin
#endif
    {
#if STEAM
        static PluginLoader()
#elif MODMAN
        private static Harmony harmony = new Harmony(SpookboxPlugin.MOD_GUID);

        void Awake()
#endif
        {
#if STEAM
            Debug.Log($"{SpookboxPlugin.MOD_GUID} loading via vanilla mod loader.");
#elif MODMAN
            Debug.Log($"{SpookboxPlugin.MOD_GUID} loading via BepInEx mod loader.");
            // Mod settings will initialise on GameHandler.Initialize postfix patch
            harmony.PatchAll();
#endif
            try
            {
                // Deferes the type resolver to the lambda, so *that* faults if ShopAPI is missing, not the current method, allowing it to be caught.
                Action proxyCall = () => { SpookboxPlugin.InitialisePlugin(); };
                proxyCall();
#if STEAM
                GameHandler.Instance.StartCoroutine(Deferred_LoadSettings());
#endif
            }
            catch (TypeLoadException ex)
            {
                var isMissing = IsShopApiAssemblyLoaded();
                Debug.LogError($"{SpookboxPlugin.MOD_GUID} failed to load due to missing Types: missing ShopAPI? {!isMissing}");
                Debug.LogException(ex);
                if (isMissing)
                {
                    ShowMissingDependencyRestartPrompt();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"{SpookboxPlugin.MOD_GUID} failed: missing ShopAPI? {!IsShopApiAssemblyLoaded()}");
                Debug.LogException(ex);
                ShowGenericRestartPrompt();
            }
            Debug.Log($"{SpookboxPlugin.MOD_GUID} loaded.");
        }

#if STEAM
        private static IEnumerator Deferred_LoadSettings()
        {
            yield return new WaitUntil(() => GameHandler.Instance.SettingsHandler != null);
            SpookboxPlugin.InitialiseSettings();
        }
#endif

        private static bool IsShopApiAssemblyLoaded()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var target = Array.Find(assemblies, a => a.GetName().Name == "ShopAPI");
            return target != null;
        }

        private static void ShowMissingDependencyRestartPrompt()
        {
            var options = new ModalOption[1] { new ModalOption("Close Game") };
            Modal.Show($"{SpookboxPlugin.MOD_NAME} Dependency Error", "Spöökbox depends on the ShopAPI mod, which is missing. This can often be caused by installing the mod while the game is running. Please ensure you've subscribed to the ShopAPI mod, and restart your game. If you run into further issues please read the Workshop page, and don't hesitate to reach out.", options, () => { Application.Quit(); });
        }

        private static void ShowGenericRestartPrompt()
        {
            var options = new ModalOption[1] { new ModalOption("OK") };
            Modal.Show($"{SpookboxPlugin.MOD_NAME} Load Error", "Spöökbox encountered an error while loading. This can often be caused by installing the mod while the game is running. Please ensure you've subscribed to the ShopAPI mod, and restart your game. If you run into further issues please read the Workshop page, and don't hesitate to reach out.", options);
        }
    }
}
