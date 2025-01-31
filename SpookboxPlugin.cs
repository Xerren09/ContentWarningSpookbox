using System.Reflection;
using UnityEngine;
using ContentWarningShop;
using ContentWarningShop.Localisation;
using Spookbox.Behaviour;
using System.Collections;
#if MODMAN
using BepInEx;
using HarmonyLib;
#endif

/*
    TASKS:

    DONE: Add infinite battery setting
    DONE: Price setting setup
    DONE: Keep the beats flowing even if the boombox is stashed
    DONE: Respect local volume override when detached

    TODO: Scroll to swap tracks
    TODO: Right click hold on shoulder - plays louder?
    TODO: Localise strings
 */

namespace Spookbox
{
    // This partial class file is used purely to separate the mess that is the multi target setup.
    // Define mod setup changes between the vanilla mod loader and bepinx
    [ContentWarningPlugin(MOD_GUID, MOD_VER, false)]
#if MODMAN
    [BepInPlugin(MOD_GUID, MOD_NAME, MOD_VER)]
    [BepInDependency(ShopApiPlugin.MOD_GUID)]
#endif
    public partial class SpookboxPlugin
#if MODMAN
    : BaseUnityPlugin
#endif
    {
        public static string PluginDirPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public const string MOD_GUID = "xerren.cwspookbox";
        public const string MOD_NAME = "Spöökbox";
        public const string MOD_VER = "1.0.0";

        internal static Item _spookboxItem;
        internal const string SPOOKBOX_GUID = "91f31218-6507-4bef-928d-e76b33f44a51";
        internal static AssetBundle _bundle;
#if MODMAN
        private static Harmony harmony = new Harmony(MOD_GUID);
#endif


#if STEAM
        static SpookboxPlugin() 
#elif MODMAN
        void Awake()
#endif
        {
            Initialise();
#if STEAM
            Debug.Log($"{MOD_GUID} initialising via the vanilla mod loader.");
            GameHandler.Instance.StartCoroutine(Deferred_LoadSettings());
#elif MODMAN
            Debug.Log($"{MOD_GUID} initialising via BepInEx mod loader.");
            // Mod will initialise on GameHandler.Initialize
            harmony.PatchAll();
#endif
            Debug.Log($"{MOD_GUID} initialised.");
        }

        private static void Initialise()
        {
            _bundle = LoadAssetBundle();
            _spookboxItem = _bundle.LoadAsset<Item>("Spookbox");
            _spookboxItem.itemObject.AddComponent<SpookboxBehaviour>();

            Shop.RegisterItem(_spookboxItem);
            Shop.RegisterCustomDataEntries();

            _spookboxItem.SetDefaultTooltips($"{ShopLocalisation.UseGlyph} Play;{ShopLocalisation.Use2Glyph} Next Track");

            Mixtape.LoadTracks();
        }

#if STEAM
        private static IEnumerator Deferred_LoadSettings()
        {
            yield return new WaitUntil(() => GameHandler.Instance.SettingsHandler != null);
            InitialiseSettings();
        }
#endif

        private static AssetBundle LoadAssetBundle()
        {
            var path = Path.Join(PluginDirPath, "spookbox");
            return AssetBundle.LoadFromFile(path);
        }
    }
}
