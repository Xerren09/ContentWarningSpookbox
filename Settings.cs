using UnityEngine;
using Unity.Mathematics;
using Zorro.Settings;
using Spookbox.Entries;
using ContentWarningShop;

namespace Spookbox.Settings
{
    /// <summary>
    /// Controls all boomboxes' local volume. This is on top of the separate volume set on the specific boombox instance.
    /// </summary>
    [ContentWarningSetting]
    public class BoomboxLocalVolumeSetting : FloatSetting, IExposedSetting
    {
        public event Action<float> Changed;
        public override void ApplyValue()
        {
            Changed?.Invoke(Value);
        }
        public override float GetDefaultValue() => 1;
        public override float2 GetMinMaxValue() => new(0, 1);
        public SettingCategory GetSettingCategory() => SettingCategory.Mods;
        public string GetDisplayName() => $"[{SpookboxPlugin.MOD_NAME}] Local volume";
        private void AdjustDetachedSpeakers()
        {
            // Update volume for any detached speakers
            foreach (var player in PlayerHandler.instance.players)
            {
                foreach (Transform transform in player.refs.cameraPos)
                {
                    if (transform.gameObject.name.Contains("__spookbox_speaker_"))
                    {
                        var guid = transform.gameObject.name.Replace("__spookbox_speaker_", "");
                        player.TryGetInventory(out PlayerInventory inv);
                        var item = inv.GetItems().Find(i => i.data.m_guid.ToString() == guid);
                        item.data.TryGetEntry(out VolumeEntry vol);
                        transform.gameObject.GetComponent<AudioSource>().volume = vol.Volume * Value;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Sets the boombox's volume up keybind.
    /// </summary>
    /// <remarks>
    /// This only controls the a boombox instance's own volume, not <see cref="BoomboxLocalVolumeSetting"/>.
    /// </remarks>
    [ContentWarningSetting]
    public class BoomboxVolumeUpBindSetting : KeyCodeSetting, IExposedSetting
    {
        public SettingCategory GetSettingCategory() => SettingCategory.Mods;
        public string GetDisplayName() => $"[{SpookboxPlugin.MOD_NAME}] Volume Up Key";

        public override KeyCode GetDefaultKey()
        {
            return KeyCode.PageUp;
        }
    }

    /// <summary>
    /// Sets the boombox's volume down keybind.
    /// </summary>
    /// <remarks>
    /// This only controls the a boombox instance's own volume, not <see cref="BoomboxLocalVolumeSetting"/>.
    /// </remarks>
    [ContentWarningSetting]
    public class BoomboxVolumeDownBindSetting : KeyCodeSetting, IExposedSetting
    {
        public SettingCategory GetSettingCategory() => SettingCategory.Mods;
        public string GetDisplayName() => $"[{SpookboxPlugin.MOD_NAME}] Volume Down Key";

        public override KeyCode GetDefaultKey()
        {
            return KeyCode.PageDown;
        }
    }

    /// <summary>
    /// Sets if the boombox's music should alert other monsters.
    /// </summary>
    /// <remarks>
    /// If the current player is the host, this synchronises to every player in the lobby, without actually modifying their own settings.
    /// </remarks>
    [ContentWarningSetting]
    public class BoomboxAlertSetting : BoolSetting, IExposedSetting
    {
        public override void ApplyValue()
        {
            SpookboxPlugin.AlertMonsters.SetValue(Value);
        }
        public SettingCategory GetSettingCategory() => SettingCategory.Mods;
        public string GetDisplayName() => $"[{SpookboxPlugin.MOD_NAME}] Let BigSlap hear the tunes (Host)";

        public override bool GetDefaultValue()
        {
            return true;
        }
    }

    /// <summary>
    /// Sets if the boombox's battery should discharge.
    /// </summary>
    /// <remarks>
    /// If the current player is the host, this synchronises to every player in the lobby, without actually modifying their own settings.
    /// </remarks>
    [ContentWarningSetting]
    public class BoomboxInfiniteBatterySetting : BoolSetting, IExposedSetting
    {
        public override void ApplyValue()
        {
            SpookboxPlugin.InfiniteBattery.SetValue(Value);
        }
        public SettingCategory GetSettingCategory() => SettingCategory.Mods;
        public string GetDisplayName() => $"[{SpookboxPlugin.MOD_NAME}] Infinite battery (Host)";

        public override bool GetDefaultValue()
        {
            return false;
        }
    }

    /// <summary>
    /// Sets the purchase price of the boombox. This is a host only setting and will be set on lobby creation; changing this is not possible later.
    /// </summary>
    [ContentWarningSetting]
    public class BoomboxPriceSetting : IntSetting, IExposedSetting
    {
        public override void ApplyValue()
        {
            if (SpookboxPlugin._spookboxItem == null) return;
            if (Shop.UpdateItemPrice(SpookboxPlugin._spookboxItem, Value) == false)
            {
                Debug.LogWarning($"Attempted to apply {nameof(BoomboxPriceSetting)} value to item while not the host of the current lobby.");
            }
        }
        public SettingCategory GetSettingCategory() => SettingCategory.Mods;
        public string GetDisplayName() => $"[{SpookboxPlugin.MOD_NAME}] Price (Host)";

        public override int GetDefaultValue()
        {
            return 100;
        }
    }
}
