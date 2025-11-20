using UnityEngine;
using ContentWarningShop.Localisation;

namespace Spookbox.Entries
{
    public class VolumeEntry : ItemDataEntry, IHaveUIData
    {
        public const string VOLUME_LOC_KEY_NAME = "spookbox_volume";
        private string _labelText = "Volume";
        private float _volume = 0f;
        public float Volume 
        { 
            get => _volume;
            set
            {
                _volume = Mathf.Clamp01(value);
            }
        }
        public const float MaxVolume = 1f;
        public string UIString = "{0}% {1}";

        public override void Deserialize(Zorro.Core.Serizalization.BinaryDeserializer binaryDeserializer)
        {
            // Landfall asked for as little data as possible over photon. All I ask for is one byte <3 (+1 for length....)
            Volume = ((float)binaryDeserializer.ReadByte())/100f;
        }

        public override void Serialize(Zorro.Core.Serizalization.BinarySerializer binarySerializer)
        {
            binarySerializer.WriteByte((byte)Mathf.RoundToInt(Volume * 100));
        }

        public string GetString()
        {
            return string.Format(UIString, Mathf.RoundToInt(Volume * 100), _labelText);
        }

        public void UpdateLocale()
        {
            if (ShopLocalisation.TryGetLocaleString("spookbox_volume", out string label))
            {
                _labelText = label;
            }
        }
    }
}
