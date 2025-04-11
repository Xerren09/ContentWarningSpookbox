using UnityEngine;
#if MODMAN
using ContentWarningShop;
using BepInEx;
#endif

namespace Spookbox
{
    [ContentWarningPlugin(SpookboxPlugin.MOD_GUID, SpookboxPlugin.MOD_VER, false)]
#if MODMAN
    [BepInPlugin(SpookboxPlugin.MOD_GUID, SpookboxPlugin.MOD_NAME, SpookboxPlugin.MOD_VER)]
    [BepInDependency(ShopApiPlugin.MOD_GUID, ShopApiPlugin.MOD_VER)]
#endif
    public partial class PluginLoader
#if MODMAN
    : BaseUnityPlugin
#endif
    {
        
    }
}
