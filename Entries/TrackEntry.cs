using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Spookbox.Entries
{
    public class TrackEntry : ItemDataEntry, IHaveUIData
    {
        private int _trackIndex = 0;
        public int TrackIndex 
        {
            get => _trackIndex;
            set
            {
                if (Mixtape.Tracks.Count == 0)
                {
                    return;
                }
                _trackIndex = Math.Clamp(value, 0, Mixtape.Tracks.Count-1);
                TrackName = FormatTrackName();
            } 
        }
        public string TrackName { get; private set; } = NO_TRACKS;
        private const string NO_TRACKS = "No Tracks Found :[";
        private const int MAX_TRACK_NAME_LENGTH = 17;

        public override void Deserialize(Zorro.Core.Serizalization.BinaryDeserializer binaryDeserializer)
        {
            TrackIndex = binaryDeserializer.ReadByte();
        }

        public string GetString()
        {
            return TrackName;
        }

        public override void Serialize(Zorro.Core.Serizalization.BinarySerializer binarySerializer)
        {
            binarySerializer.WriteByte((byte)TrackIndex);
        }

        private string FormatTrackName()
        {
            string UIString = string.Empty;
            var tracks = Mixtape.Tracks;
            if (tracks.Count == 0)
            {
                return NO_TRACKS;
            }
            UIString = Path.GetFileNameWithoutExtension(tracks[TrackIndex].name);
            if (UIString.Length > MAX_TRACK_NAME_LENGTH)
            {
                UIString = UIString.Substring(0, MAX_TRACK_NAME_LENGTH) + "...";
            }
            return UIString;
        }

        public void UpdateLocale()
        {
            // No label
        }
    }
}
