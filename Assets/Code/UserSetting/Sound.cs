using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.UserSetting
{
    [Serializable]
    public class Sound
    {
        [Tooltip("���Ұ�")]
        public bool Mute;

        [Tooltip("��ü ����")]
        [Range(0f, 1f)]
        public float MasterVolume;

        [Tooltip("�÷��̾� ����")]
        [Range(0f, 1f)]
        public float PlayerVolume;

        [Tooltip("������ ����")]
        [Range(0f, 1f)]
        public float ItemVolume;

        [Tooltip("�뷡 ����")]
        [Range(0f, 1f)]
        public float MusicVolume;

        public Sound(bool mute, float masterVolume, float playerVolume, float itemVolume, float musicVolume)
        {
            Mute = mute;
            MasterVolume = masterVolume;
            PlayerVolume = playerVolume;
            ItemVolume = itemVolume;
            MusicVolume = musicVolume;
        }
    }
}