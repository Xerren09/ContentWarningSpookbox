using Spookbox.Entries;
using Spookbox.Settings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Spookbox.Behaviour
{
    public class SpookboxBehaviour : ItemInstanceBehaviour
    {
        private static readonly string INPUTACTIONREF_SOURCE_ITEM_PERSISTENT_GUID = "76f4d02a-65ae-4d8b-89da-1e3e1e82f82d";
        private static readonly float INPUT_DEBOUNCE_TIME = 0.15f;
        private static InputActionReference ZoomIn;
        private static InputActionReference ZoomOut;

        static SpookboxBehaviour()
        {
            ItemDatabase.TryGetItemFromPersistentID(new Guid(INPUTACTIONREF_SOURCE_ITEM_PERSISTENT_GUID), out Item camItem);
            var cam = camItem.itemObject.GetComponent<VideoCamera>();
            ZoomIn = cam.m_cameraZoomIn;
            ZoomOut = cam.m_cameraZoomOut;
        }

        private bool _ready = false;

        private GameObject _speakerObject;
        private AudioSource _speaker;
        private bool isPlaying { get { return _speaker.isPlaying; } }
        private SFX_PlayOneShot _interactSFX;

        private BatteryEntry _battery;
        private const float MAX_BATTERY_CHARGE = 180f;
        private OnOffEntry _onOffEntry;
        private bool isOn { get { return _onOffEntry.on; } }
        private VolumeEntry _volume;
        private TrackEntry _track;
        private TimeEntry _playbackTime;

        private BoomboxLocalVolumeSetting _localVolumeSetting;
        private BoomboxVolumeUpBindSetting _volumeUpBindSetting;
        private BoomboxVolumeDownBindSetting _volumeDownBindSetting;

        private static readonly int DEFAULT_ALERT_DISTANCE = 35;
        private static readonly float ALERT_INTERVAL = 0.15f;

        private float _alertCountdown = 0f;
        private int _trackIndex = -1;
        private float _instanceVolume = 0.5f;
        private float _inputDebounceTimer = 0f;

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
            if (!data.TryGetEntry(out _playbackTime))
            {
                _playbackTime = new TimeEntry()
                {
                    currentTime = 0f,
                };
                data.AddDataEntry(_playbackTime);
            }
            if (!data.TryGetEntry(out _track))
            {
                _track = new TrackEntry();
                data.AddDataEntry(_track);
            }
            #endregion
            itemInstance.onItemEquipped.AddListener(OnEquip);
            itemInstance.onUnequip.AddListener(OnUnequip);
            //
            _ready = true;
        }

        void Update()
        {
            if (_ready == false) return;

            // Local player interaction
            _inputDebounceTimer += Time.deltaTime;
            if (isHeldByMe && !Player.localPlayer.HasLockedInput() && GlobalInputHandler.CanTakeInput())
            {
                if (Player.localPlayer.input.clickWasPressed)
                {
                    _onOffEntry.on = !_onOffEntry.on;
                    _onOffEntry.SetDirty();
                    // Determines whether the monsters can hear TUCA DONKA
                    if (_onOffEntry.on == true && SpookboxPlugin.AlertMonsters.Value == true)
                    {
                        _alertCountdown = ALERT_INTERVAL;
                    }
                    PlayClickButtonSFX();
                }
                float axisRaw = Input.GetAxisRaw("Mouse ScrollWheel");
                if (_inputDebounceTimer > INPUT_DEBOUNCE_TIME && axisRaw > 0f || ZoomOut.action.WasPressedThisFrame())
                {
                    _inputDebounceTimer = 0f;
                    var newIdx = _track.TrackIndex == 0 ? Mixtape.Tracks.Count-1 : (_track.TrackIndex - 1);
                    SetTrack((newIdx % Mixtape.Tracks.Count));
                    PlayClickButtonSFX();
                }
                else if (_inputDebounceTimer > INPUT_DEBOUNCE_TIME && axisRaw < 0f || ZoomIn.action.WasPressedThisFrame())
                {
                    _inputDebounceTimer = 0f;
                    SetTrack(((_track.TrackIndex + 1) % Mixtape.Tracks.Count));
                    PlayClickButtonSFX();
                }
                if (Player.localPlayer.input.aimWasPressed)
                {
                    SetTrack(((_track.TrackIndex + 1) % Mixtape.Tracks.Count));
                    PlayClickButtonSFX();
                }
                if (GlobalInputHandler.GetKeyUp(_volumeUpBindSetting.Keycode()))
                {
                    _volume.Volume += 0.1f;
                    _volume.SetDirty();
                    AdjustVolume();
                    PlayClickButtonSFX();
                }
                if (GlobalInputHandler.GetKeyUp(_volumeDownBindSetting.Keycode()))
                {
                    _volume.Volume -= 0.1f;
                    _volume.SetDirty();
                    AdjustVolume();
                    PlayClickButtonSFX();
                }
            }

            if (_instanceVolume != _volume.Volume)
            {
                AdjustVolume();
                PlayClickButtonSFX();
            }

            if (_trackIndex != _track.TrackIndex)
            {
                SetTrack(_track.TrackIndex, _playbackTime.currentTime);
                if (isHeld && isHeldByMe == false)
                {
                    PlayClickButtonSFX();
                }
            }

            if (_onOffEntry.on != _speaker.isPlaying)
            {
                if (_onOffEntry.on)
                {
                    TryStartPlayback();
                }
                else
                {
                    TryStopPlayback();
                }
                if (isHeld && isHeldByMe == false)
                {
                    PlayClickButtonSFX();
                }
            }

            if (_onOffEntry.on == false)
            {
                return;
            }

            if (SpookboxPlugin.InfiniteBattery.Value == false)
            {
                _battery.m_charge -= Time.deltaTime;
            }

            if (SpookboxPlugin.AlertMonsters.Value)
            {
                _alertCountdown -= Time.deltaTime;
                if (_alertCountdown < 0f)
                {
                    var scaledAlertDist = DEFAULT_ALERT_DISTANCE * _volume.Volume;
                    SFX_Player.instance.PlayNoise(base.transform.position, scaledAlertDist);
                    _alertCountdown = ALERT_INTERVAL;
                }
            }

            if (_battery.m_charge < 0f)
            {
                _onOffEntry.on = false;
                _onOffEntry.SetDirty();
                PlayClickButtonSFX();
                return;
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
            SetTrack(_track.TrackIndex, _playbackTime.currentTime);
            TryStartPlayback();
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
        private GameObject TryGetDetachedSpeaker(Player player)
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
            stashedController.Configure(player, _onOffEntry, _battery, _volume.Volume);
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
            var dSpeaker = TryGetDetachedSpeaker(player);
            if (dSpeaker == null)
            {
                return false;
            }
            var speakerAudio = dSpeaker.GetComponent<AudioSource>();
            _playbackTime.currentTime = speakerAudio.time;
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

        /// <summary>
        /// Sets the track to the clip at the specific index.
        /// </summary>
        /// <remarks>
        /// Will only synchronise settings if the player holding it calls this method.
        /// </remarks>
        /// <param name="idx">The index of the track in <see cref="Mixtape.Tracks"/></param>
        /// <param name="trackTime">The seek time of the track. By default the track will play from the beginning</param>
        private void SetTrack(int idx, float trackTime = 0)
        {
            if (Mixtape.HasTracks() == false)
            {
                return;
            }
            var clip = Mixtape.Tracks[idx];
            if (clip == _speaker.clip)
            {
                return;
            }
            if (isHeldByMe)
            {
                // Sync to other players
                _playbackTime.currentTime = trackTime;
                _playbackTime.SetDirty();
                _track.TrackIndex = idx;
                _track.SetDirty();
            }
            //
            _speaker.clip = clip;
            _speaker.time = trackTime;
            _trackIndex = idx;
        }

        /// <summary>
        /// Starts playback of the current track if possible. Calling it while playing does nothing.
        /// </summary>
        /// <param name="resume"></param>
        /// <returns>Returns <see langword="true"/> if playback was started.</returns>
        private bool TryStartPlayback(bool resume = true)
        {
            if (_speaker.isPlaying == false && _onOffEntry.on)
            {
                _speaker.Play();
                if (resume)
                {
                    _speaker.time = _playbackTime.currentTime;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Stops playback of the current track if possible. Calling it while playing does nothing.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if playback was stopped.</returns>
        private bool TryStopPlayback()
        {
            if (_speaker.isPlaying && _onOffEntry.on == false)
            {
                _speaker.Stop();
                return true;
            }
            return false;
        }

        private void PlayClickButtonSFX()
        {
            if (_interactSFX != null)
            {
                _interactSFX.Play();
            }
        }

        /// <summary>
        /// Adjusts the local volume according to <see cref="BoomboxLocalVolumeSetting"/>.
        /// </summary>
        private void AdjustVolume()
        {
            var vol = _volume.Volume * _localVolumeSetting.Value;
            _speaker.volume = vol;
            _instanceVolume = _volume.Volume;
        }
    }
}
