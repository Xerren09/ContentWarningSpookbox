using UnityEngine;
using Spookbox.Settings;

namespace Spookbox.Behaviour
{
    public class StashedSpookboxBehaviour : MonoBehaviour
    {
        private OnOffEntry _onOffEntry;
        private BatteryEntry _batteryEntry;
        private float _volume;
        private AudioSource _speaker;
        private BoomboxLocalVolumeSetting _localVolumeSetting;

        void Awake()
        {
            _speaker = GetComponent<AudioSource>();
            _speaker.outputAudioMixerGroup = GameHandler.Instance.SettingsHandler.GetSetting<SFXVolumeSetting>().mixerGroup;
            //
            _localVolumeSetting = GameHandler.Instance.SettingsHandler.GetSetting<BoomboxLocalVolumeSetting>();
            _localVolumeSetting.Changed += _localVolumeSetting_Changed;
        }

        public void Configure(OnOffEntry onOff, BatteryEntry battery, float volume)
        {
            _onOffEntry = onOff;
            _batteryEntry = battery;
            _volume = volume;
        }

        private void _localVolumeSetting_Changed(float obj)
        {
            _speaker.volume = _volume * obj;
        }

        void OnDestroy()
        {
            _localVolumeSetting.Changed -= _localVolumeSetting_Changed;
        }

        void Update()
        {
            if (SpookboxPlugin.InfiniteBattery.Value == false)
            {
                if (_batteryEntry.m_charge < 0f)
                {
                    _onOffEntry.on = false;
                    _onOffEntry.SetDirty();
                    Debug.Log($"Stashed {gameObject.name} destroyed; ran out of battery.");
                    Destroy(gameObject);
                }
                _batteryEntry.m_charge -= Time.deltaTime;
            }
        }
    }
}
