using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CharacterStatus : ICharacterStatus {
    
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
    public float _max_health = 0;
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
    public float _armor = 0;
    public float armor { 
        get {
            return _armor;
        }
        set {
            _armor = value;
            UpdateStatus();
        }
    }
    public float _armor_hardness = 0;
    public float armor_hardness { 
        get {
            return _armor_hardness;
        }
        set {
            _armor_hardness = value;
            UpdateStatus();
        }
    }
    private List<ICharStatusSubscriber> subscribers = new List<ICharStatusSubscriber>();

    public CharacterStatus() : this(100f) { /* do nothing */ }
    public CharacterStatus(float max_health) {
        this.max_health = max_health;
        this.health = max_health;
        this.armor = 0;
        this.armor_hardness = 0;
        Debug.Log($"health: {health} / {this.max_health}");
    }

    public void Subscribe(ICharStatusSubscriber sub) => subscribers.Add(sub);

    public void Unsubscribe(ICharStatusSubscriber sub) => subscribers.Remove(sub);

    public void UpdateStatus() {
        Debug.Log($"UpdateStatus: health {health}/{max_health}, armor {armor}");
        foreach(ICharStatusSubscriber sub in subscribers) {
            sub.StatusUpdated(this);
        }
    }
}