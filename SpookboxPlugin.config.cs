using UnityEngine;
using Spookbox.Settings;
using ContentWarningShop;

namespace Spookbox;

public partial class SpookboxPlugin
{
    private static bool _initialised = false;
    private const string ALERT_META_GUID = "bigslapTunes";
    internal static readonly SynchronisedMetadata<bool> AlertMonsters = new($"{MOD_GUID}_{ALERT_META_GUID}", true);
    private static BoomboxAlertSetting _alertSetting;

    private const string INF_BATTERY_META_GUID = "infiniteBattery";
    internal static readonly SynchronisedMetadata<bool> InfiniteBattery = new($"{MOD_GUID}_{INF_BATTERY_META_GUID}", false);
    private static BoomboxInfiniteBatterySetting _infiniteBatterySetting;

    private static BoomboxPriceSetting _priceSetting;

    internal static void InitialiseSettings()
    {
        if (_initialised) return;
        _alertSetting = GameHandler.Instance.SettingsHandler.GetSetting<BoomboxAlertSetting>();
        _alertSetting.Changed += _alertSetting_Changed;
        AlertMonsters.SetValue(_alertSetting.Value);

        _infiniteBatterySetting = GameHandler.Instance.SettingsHandler.GetSetting<BoomboxInfiniteBatterySetting>();
        _infiniteBatterySetting.Changed += _infiniteBatterySetting_Changed;
        InfiniteBattery.SetValue(_infiniteBatterySetting.Value);

        _priceSetting = GameHandler.Instance.SettingsHandler.GetSetting<BoomboxPriceSetting>();
        _priceSetting.Changed += _priceSetting_Changed;
        _spookboxItem.price = _priceSetting.Value;

        _initialised = true;

        Debug.Log($"{MOD_GUID} settings initialised.");
    }

    private static void _priceSetting_Changed(int obj)
    {
        if (SynchronisedMetadata<bool>.InLobby == false)
        {
            _spookboxItem.price = obj;
        }
        else
        {
            Debug.LogWarning($"Attempted to apply {nameof(BoomboxPriceSetting)} value to item while in a lobby. Item price can only be changed outside of the lobby; please restart the lobby to change this setting.");
        }
    }

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
}
