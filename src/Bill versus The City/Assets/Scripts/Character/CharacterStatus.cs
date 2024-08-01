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
    public IArmor _armor;
    public IArmor armor { 
        get { return _armor; }
        set { _armor = value; }
    }
    private List<ICharStatusSubscriber> subscribers = new List<ICharStatusSubscriber>();

    public CharacterStatus() : this(100f) { /* do nothing */ }
    public CharacterStatus(float max_health) {
        this.max_health = max_health;
        this.health = max_health;
        this.armor = null;
    }

    public void Subscribe(ICharStatusSubscriber sub) => subscribers.Add(sub);

    public void Unsubscribe(ICharStatusSubscriber sub) => subscribers.Remove(sub);

    public void UpdateStatus() {
        foreach(ICharStatusSubscriber sub in subscribers) {
            sub.StatusUpdated(this);
        }
    }
}