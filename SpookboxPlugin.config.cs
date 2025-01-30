using UnityEngine;
using Spookbox.Settings;
using ContentWarningShop;
using Zorro.Settings;
using Steamworks;

namespace Spookbox;

public partial class SpookboxPlugin
{
    private static bool _initialised = false;
    private static Callback<LobbyCreated_t> cb_onLobbyCreated = Callback<LobbyCreated_t>.Create(Steam_LobbyCreated);

    private const string ALERT_META_GUID = "bigslapTunes";
    internal static readonly SynchronisedMetadata<bool> AlertMonsters = new($"{MOD_GUID}_{ALERT_META_GUID}", true);

    private const string INF_BATTERY_META_GUID = "infiniteBattery";
    internal static readonly SynchronisedMetadata<bool> InfiniteBattery = new($"{MOD_GUID}_{INF_BATTERY_META_GUID}", false);

    internal static void InitialiseSettings()
    {
        if (_initialised) return;

        InitialiseGameSetting<BoomboxAlertSetting>();
        InitialiseGameSetting<BoomboxInfiniteBatterySetting>();
        InitialiseGameSetting<BoomboxPriceSetting>();

        _initialised = true;

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
}
