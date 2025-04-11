#if STEAM
using ContentWarningShop;
using System.Collections;
using UnityEngine;

namespace Spookbox
{
    public partial class PluginLoader
    {
        /*
         NOTE: 
            Initialising this mod is MUCH more difficult on Steam because of the external dependency, ShopAPI. Realistically
            I could just let it rip, but I want to make sure if things go very wrong the player isn't left confused.

            When a Workshop Item is subscribed to WHILE the game is running, the mod loader works quite differently:
            1. Settings must be manually registered.
            2. Dependency order is not enforced.
                This one SUCKS. Normally Steam correctly resolves that ShopAPI must be loaded before Spookbox, but when installing
                the mod WHILE the game is running, it's either a cointoss or reverse order (couldn't figure it out) which one gets downloaded
                first and loaded. 
                In some cases Spookbox is loaded first, which throws a TypeLoadException since things like SynchronisedMetadata
                are pulled from ShopAPI. 
         */

        private static Version _minDepVersion = new Version(ShopApiPlugin.MOD_VER);
        static PluginLoader() 
        {
            Debug.Log($"{SpookboxPlugin.MOD_GUID} loading via vanilla mod loader.");
            Coroutine routine = null;
            try
            {
                // Defers the type resolver to the lambda, so *that* faults if ShopAPI is missing, not the current method, allowing it to be caught.
                Action proxyCall = () => { SpookboxPlugin.InitialisePlugin(); };
                proxyCall();
                routine = GameHandler.Instance.StartCoroutine(Deferred_LoadSettings());
                Debug.Log($"{SpookboxPlugin.MOD_GUID} loaded.");
            }
            catch (Exception ex)
            {
                if (routine != null)
                {
                    GameHandler.Instance.StopCoroutine(routine);
                }
                if (IsDependencyLoaded() == false)
                {
                    Debug.LogError($"{SpookboxPlugin.MOD_GUID} failed to load: {ShopApiPlugin.MOD_GUID} not loaded.");
                    ShowMissingDependencyRestartPrompt();
                }
                else if (IsDependencyVersionCorrect() == false)
                {
                    Debug.LogError($"{SpookboxPlugin.MOD_GUID} failed to load: {ShopApiPlugin.MOD_GUID} version incorrect.");
                    ShowMissingDependencyRestartPrompt();
                }
                else
                {
                    Debug.LogError($"{SpookboxPlugin.MOD_GUID} failed to load ({ex.GetType().Name}).");
                }
                Debug.LogError($"{SpookboxPlugin.MOD_GUID} IsRuntimeInstallFault: {GameHandler.Instance.SettingsHandler != null}");
                Debug.LogException(ex);
            }
        }

        private static IEnumerator Deferred_LoadSettings()
        {
            yield return new WaitUntil(() => GameHandler.Instance.SettingsHandler != null);
            SpookboxPlugin.RegisterSettings();
        }

        private static bool IsDependencyVersionCorrect()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var target = Array.Find(assemblies, a => a.GetName().Name == "ShopAPI");
            if (target == null)
            {
                return false;
            }
            if (target.GetVersion().CompareTo(_minDepVersion) < 0)
            {
                return false;
            }
            return true;
        }

        private static bool IsDependencyLoaded()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var target = Array.Find(assemblies, a => a.GetName().Name == "ShopAPI");
            return target != null;
        }

        private static void ShowMissingDependencyRestartPrompt()
        {
            var options = new ModalOption[2] { new ModalOption("Close Game", () => { Application.Quit(); }), new ModalOption("Ignore") };
            Modal.Show($"{SpookboxPlugin.MOD_NAME} Dependency Error", "Spöökbox depends on the ShopAPI mod, which is missing. This can often be caused by installing the mod while the game is running. Please ensure you've subscribed to the ShopAPI mod, and restart your game. If you run into further issues please read the Workshop page, and don't hesitate to reach out.", options);
        }

        private static void ShowGenericRestartPrompt()
        {
            var options = new ModalOption[1] { new ModalOption("OK") };
            Modal.Show($"{SpookboxPlugin.MOD_NAME} Load Error", "Spöökbox encountered an error while loading. This can often be caused by installing the mod while the game is running. Please ensure you've subscribed to the ShopAPI mod, and restart your game. If you run into further issues please read the Workshop page, and don't hesitate to reach out.", options);
        }
    }
}
#endif