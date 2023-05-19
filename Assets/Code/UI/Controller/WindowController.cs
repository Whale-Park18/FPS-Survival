using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhalePark18.UI.Window;

public class WindowController : MonoBehaviour
{
    private Type enumType;
    private List<WindowBase> listWindow;

    public Type EnumType { set { enumType = value; } }

    private void Awake()
    {
        listWindow = new List<WindowBase>();
    }

    /// <summary>
    /// windowType Ÿ���� ������ Ȱ��ȭ/��Ȱ��ȭ �޼ҵ�
    /// </summary>
    /// <param name="windowType"></param>
    public void SetWindowActive(Enum windowType)
    {
        listWindow[(int)Enum.Parse(enumType, windowType.ToString())].OnWindowPower();
    }
}
