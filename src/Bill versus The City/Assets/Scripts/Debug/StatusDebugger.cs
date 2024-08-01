using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDebugger : MonoBehaviour
{

    public float health = -1;
    public float max_health = -1;
    public float max_armor = -1;
    public float armor = -1;
    public float armor_hardness = -1;
    public float armor_protection = -1;

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

        if (status.armor == null) {
            SetArmorNull();
        }
        else {
            max_armor = status.armor.armor_max_durability;
            armor = status.armor.armor_durability;
            armor_hardness = status.armor.armor_hardness;
            armor_protection = status.armor.armor_protection;
        } 
    }

    private void SetArmorNull() {
        max_armor = 0;
        armor = 0;
        armor_hardness = 0;
        armor_protection = 0;
    }
}
