using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsJsonInspector : MonoBehaviour
{

    public string json_output = "";

    // Update is called once per frame
    void Update()
    {
        json_output = GameSettings.inst.AsJson();
    }
}
