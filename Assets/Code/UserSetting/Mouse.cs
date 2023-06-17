using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.UserSetting
{
    [Serializable]
    public class Mouse
    {
        [Tooltip("熱霜 團馬紫")]
        [Range(0f, 1f)]
        public float VerticalSensitivity;

        [Tooltip("熱ゎ 團馬紫")]
        [Range(0f, 1f)]
        public float HorizontalSensitivity;

        public Mouse(float verticalSensitivity, float horizontalSensitivity)
        {
            VerticalSensitivity = verticalSensitivity;
            HorizontalSensitivity = horizontalSensitivity;
        }
    }
}