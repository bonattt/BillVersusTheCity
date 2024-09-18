using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugModeSetter : MonoBehaviour
{
    public bool set_debug_mode = false;
    // Start is called before the first frame update
    void Start()
    {
        SetDebugMode();
    }

    // Update is called once per frame
    void Update()
    {
        SetDebugMode();
    }

    private void SetDebugMode() {
        DebugMode.inst.debug_enabled = set_debug_mode;
    }
}
