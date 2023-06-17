using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WhalePark18.Manager;

public class Test : MonoBehaviour
{
    public Data data;

    private void Start()
    {
        var strJson = File.ReadAllText(Application.dataPath + "/TestJson.json");
        data = JsonUtility.FromJson<Data>(strJson);
        LogManager.ConsoleDebugLog($"{name}", $"json: {data}");

        var json = JsonUtility.ToJson(data);
        LogManager.ConsoleDebugLog($"{name}", $"json: {json}");
        File.WriteAllText(Application.dataPath + "/TestJson.json", json);
    }
}
