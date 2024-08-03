using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CharacterStatus : MonoBehaviour, ICharacterStatus {
    
    public float _health = 0;
    public float health { 
        get {
            return _health;
        }
        set {
            _health = value;
            if (_health < 0) {
                _health = 0;
            }
            else if (_health > _max_health) {
                _health = _max_health;
            }  
            UpdateStatus();
        }
    }
    public float _max_health = 100;
    public float max_health { 
        get {
            return _max_health;
        }
        set {
            _max_health = value;
            if (_max_health < 0) {
                _max_health = 0;
            }
            if (_health > _max_health) {
                _health = _max_health;
            }
            UpdateStatus();
        }
    }
    public ArmorPlate armor_init;
    public IArmor _armor;
    public IArmor armor { 
        get { return _armor; }
        set { _armor = value; }
    }
    private List<ICharStatusSubscriber> subscribers = new List<ICharStatusSubscriber>();

    public CharacterStatus() : this(100f) { /* do nothing */ }
    public CharacterStatus(float max_health) {
    }
    
    void Start() {
        if (_health <= 0 || _health > max_health) {
            this.health = max_health;
        }
        this.armor = null;
        if (armor_init != null) { 
            try {
                this.armor = (IArmor) Instantiate(armor_init);
            } catch (InvalidCastException) {
                Debug.LogError($"invalid init armor: {armor_init}");
            }
        }
    }   

    void Update() {
        SetDebug();
    }

    public bool _debug_has_armor = false;
    public float _debug_max_armor = -1f;
    public float _debug_armor = -1f;
    public float _debug_armor_protection = -1f;
    private void SetDebug() {
        // sets some values for debugging from the inspector
        if (armor == null) {
            _debug_has_armor = false;
            _debug_max_armor = 0f;
            _debug_armor = 0f;
            _debug_armor_protection = 0f;
        }
        else {
            _debug_has_armor = true;
            _debug_max_armor = armor.armor_max_durability;
            _debug_armor = armor.armor_durability;
            _debug_armor_protection = armor.armor_protection;
        }
    }

    public void Subscribe(ICharStatusSubscriber sub) => subscribers.Add(sub);

    public void Unsubscribe(ICharStatusSubscriber sub) => subscribers.Remove(sub);

    public void UpdateStatus() {
        foreach(ICharStatusSubscriber sub in subscribers) {
            sub.StatusUpdated(this);
        }
    }
}