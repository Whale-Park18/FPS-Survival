using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18
{
    public enum DebugCategory { Debug, Error, Warning }

    public class Debug : MonoBehaviour
    {
        public static void Log(DebugCategory category, string title)
        {
            UnityEngine.Debug.LogFormat($"<color={GetCategoryColor(category)}><b>[{title}]</b></color>");
        }

        public static void Log(DebugCategory category, string title, string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat($"<color={GetCategoryColor(category)}><b>[{title}]</b></color>\n" + format, args);
        }

        public static string GetCategoryColor(DebugCategory category)
        {
            string color = string.Empty;

            switch (category)
            {
                case DebugCategory.Debug:
                    color = "green";
                    break;

                case DebugCategory.Error:
                    color = "yellow";
                    break;

                case DebugCategory.Warning:
                    color = "red";
                    break;
            }

            return color;
        }
    }
}