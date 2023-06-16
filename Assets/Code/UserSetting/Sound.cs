using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.UserSetting
{
    [CreateAssetMenu(fileName = "Sound Setting", menuName = "ScriptObject/Sound Setting")]
    public class Sound : ScriptableObject
    {
        [Tooltip("음소거")]
        public bool Mute;

        [Tooltip("전체 음량")]
        [Range(0f, 1f)]
        public float MasterVolume;

        [Tooltip("플레이어 음량")]
        [Range(0f, 1f)]
        public float PlayerVolume;

        [Tooltip("아이템 음량")]
        [Range(0f, 1f)]
        public float ItemVolume;

        [Tooltip("노래 음량")]
        [Range(0f, 1f)]
        public float MusicVolume;
    }
}