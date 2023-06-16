using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.UserSetting
{
    [CreateAssetMenu(fileName = "Sound Setting", menuName = "ScriptObject/Sound Setting")]
    public class Sound : ScriptableObject
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
    }
}