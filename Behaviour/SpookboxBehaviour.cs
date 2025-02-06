using Spookbox.Entries;
using Spookbox.Settings;
using UnityEngine;

namespace Spookbox.Behaviour
{
    public class SpookboxBehaviour : ItemInstanceBehaviour
    {
        private GameObject _speakerObject;
        private AudioSource _speaker;
        private SFX_PlayOneShot _interactSFX;

        private BatteryEntry _battery;
        private const float MAX_BATTERY_CHARGE = 180f;
        private OnOffEntry _onOffEntry;
        private VolumeEntry _volume;
        private TrackEntry _track;
        private TimeEntry _playbackTime;

        private BoomboxLocalVolumeSetting _localVolumeSetting;
        private BoomboxVolumeUpBindSetting _volumeUpBindSetting;
        private BoomboxVolumeDownBindSetting _volumeDownBindSetting;

        private static readonly int _defaultAlertDistance = 35;
        private float _alertCountdown = 0f;
        private float _alertInterval = 0.15f;
        private int _instanceTrackIndex = -1;

        void Awake()
        {
            _speakerObject = transform.Find("SFX/SpookboxSpeaker").gameObject;
            _speaker = _speakerObject.GetComponent<AudioSource>();
            _interactSFX = transform.Find("SFX/Interact").GetComponent<SFX_PlayOneShot>();
            // Tie the volume to the SFX bus
            _speaker.outputAudioMixerGroup = GameHandler.Instance.SettingsHandler.GetSetting<SFXVolumeSetting>().mixerGroup;
            //
            _localVolumeSetting = GameHandler.Instance.SettingsHandler.GetSetting<BoomboxLocalVolumeSetting>();
            _localVolumeSetting.Changed += _localVolumeSetting_Changed;
            _volumeUpBindSetting = GameHandler.Instance.SettingsHandler.GetSetting<BoomboxVolumeUpBindSetting>();
            _volumeDownBindSetting = GameHandler.Instance.SettingsHandler.GetSetting<BoomboxVolumeDownBindSetting>();
        }

        void Update()
        {
            if (isHeldByMe && !Player.localPlayer.HasLockedInput())
            {
                if (Player.localPlayer.input.clickWasPressed)
                {
                    _onOffEntry.on = !_onOffEntry.on;
                    _onOffEntry.SetDirty();
                    // Determines whether the monsters can hear TUCA DONKA
                    if (_onOffEntry.on == true && SpookboxPlugin.AlertMonsters.Value == true)
                    {
                        _alertCountdown = _alertInterval;
                    }
                    ClickButtonSFX();
                }
                if (Player.localPlayer.input.aimWasPressed)
                {
                    SetTrack(((_track.TrackIndex + 1) % Mixtape.Tracks.Count));
                }
                if (GlobalInputHandler.GetKeyUp(_volumeUpBindSetting.Keycode()))
                {
                    _volume.Volume += 0.1f;
                    _volume.SetDirty();
                    AdjustVolume();
                    ClickButtonSFX();
                }
                if (GlobalInputHandler.GetKeyUp(_volumeDownBindSetting.Keycode()))
                {
                    _volume.Volume -= 0.1f;
                    _volume.SetDirty();
                    AdjustVolume();
                    ClickButtonSFX();
                }

            }

            if (_onOffEntry.on != _speaker.isPlaying)
            {
                if (_onOffEntry.on)
                {
                    _speaker.time = _playbackTime.currentTime;
                    _speaker.Play();
                }
                else
                {
                    _speaker.Stop();
                }
            }

            // Return if the music is off, the rest of the settings don't need updating
            if (_onOffEntry.on == false)
            {
                return;
            }

            if (_battery.m_charge < 0f)
            {
                _onOffEntry.on = false;
                _onOffEntry.SetDirty();
                ClickButtonSFX();
            }

            if (SpookboxPlugin.AlertMonsters.Value)
            {
                _alertCountdown -= Time.deltaTime;
                if (_alertCountdown < 0f)
                {
                    var scaledAlertDist = _defaultAlertDistance * _volume.Volume;
                    SFX_Player.instance.PlayNoise(base.transform.position, scaledAlertDist);
                    _alertCountdown = _alertInterval;
                }
            }

            if (_instanceTrackIndex != _track.TrackIndex)
            {
                _instanceTrackIndex = _track.TrackIndex;
                SetTrack(_instanceTrackIndex, true);
                if (_onOffEntry.on)
                {
                    _speaker.Play();
                }
                else
                {
                    _speaker.Stop();
                }
            }

            if (SpookboxPlugin.InfiniteBattery.Value == false)
            {
                _battery.m_charge -= Time.deltaTime;
            }
            _playbackTime.currentTime = _speaker.time;
        }

        void OnDestroy()
        {
            _localVolumeSetting.Changed -= _localVolumeSetting_Changed;
            itemInstance.onItemEquipped.RemoveListener(OnEquip);
            itemInstance.onUnequip.RemoveListener(OnUnequip);
        }

        private void OnEquip(Player player)
        {
            if (TryReattachSpeaker(player))
            {
                Debug.Log($"Spöökbox speaker reattached: {itemInstance.instanceData.m_guid}");
            }
        }

        private void OnUnequip(Player player)
        {
            var stillOwnedByPlayer = IsItemInPlayerInventory(player, itemInstance.instanceData.m_guid);
            if (stillOwnedByPlayer && _onOffEntry.on == true)
            {
                DetachSpeaker(player);
                Debug.Log($"Spöökbox speaker detached: {itemInstance.instanceData.m_guid}");
            }
        }

        /// <summary>
        /// Checks if the item with the given Guid is in the player's inventory.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="itemGuid"></param>
        /// <returns></returns>
        private static bool IsItemInPlayerInventory(Player player, Guid itemGuid)
        {
            if (player.TryGetInventory(out PlayerInventory inventory))
            {
                return inventory.GetItems().FindIndex(item => item.data.m_guid == itemGuid) != -1;
            }
            return false;
        }

        /// <summary>
        /// Tries to get the instance's detached speaker on a player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private GameObject GetDetachedSpeaker(Player player)
        {
            var speakerName = $"__spookbox_speaker_{itemInstance.m_guid.Value}";
            var target = player.refs.cameraPos;
            var tr = target.Find(speakerName);
            if (tr == null)
            {
                return null;
            }
            return tr.gameObject;
        }

        /// <summary>
        /// Creates a detached speaker instance on the player, and configures it to play from where the current instance's speaker stops.
        /// </summary>
        /// <param name="player"></param>
        private void DetachSpeaker(Player player)
        {
            var target = player.refs.cameraPos;
            var speakerPrefab = SpookboxPlugin._bundle.LoadAsset<GameObject>("SpookboxSpeaker");
            var speaker = Instantiate(speakerPrefab);
            var stashedController = speaker.AddComponent<StashedSpookboxBehaviour>();
            stashedController.Configure(_onOffEntry, _battery, _volume.Volume);
            speaker.name = $"__spookbox_speaker_{itemInstance.m_guid.Value}";
            speaker.transform.SetParent(target, false);
            var speakerAudio = speaker.GetComponent<AudioSource>();
            speakerAudio.enabled = true;
            speakerAudio.outputAudioMixerGroup = _speaker.outputAudioMixerGroup;
            speakerAudio.clip = _speaker.clip;
            speakerAudio.volume = _volume.Volume * _localVolumeSetting.Value;
            speakerAudio.time = _playbackTime.currentTime;
            //_playbackTime.SetDirty();
            speakerAudio.Play();
        }

        /// <summary>
        /// Finds the current instance's detached speaker if any, moves its data back to the instance's speaker, then destroys it.
        /// </summary>
        /// <param name="player"></param>
        private bool TryReattachSpeaker(Player player)
        {
            var dSpeaker = GetDetachedSpeaker(player);
            if (dSpeaker == null)
            {
                return false;
            }
            var speakerAudio = dSpeaker.GetComponent<AudioSource>();
            _speaker.clip = speakerAudio.clip;
            _speaker.time = speakerAudio.time;
            _playbackTime.currentTime = speakerAudio.time;
            //_playbackTime.SetDirty();
            Destroy(dSpeaker);
            return true;
        }

        private void _localVolumeSetting_Changed(float obj)
        {
            if (_speaker == null)
            {
                return;
            }
            AdjustVolume();
        }

        public override void ConfigItem(ItemInstanceData data, Photon.Pun.PhotonView playerView)
        {
            #region DefaultItemConfig
            if (!data.TryGetEntry(out _battery))
            {
                _battery = new BatteryEntry()
                {
                    m_maxCharge = MAX_BATTERY_CHARGE,
                    m_charge = MAX_BATTERY_CHARGE
                };
                data.AddDataEntry(_battery);
            }
            if (!data.TryGetEntry(out _onOffEntry))
            {
                _onOffEntry = new OnOffEntry()
                {
                    on = false
                };
                data.AddDataEntry(_onOffEntry);
            }
            #endregion
            #region BoomboxConfig
            if (!data.TryGetEntry(out _volume))
            {
                _volume = new VolumeEntry()
                {
                    Volume = 0.5f,
                };
                data.AddDataEntry(_volume);
            }
            AdjustVolume();
            if (!data.TryGetEntry(out _track))
            {
                _track = new TrackEntry();
                data.AddDataEntry(_track);
            }
            if (!data.TryGetEntry(out _playbackTime))
            {
                _playbackTime = new TimeEntry()
                {
                    currentTime = 0f,
                };
                data.AddDataEntry(_playbackTime);
            }
            #endregion
            itemInstance.onItemEquipped.AddListener(OnEquip);
            itemInstance.onUnequip.AddListener(OnUnequip);
        }

        /// <summary>
        /// Sets the track to the clip at the specific index.
        /// </summary>
        /// <param name="idx"></param>
        private void SetTrack(int idx, bool local = false)
        {
            if (Mixtape.HasTracks())
            {
                if (local == false)
                {
                    _playbackTime.currentTime = 0;
                    _playbackTime.SetDirty();
                    _track.TrackIndex = idx;
                    _track.SetDirty();
                    ClickButtonSFX();
                }
                //
                _speaker.clip = Mixtape.Tracks[_track.TrackIndex];
                _speaker.time = _playbackTime.currentTime;
            }
        }

        private void ClickButtonSFX()
        {
            if (_interactSFX != null)
            {
                _interactSFX.Play();
            }
        }

        private void AdjustVolume()
        {
            var vol = _volume.Volume * _localVolumeSetting.Value;
            _speaker.volume = vol;
        }
    }
}
