using UnityEngine;
using System.Reflection;
using ContentWarningShop;
using ContentWarningShop.Localisation;
using Spookbox.Behaviour;
#if MODMAN
using BepInEx;
#endif

namespace Spookbox
{
    // This partial class file is used purely to separate the mess that is the multi target setup.
    // Define changes mod setup changes between the vanilla mod loader and bepinx
    [ContentWarningPlugin(MOD_GUID, "1.0.0", false)]
#if MODMAN
    [BepInPlugin(MOD_GUID, MOD_NAME, "1.0.0")]
    [BepInDependency(ShopApiPlugin.MOD_GUID)]
#endif
    public partial class SpookboxPlugin
#if MODMAN
    : BaseUnityPlugin
#endif
    {
        public static string PluginDirPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public const string MOD_GUID = "xerren.cwspookbox";
        public const string MOD_NAME = "Spookbox";

        internal static Item _spookboxItem;
        internal const string SPOOKBOX_GUID = "91f31218-6507-4bef-928d-e76b33f44a51";
        internal static AssetBundle _bundle;

#if STEAM
        static SpookboxPlugin()
#elif MODMAN
        void Awake()
#endif
        {
#if STEAM
            Debug.Log($"{MOD_GUID} initialising via the vanilla mod loader.");
#elif MODMAN
            Debug.Log($"{MOD_GUID} initialising via BepInEx mod loader.");
#endif
            //
            _bundle = LoadAssetBundle();
            _spookboxItem = _bundle.LoadAsset<Item>("Spookbox");
            _spookboxItem.itemObject.AddComponent<SpookboxBehaviour>();

            Shop.RegisterItem(_spookboxItem);
            Shop.RegisterCustomDataEntries();

            _spookboxItem.SetDefaultTooltips("{key_use} Play;{key_use2} Next Track");

            Mixtape.LoadTracks();
            Debug.Log($"{MOD_GUID} initialised.");
        }

        private static AssetBundle LoadAssetBundle()
        {
            var path = Path.Join(PluginDirPath, "spookbox");
            return AssetBundle.LoadFromFile(path);
        }
    }
}
