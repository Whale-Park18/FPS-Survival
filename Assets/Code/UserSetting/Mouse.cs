using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.UserSetting
{
    [Serializable]
    public class Mouse
    {
        [Tooltip("���� �ΰ���")]
        [Range(0f, 1f)]
        public float VerticalSensitivity;

        [Tooltip("���� �ΰ���")]
        [Range(0f, 1f)]
        public float HorizontalSensitivity;

        public Mouse(float verticalSensitivity, float horizontalSensitivity)
        {
            VerticalSensitivity = verticalSensitivity;
            HorizontalSensitivity = horizontalSensitivity;
        }
    }
}