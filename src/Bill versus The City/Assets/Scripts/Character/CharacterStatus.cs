using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CharacterStatus : MonoBehaviour, ICharacterStatus, ISettingsObserver {
    
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
        protected set { _armor = value; }
    }
    private List<ICharStatusSubscriber> subscribers = new List<ICharStatusSubscriber>();

    public bool is_player { get { return false; }} // TODO --- implement this

    private float health_multiplier = 1f; // set by difficulty
    private void SetHealthDifficulty() {
        // adjusts health and max_healht based on the games difficulty settings 
        float previous_multiplier = health_multiplier; 
        if (is_player) {
            health_multiplier = 1f;
        } else {
            health_multiplier = GameSettings.inst.difficulty_settings.GetMultiplier(DifficultySettings.ENEMY_HEALTH);
        }
        // remove previous multiplier and add new multiplier
        _max_health = Mathf.Round(_max_health * health_multiplier / previous_multiplier);
        _health = Mathf.Round(_health * health_multiplier / previous_multiplier);
    }

    private string health_difficulty_field { 
        get {
            if (is_player) { return DifficultySettings.PLAYER_HEALTH; }
            return DifficultySettings.ENEMY_HEALTH;
        }
    }

    private string armor_difficulty_field { 
        get {
            if (is_player) { return DifficultySettings.PLAYER_ARMOR; }
            return DifficultySettings.ENEMY_ARMOR;
        }
    }
    
    public void SettingsUpdated(ISettingsModule updated, string field) {
        DifficultySettings difficulty;
        try {
            difficulty = (DifficultySettings) updated;
        } catch (InvalidCastException) {
            // do nothing, armor only cares about DifficultySettings
            return;
        }
        
        if (field == this.health_difficulty_field) {
            SetHealthDifficulty();
        }
        else if (field == this.armor_difficulty_field) {
            SetArmorDifficulty();
        }
    }

    void OnDestroy() {
        GameSettings.inst.difficulty_settings.Unsubscribe(this);
    }

    void Start() {
        GameSettings.inst.difficulty_settings.Subscribe(this);
        SetHealthDifficulty();
        if (_health <= 0 || _health > max_health) {
            this.health = max_health;
        }
        this.armor = null;
        if (armor_init != null) { 
            ApplyNewArmor(armor_init);
        }
        UpdateStatus();
    }  

    public void ApplyNewArmor(ScriptableObject armor_template) {
        // takes a ScriptableObject, instantiates a new copy of it with full durability
        try {
            IArmor new_armor = (IArmor) Instantiate(armor_init);
            RemoveArmor();
            this.armor = new_armor;
            SetArmorDifficulty();
            UpdateStatus();
        } catch (InvalidCastException) {
            Debug.LogError($"invalid init armor: {armor_init}");
        }
    }

    public void ApplyExistingArmor(IArmor existing_armor) {
        // equips armor to the character, without copying the armor, so pre-existing durability is preserved
        RemoveArmor();
        this.armor = existing_armor;
        SetArmorDifficulty();  // 
    }

    public void RemoveArmor() {
        // removes this.armor, and cleans up it's dependencies
        armor = null;
        UpdateStatus();
    }

    private void SetArmorDifficulty() {
        if (armor == null) { return; }
        
        float difficulty = GameSettings.inst.difficulty_settings.GetMultiplier(armor_difficulty_field);
        this.armor.SetDiffcultyMultiplier(difficulty);
        UpdateStatus();
    }

    void Update() {
        SetDebug();
    }

    public bool _debug_has_armor = false;
    public float _debug_max_armor = -1f;
    public float _debug_armor = -1f;
    public float _debug_armor_protection = -1f;
    public bool _debug_is_player = false;
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
        _debug_is_player = is_player;
    }

    public void Subscribe(ICharStatusSubscriber sub) => subscribers.Add(sub);

    public void Unsubscribe(ICharStatusSubscriber sub) => subscribers.Remove(sub);

    public void UpdateStatus() {
        foreach(ICharStatusSubscriber sub in subscribers) {
            sub.StatusUpdated(this);
        }
    }
}