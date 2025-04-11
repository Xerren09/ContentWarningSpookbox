using Zorro.Settings.DebugUI;
using Zorro.Settings;
using UnityEngine;
using Zorro.Core;
using TMPro;

namespace Spookbox.Settings
{
    /// <summary>
    /// Custom setting that displays a standard game UI button.
    /// </summary>
    public abstract class ButtonSetting : Setting
    {
        public override SettingUI GetDebugUI(ISettingHandler settingHandler)
        {
            return new ButtonSettingUI(OnClick, GetButtonText());
        }

        public override GameObject GetSettingUICell()
        {
            // Hack the default KeyCode UI to function as a regular button
            /*
                UnityEngine.Object.Instantiate(setting.GetSettingUICell(), inputContainer).GetComponent<SettingInputUICell>().Setup(setting, settingHandler);
             */
            var ui = SingletonAsset<InputCellMapper>.Instance.KeyCodeSettingCell;
            var edit = GameObject.Instantiate(ui);
            var rm = edit.GetComponent<KeyCodeSettingUI>();
            // Destroy the original component immediately so when the settings builder looks for a SettingInputUICell
            // we are found first. Normal destroy would defer it to end of frame, which is too late
            GameObject.DestroyImmediate(rm);
            edit.AddComponent<ButtonInputSettingUI>();
            return edit;
        }

        /// <summary>
        /// The action text that displays on the settings's UI button.
        /// </summary>
        /// <returns></returns>
        public abstract string GetButtonText();

        /// <summary>
        /// Invoked when the setting's corresponding UI button is clicked.
        /// </summary>
        public abstract void OnClick();
    }

    /// <summary>
    /// UXML based UI (UnityEngine.UIElements)
    /// </summary>
    public class ButtonSettingUI : SettingUI
    {
        private UnityEngine.UIElements.Button _button;

        public ButtonSettingUI(Action OnClick, string text)
        {
            _button = new UnityEngine.UIElements.Button(OnClick);
            _button.text = text;
        }
    }

    /// <summary>
    /// Canvas based UI (UnityEngine.UI) - Used by the game.
    /// </summary>
    public class ButtonInputSettingUI : SettingInputUICell
    {
        private UnityEngine.UI.Button _button;
        private ButtonSetting _setting;

        public override void Setup(Setting setting, ISettingHandler settingHandler)
        {
            if (setting is not ButtonSetting)
            {
                return;
            }
            _setting = setting as ButtonSetting;
            _button = gameObject.GetComponentInChildren<UnityEngine.UI.Button>();
            if (setting is ButtonSetting exSetting)
            {
                var text = _button.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = exSetting.GetButtonText();
                }
            }
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _setting.OnClick();
        }
    }
}
