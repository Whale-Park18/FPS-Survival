using Mono.CompilerServices.SymbolWriter;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using WhalePark18.Manager;

namespace WhalePark18.UI.Window
{
    [System.Serializable]
    struct TextAndSlider
    {
        public TextMeshProUGUI text;
        public Slider slider;
    }

    public class WindowSetting : WindowBase
    {
        [Header("Control")]
        [SerializeField]
        [Tooltip("마우스 수평 민감도")]
        private TextAndSlider _horizontalSensitivity;

        [SerializeField]
        [Tooltip("마우스 수직 민감도")]
        private TextAndSlider _verticalSensitivity;

        [Header("Sound")]
        [SerializeField]
        [Tooltip("전체 음량")]
        private TextAndSlider _masterVolume;

        [SerializeField]
        [Tooltip("플레이어 음량")]
        private TextAndSlider _playerVolume;

        [SerializeField]
        [Tooltip("아이템 음량")]
        private TextAndSlider _itemVolume;

        [SerializeField]
        [Tooltip("음악 음량")]
        private TextAndSlider _musicVolume;

        [Header("버튼")]
        [SerializeField]
        [Tooltip("나가기 버튼")]
        private Button _exitButton;

        private void Awake()
        {
            OnInitialized();
            SliderEventBinding();
            ButtonBinding();
        }

        private void OnInitialized()
        {
            float sensitivity = GameManager.Instance.MouseSetting.HorizontalSensitivity;
            _horizontalSensitivity.text.text = (sensitivity * 100).ToString();
            _horizontalSensitivity.slider.value = sensitivity;

            sensitivity = GameManager.Instance.MouseSetting.VerticalSensitivity;
            _verticalSensitivity.text.text = (sensitivity * 100).ToString();
            _verticalSensitivity.slider.value = sensitivity;

            float volume = SoundManager.Instance.SoundSetting.MasterVolume;
            _masterVolume.text.text = (volume * 100).ToString();
            _masterVolume.slider.value = volume;

            volume = SoundManager.Instance.SoundSetting.PlayerVolume;
            _playerVolume.text.text = (volume * 100).ToString();
            _playerVolume.slider.value = volume;

            volume = SoundManager.Instance.SoundSetting.ItemVolume;
            _itemVolume.text.text = (volume * 100).ToString();
            _itemVolume.slider.value = volume;

            volume = SoundManager.Instance.SoundSetting.MusicVolume;
            _musicVolume.text.text = (volume * 100).ToString();
            _musicVolume.slider.value = volume;
        }

        private void SliderEventBinding()
        {
            _horizontalSensitivity.slider.onValueChanged.AddListener(delegate { HorizontalSEnsitivityChanged(); });
            _verticalSensitivity.slider.onValueChanged.AddListener(delegate { VerticalSEnsitivityChanged(); });

            _masterVolume.slider.onValueChanged.AddListener(delegate {  MasterVolumeChanged(); });  
            _playerVolume.slider.onValueChanged.AddListener(delegate { PlayerVolumeChanged(); });   
            _itemVolume.slider.onValueChanged.AddListener(delegate { ItemVolumeChanged(); });
            _musicVolume.slider.onValueChanged.AddListener(delegate { MusicVolumeChanged(); });
        }

        private void HorizontalSEnsitivityChanged()
        {
            float sensitivity = _horizontalSensitivity.slider.value;
            sensitivity = Mathf.Round(sensitivity * 1000);
            sensitivity /= 1000;

            GameManager.Instance.MouseSetting.HorizontalSensitivity = sensitivity;
            _horizontalSensitivity.text.text = sensitivity.ToString();
        }

        private void VerticalSEnsitivityChanged()
        {
            float sensitivity = _verticalSensitivity.slider.value;
            sensitivity = Mathf.Round(sensitivity * 1000);
            sensitivity /= 1000;

            GameManager.Instance.MouseSetting.VerticalSensitivity = sensitivity;
            _verticalSensitivity.text.text = sensitivity.ToString();
        }

        private void MasterVolumeChanged()
        {
            float volume = _masterVolume.slider.value;
            volume = Mathf.Round(volume * 1000);
            volume /= 1000;

            SoundManager.Instance.SoundSetting.MasterVolume = volume;
            _masterVolume.text.text = (volume * 100).ToString();

            SoundManager.Instance.ResetAudioVolume();
        }

        private void PlayerVolumeChanged()
        {
            float volume = _playerVolume.slider.value;
            volume = Mathf.Round(volume * 1000);
            volume /= 1000;

            SoundManager.Instance.SoundSetting.PlayerVolume = volume;
            _playerVolume.text.text = (volume * 100).ToString();

            SoundManager.Instance.ResetAudioVolume();
        }

        private void ItemVolumeChanged()
        {
            float volume = _itemVolume.slider.value;
            volume = Mathf.Round(volume * 1000);
            volume /= 1000;

            SoundManager.Instance.SoundSetting.ItemVolume = volume;
            _itemVolume.text.text = (volume * 100).ToString();

            SoundManager.Instance.ResetAudioVolume();
        }

        private void MusicVolumeChanged()
        {
            float volume = _musicVolume.slider.value;
            volume = Mathf.Round(volume * 1000);
            volume /= 1000;

            SoundManager.Instance.SoundSetting.MusicVolume = volume;
            _musicVolume.text.text = (volume * 100).ToString();

            SoundManager.Instance.ResetAudioVolume();
        }


        private void ButtonBinding()
        {
            _exitButton.onClick.AddListener(OnClickExit);

            SoundManager.Instance.ResetAudioVolume();
        }

        private void OnClickExit()
        {
            gameObject.SetActive(false);
        }
    }
}