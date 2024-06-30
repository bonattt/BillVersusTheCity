using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDebugger : MonoBehaviour
{

    public float health = -1;
    public float max_health = -1;
    public float armor = -1;
    public float armor_hardness = -1;

    private ICharacterStatus _status;
    private ICharacterStatus status {
        get {
            if (_status == null) {
                _status = GetComponent<CharCtrl>().GetStatus();
            }
            return _status;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (status == null) {
            Debug.LogError("is null");
        } 
        health = status.health;
        max_health = status.max_health;
        armor = status.armor;
        armor_hardness = status.armor_hardness;
    }
}
