using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.UserSetting
{
    [CreateAssetMenu(fileName = "Mouse Setting", menuName = "ScriptObject/Mouse Setting")]
    public class Mouse : ScriptableObject
    {
        [Tooltip("熱霜 團馬紫")]
        [Range(0f, 1f)]
        public float VirticalSensitivity;

        [Tooltip("熱ゎ 團馬紫")]
        [Range(0f, 1f)]
        public float HorizontalSensitivity;
    }
}