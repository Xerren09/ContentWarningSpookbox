using System.Reflection;
using UnityEngine;
using ContentWarningShop;
using ContentWarningShop.Localisation;
using Spookbox.Behaviour;
using Spookbox.Settings;
using Steamworks;
using Zorro.Settings;

/*
    TASKS:

    DONE: Add infinite battery setting
    DONE: Price setting setup
    DONE: Keep the beats flowing even if the boombox is stashed
    DONE: Respect local volume override when detached
    DONE: Scroll to swap tracks
    
    TODO: Right click hold on shoulder - plays louder?
    TODO: Localise strings
*/

namespace Spookbox
{
    // This partial class file is used purely to separate the mess that is the multi target setup.
    // Define mod setup changes between the vanilla mod loader and bepinx
    public partial class SpookboxPlugin
    {
        public const string MOD_GUID = "xerren.cwspookbox";
        public const string MOD_NAME = "Spöökbox";
        public const string MOD_VER = ThisAssembly.AssemblyVersion;
        private const string ASSETBUNDLE_NAME = "spookbox";
        internal static string PluginDirPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static bool _initialised = false;
        private static bool _initialisedSettings = false;

        /// <summary>
        /// Asset bundle containing all the mod's assets.
        /// </summary>
        internal static AssetBundle? _bundle;
        /// <summary>
        /// Base <see cref="ScriptableObject"/> <see cref="Item"/>. All spawned items are configured from this.
        /// </summary>
        internal static Item? _spookboxItem;
        /// <summary>
        /// The Item GUID used by the Spookbox item. Not directly used, but encoded in the dll for convenience.
        /// </summary>
        public const string SPOOKBOX_GUID = "91f31218-6507-4bef-928d-e76b33f44a51";

        private static Callback<LobbyCreated_t> cb_onLobbyCreated;

        private const string ALERT_META_GUID = "bigslapTunes";
        internal static readonly SynchronisedMetadata<bool> AlertMonsters = new($"{MOD_GUID}_{ALERT_META_GUID}", true);

        private const string INF_BATTERY_META_GUID = "infiniteBattery";
        internal static readonly SynchronisedMetadata<bool> InfiniteBattery = new($"{MOD_GUID}_{INF_BATTERY_META_GUID}", false);

        internal static void InitialisePlugin()
        {
            if (_initialised)
                return;

            LoadAssetBundle();
            _spookboxItem = _bundle.LoadAsset<Item>("Spookbox");
            _spookboxItem.itemObject.AddComponent<SpookboxBehaviour>();

            Shop.RegisterItem(_spookboxItem);
            Shop.RegisterCustomDataEntries();

            _spookboxItem.SetDefaultTooltips($"{ShopLocalisation.UseGlyph} Play;{ShopLocalisation.ZoomGlyph} Select Track");

            Mixtape.LoadTracks();

            _initialised = true;
            Debug.Log($"{MOD_GUID} setup complete.");
        }

        internal static void InitialiseSettings()
        {
            if (_initialisedSettings)
                return;

            cb_onLobbyCreated = Callback<LobbyCreated_t>.Create(Steam_LobbyCreated);

            InitialiseGameSetting<BoomboxAlertSetting>();
            InitialiseGameSetting<BoomboxInfiniteBatterySetting>();
            InitialiseGameSetting<BoomboxPriceSetting>();

            _initialisedSettings = true;

            Debug.Log($"{MOD_GUID} settings initialised.");
        }

        private static void Steam_LobbyCreated(LobbyCreated_t e)
        {
            if (e.m_eResult != EResult.k_EResultOK)
            {
                return;
            }
            ApplyGameSetting<BoomboxAlertSetting>();
            ApplyGameSetting<BoomboxInfiniteBatterySetting>();
            ApplyGameSetting<BoomboxPriceSetting>();
        }

        private static void InitialiseGameSetting<T>() where T : Setting, new()
        {
            if (GameHandler.Instance.SettingsHandler.GetSetting<T>() == null)
            {
                GameHandler.Instance.SettingsHandler.AddSetting(new T());
            }
        }

        private static void ApplyGameSetting<T>() where T : Setting, new()
        {
            var setting = GameHandler.Instance.SettingsHandler.GetSetting<T>();
            if (setting != null)
            {
                setting.ApplyValue();
            }
        }

        private static void LoadAssetBundle()
        {
            var path = Path.Join(PluginDirPath, ASSETBUNDLE_NAME);
            _bundle = AssetBundle.LoadFromFile(path);
        }
    }
}
