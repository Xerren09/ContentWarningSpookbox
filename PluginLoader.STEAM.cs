#if STEAM
using ContentWarningShop;
using Steamworks;
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

            During standard install, Spookbox and ShopAPI are downloaded and their load orders are set by Steam automatically.
                After game start settings are pulled and registered automatically by the game.

            When a Workshop Item is subscribed to *while* the game is running, the game's mod loader works quite differently:
            1. Settings must be manually registered.
            2. Mod dependency order is not enforced: items are loaded in the other they have been subscribed to. This means ShopAPI
               loads second, and Spookbox faults because things like SynchronisedMetadata are missing.
         */

        private static System.Version _minDepVersion = new System.Version(ShopApiPlugin.MOD_VER);
        static PluginLoader() 
        {
            Logger.Log($"Loading via vanilla mod loader.");
            Coroutine routine = null;
            try
            {
                // Defers the type resolver to the lambda, so *that* faults if ShopAPI is missing, not the current method, allowing it to be caught.
                Action proxyCall = () => { SpookboxPlugin.InitialisePlugin(); };
                proxyCall();
                routine = GameHandler.Instance.StartCoroutine(Deferred_LoadSettings());
                Logger.Log($"Finished loading.");
            }
            catch (Exception ex)
            {
                // If installed after the game is started, SettingsHandler is already initialised
                var isRuntimeInstallFault = GameHandler.Instance.SettingsHandler != null;
                Logger.LogError($"IsRuntimeInstallFault: {isRuntimeInstallFault}");
                if (routine != null)
                {
                    GameHandler.Instance.StopCoroutine(routine);
                }

                if (isRuntimeInstallFault && IsDependencyLoaded() == false)
                {
                    Logger.LogError($"Failed to load: {ShopApiPlugin.MOD_GUID} not loaded during late install.");
                    ShowDependencyLateInstallRestartPrompt();
                }
                else if (IsDependencyLoaded() == false)
                {
                    if (IsDependencyDownloaded() == false)
                    {
                        Logger.LogError($"Failed to load: {ShopApiPlugin.MOD_GUID} missing.");
                        ShowMissingDependencyErrorPrompt();
                    }
                    if (IsDependencyEnabled() == false)
                    {
                        Logger.LogError($"Failed to load: {ShopApiPlugin.MOD_GUID} present but not enabled.");
                        ShowDisabledDependencyErrorPrompt();
                    }
                }
                else if (IsDependencyVersionCorrect() == false)
                {
                    Logger.LogError($"Failed to load: {ShopApiPlugin.MOD_GUID} version incorrect.");
                    ShowDependencyVersionErrorPrompt();
                }
                else
                {
                    Logger.LogError($"Failed to load ({ex.GetType().Name}).");
                    ShowGenericErrorPrompt();
                }
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

        private static bool IsDependencyDownloaded()
        {
#if RELEASE
            EItemState dependencyState = (EItemState)SteamUGC.GetItemState(new PublishedFileId_t(ShopApiPlugin.STEAM_WORKSHOP_ITEM_ID));
            return dependencyState.HasFlag(EItemState.k_EItemStateInstalled) || dependencyState.HasFlag(EItemState.k_EItemStateSubscribed);
#elif DEBUG
            return true;
#endif
        }

        private static bool IsDependencyEnabled()
        {
#if RELEASE
            EItemState dependencyState = (EItemState)SteamUGC.GetItemState(new PublishedFileId_t(ShopApiPlugin.STEAM_WORKSHOP_ITEM_ID));
            return !dependencyState.HasFlag(EItemState.k_EItemStateDisabledLocally);
#elif DEBUG
            return true;
#endif
        }

        /// <summary>
        /// Error message shown if the mod is installed from the Steam Workshop while the game is running
        /// </summary>
        private static void ShowDependencyLateInstallRestartPrompt()
        {
            var options = new ModalOption[2] { new ModalOption("Close Game", () => { Application.Quit(); }), new ModalOption("Ignore") };
            Modal.Show($"{SpookboxPlugin.MOD_NAME} Late Install", "Spöökbox depends on the ShopAPI mod, which needs to be loaded first. This can often be caused by installing the mod while the game is running. Please ensure you've subscribed to the ShopAPI mod and restart your game, otherwise Spöökbox will not work for the current session.", options);
        }

        /// <summary>
        /// Error message shown if the mod is missing the ShopAPI dependency during normal startup
        /// </summary>
        private static void ShowMissingDependencyErrorPrompt()
        {
            var options = new ModalOption[2] { new ModalOption("Close Game", () => { Application.Quit(); }), new ModalOption("Ignore")};
            Modal.Show($"{SpookboxPlugin.MOD_NAME} Dependency Not Installed", "Spöökbox depends on the ShopAPI mod, which is not installed. The game will NOT work with Spöökbox enabled while ShopAPI is missing. Please ensure you've subscribed to the ShopAPI mod and restart your game, or disable Spöökbox.", options);
        }

        /// <summary>
        /// Error message shown if ShopAPI is disabled in steam
        /// </summary>
        private static void ShowDisabledDependencyErrorPrompt()
        {
            var options = new ModalOption[2] { new ModalOption("Close Game", () => { Application.Quit(); }), new ModalOption("Ignore") };
            Modal.Show($"{SpookboxPlugin.MOD_NAME} Dependency Disabled", "Spöökbox depends on the ShopAPI mod, which is not enabled. The game will NOT work with Spöökbox enabled while ShopAPI is disabled. To enable ShopAPI, go to your steam library, right click Content Warning, go to Properties -> Workshop, and enable the ShopAPI item, then restart the game.", options);
        }

        /// <summary>
        /// Error message shown if the ShopAPI dependency is lower than expected
        /// </summary>
        private static void ShowDependencyVersionErrorPrompt()
        {
            var options = new ModalOption[2] { new ModalOption("Close Game", () => { Application.Quit(); }), new ModalOption("Ignore") };
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var target = Array.Find(assemblies, a => a.GetName().Name == "ShopAPI");
            var presentVer = new System.Version();
            if (target != null)
            {
                presentVer = target.GetVersion();
            }
            Modal.Show($"{SpookboxPlugin.MOD_NAME} Dependency Version Mismatch", $"Spöökbox depends on the ShopAPI mod, but can only find an old version installed, which may result in gamebreaking errors. ShopAPI version v{_minDepVersion} expected; found v{presentVer}. Please update ShopAPI to at least v{_minDepVersion}.", options);
        }

        /// <summary>
        /// Error message shown when a generic exception occurs
        /// </summary>
        private static void ShowGenericErrorPrompt()
        {
            var options = new ModalOption[1] { new ModalOption("OK") };
            Modal.Show($"{SpookboxPlugin.MOD_NAME} Load Error", "Spöökbox encountered an error while loading. This can often be caused by installing the mod while the game is running. Please ensure you've subscribed to the ShopAPI mod, and restart your game. If you run into further issues please read the Workshop page, and don't hesitate to reach out.", options);
        }
    }
}
#endif