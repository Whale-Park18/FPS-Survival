using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        [Header("Setting Asset")]
        public Sound SoundSetting;

        // The AudioSorce in the scene
        private Dictionary<int, AudioData> audioesInScene = new Dictionary<int, AudioData>();

        private float CalculateVolume(AudioType type)
        {
            float volume = SoundSetting.MasterVolume;

            switch(type)
            {
                case AudioType.Player:
                    volume *= SoundSetting.PlayerVolume;
                    break;

                case AudioType.Item:
                    volume *= SoundSetting.ItemVolume;
                    break;

                case AudioType.Music:
                    volume *= SoundSetting.MusicVolume;
                    break;
            }

            return volume;
        }

        public void AddAudioSource(AudioData audioData)
        {
            var id = audioData.audioSource.GetInstanceID();

            if (audioesInScene.ContainsKey(id))
                return;

            audioesInScene.Add(id, audioData);
            audioData.audioSource.volume = CalculateVolume(audioData.type);
        }

        public void RemoveAudioSource(AudioData audioData)
        {
            audioesInScene.Remove(audioData.audioSource.GetInstanceID());
        }

        /// <summary>
        /// 관리 중인 오디오 소스의 음량 재설정하는 메소드
        /// </summary>
        public void ResetAudioVolume()
        {
            foreach(var audioData in audioesInScene.Values)
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