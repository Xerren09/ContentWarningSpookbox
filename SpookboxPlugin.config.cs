using UnityEngine;
using Spookbox.Settings;
using ContentWarningShop;
using Steamworks;

/*
 TODOS

    DONE: Add infinite battery setting
    DONE: Price setting setup
    DONE: Keep the beats flowing even if the boombox is stashed
    DONE: Respect local volume override when detached
    TODO: Localise strings
    TODO: Scroll to swap tracks
    TODO: Right click hold on shoulder - plays louder?
 
 */

namespace Spookbox;

public partial class SpookboxPlugin
{
    private const string ALERT_META_GUID = "bigslapTunes";
    internal static readonly SynchronisedMetadata<bool> AlertMonsters = new($"{MOD_GUID}_{ALERT_META_GUID}", true);
    private static BoomboxAlertSetting _alertSetting;

    private const string INF_BATTERY_META_GUID = "infiniteBattery";
    internal static readonly SynchronisedMetadata<bool> InfiniteBattery = new($"{MOD_GUID}_{INF_BATTERY_META_GUID}", false);
    private static BoomboxInfiniteBatterySetting _infiniteBatterySetting;

    private static Callback<LobbyEnter_t> _callback = Callback<LobbyEnter_t>.Create(Steam_LobbyEnter);

    private static void _alertSetting_Changed(bool obj)
    {
        if (AlertMonsters.CanSet())
        {
            AlertMonsters.SetValue(obj);
        }
    }

    private static void _infiniteBatterySetting_Changed(bool obj)
    {
        if (InfiniteBattery.CanSet())
        {
            InfiniteBattery.SetValue(obj);
        }
    }

    // This is messy but the best solution I could come up with to solve this. When the vanilla game loads mods, the settings handler has
    // not yet initialised mod settings, so the custom ones aren't available. I wanted to make sure the host could change some of the settings
    // mid-game for the extra added fun factor, and doing all this ONCE in a lobby join callback was the best way.
    // Sure if the mod is compiled for BepInEx this could be done in the awake method, but I'm not about to do two completely different implementations.
    private static void Steam_LobbyEnter(LobbyEnter_t e)
    {
        // Use this as first time setup
        _alertSetting = GameHandler.Instance.SettingsHandler.GetSetting<BoomboxAlertSetting>();
        _alertSetting.Changed += _alertSetting_Changed;
        AlertMonsters.SetValue(_alertSetting.Value);
        _infiniteBatterySetting = GameHandler.Instance.SettingsHandler.GetSetting<BoomboxInfiniteBatterySetting>();
        _infiniteBatterySetting.Changed += _infiniteBatterySetting_Changed;
        InfiniteBattery.SetValue(_infiniteBatterySetting.Value);
        _callback.Dispose();
        Debug.Log("Spöökbox settings initialised.");
    }
}
