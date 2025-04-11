using UnityEngine;
using Spookbox.Settings;

namespace Spookbox.Behaviour
{
    public class StashedSpookboxBehaviour : MonoBehaviour
    {
        public Player Owner => _owner;
        private Player _owner;
        private OnOffEntry _onOffEntry;
        private BatteryEntry _batteryEntry;
        private float _volume;
        private AudioSource _speaker;
        private BoomboxLocalVolumeSetting _localVolumeSetting;

        private static readonly int DEFAULT_ALERT_DISTANCE = 35;
        private static readonly float ALERT_INTERVAL = 0.15f;
        private float _alertCountdown = 0f;

        void Awake()
        {
            _speaker = GetComponent<AudioSource>();
            _speaker.outputAudioMixerGroup = GameHandler.Instance.SettingsHandler.GetSetting<SFXVolumeSetting>().mixerGroup;
            //
            _localVolumeSetting = GameHandler.Instance.SettingsHandler.GetSetting<BoomboxLocalVolumeSetting>();
            _localVolumeSetting.Changed += _localVolumeSetting_Changed;
        }

        public void Configure(Player owner, OnOffEntry onOff, BatteryEntry battery, float volume)
        {
            _owner = owner;
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
            if (SpookboxPlugin.AlertMonsters.Value == true)
            {
                _alertCountdown -= Time.deltaTime;
                if (_alertCountdown < 0f)
                {
                    var scaledAlertDist = DEFAULT_ALERT_DISTANCE * _volume;
                    SFX_Player.instance.PlayNoise(base.transform.position, scaledAlertDist);
                    _alertCountdown = ALERT_INTERVAL;
                }
            }
            if (_owner.data.dead == true)
            {
                // If the player dies, remove the speaker so the dropped item can continue playing
                Destroy(gameObject);
            }
        }
    }
}
