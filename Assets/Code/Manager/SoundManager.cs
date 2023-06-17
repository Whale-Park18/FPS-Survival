using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using WhalePark18.UserSetting;

namespace WhalePark18.Manager
{
    public enum AudioType { Player, Item, Music };

    [Serializable]
    public struct AudioData
    {
        public AudioSource audioSource;
        public AudioType type;

        public AudioData(AudioSource newAudioSource, AudioType newType)
        {
            audioSource = newAudioSource;
            type = newType;
        }
    }

    public class SoundManager : MonoSingleton<SoundManager>
    {
        [Header("Sound Setting")]
        [Tooltip("소리 설정 파일 경로")]
        public string DetailPath;

        [Tooltip("Json 파일 이름")]
        public string JsonFileName;

        public Sound SoundSetting { get => _soundSetting; }

        // Sound Setting
        private string _path;
        private Sound _soundSetting;

        // Audio Management
        private Dictionary<int, AudioData> _audioesInScene = new Dictionary<int, AudioData>();

        protected override void Awake()
        {
            base.Awake();

            _path = Application.dataPath + $"/{DetailPath}/{JsonFileName}.json";
            if(File.Exists(_path))
            {
                var json = File.ReadAllText(_path);
                _soundSetting = JsonUtility.FromJson<Sound>(json);
            }
            else
            {
                var directoryPath = Application.dataPath + $"/{DetailPath}";
                if (Directory.Exists(directoryPath) == false)
                    Directory.CreateDirectory(directoryPath);

                _soundSetting = new Sound(false, 1f, 1f, 1f, 1f);

                var jsonFile = File.CreateText(_path);
                jsonFile.Write(JsonUtility.ToJson(JsonUtility.ToJson(_soundSetting)));
                jsonFile.Close();
            }
        }

        protected override void OnApplicationQuit()
        {
            LogManager.ConsoleDebugLog($"{name}", "OnApplicationQuit");

            base.OnApplicationQuit();

            BackupSetting();
        }

        public void BackupSetting()
        {
            var json = JsonUtility.ToJson(_soundSetting);
            File.WriteAllText(_path, json);
        }

        private float CalculateVolume(AudioType type)
        {
            float volume = _soundSetting.MasterVolume;

            switch(type)
            {
                case AudioType.Player:
                    volume *= _soundSetting.PlayerVolume;
                    break;

                case AudioType.Item:
                    volume *= _soundSetting.ItemVolume;
                    break;

                case AudioType.Music:
                    volume *= _soundSetting.MusicVolume;
                    break;
            }

            return volume;
        }

        public void AddAudioSource(AudioData audioData)
        {
            var id = audioData.audioSource.GetInstanceID();

            if (_audioesInScene.ContainsKey(id))
                return;

            _audioesInScene.Add(id, audioData);
            audioData.audioSource.volume = CalculateVolume(audioData.type);
        }

        public void RemoveAudioSource(AudioData audioData)
        {
            _audioesInScene.Remove(audioData.audioSource.GetInstanceID());
        }

        /// <summary>
        /// 관리 중인 오디오 소스의 음량 재설정하는 메소드
        /// </summary>
        public void ResetAudioVolume()
        {
            foreach(var audioData in _audioesInScene.Values)
            {
                AudioType audioType = new AudioType();
                switch(audioData.type)
                {
                    case AudioType.Player:
                        audioType = AudioType.Player;
                        break;

                    case AudioType.Item:
                        audioType = AudioType.Item;
                        break;

                    case AudioType.Music:
                        audioType = AudioType.Music;
                        break;
                }

                audioData.audioSource.volume = CalculateVolume(audioType);
            }
        }
    }
}