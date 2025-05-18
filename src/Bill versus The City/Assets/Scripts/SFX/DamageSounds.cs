using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSounds : MonoBehaviour, ICharStatusSubscriber
{
    public const string DAMAGE_SOUND_PATH = "damage_chiptone";
    public const string DAMAGE_PLAYER_SOUND_PATH = "damage_grunt_player";
    public const string DEATH_SOUND_PATH = "death_grunt";
    public const string DEATH_PLAYER_SOUND_PATH = "death_grunt_player";
    // private ISoundSet reload_start_sound, reload_complete_sound;
    private static ISFXSounds damage_grunts, damage_grunts_player, death_grunts, death_grunts_player;
    private ICharacterStatus status;
    public CharCtrl target_character;
    private bool is_player = false;

    private GameObject current_damage_sounds = null;

    private bool killed = false;
    public bool only_death_sounds = true;

    void Awake() {
        damage_grunts = SFXLibrary.LoadSound(DAMAGE_SOUND_PATH);
        damage_grunts_player = SFXLibrary.LoadSound(DAMAGE_PLAYER_SOUND_PATH);
        death_grunts = SFXLibrary.LoadSound(DEATH_SOUND_PATH);
        death_grunts_player = SFXLibrary.LoadSound(DEATH_PLAYER_SOUND_PATH);
    }

    // Start is called before the first frame update
    void Start()
    {
        status = target_character.GetStatus();
        status.Subscribe(this);
        try {
            ManualCharacterMovement _ = (ManualCharacterMovement) target_character;
            is_player = true;
        } catch (InvalidCastException) {
            is_player = false;
        }
        // reload_start_sound = SFXLibrary.LoadSound(reload_start_sound_path);
        // if (reload_start_sound == null) {
        //     Debug.LogWarning($"Reload Start Sound null for {gameObject}");
        // }
        // reload_complete_sound = SFXLibrary.LoadSound(reload_complete_sound_path);
        // if (reload_complete_sound == null) {
        //     Debug.LogWarning($"Reload Complete Sound null for {gameObject}");
        // }
    }
    
    void OnDestroy() {
        status.Unsubscribe(this);
    }
    
    public void OnDamage(ICharacterStatus status) {
        if (status.adjusting_difficulty) { return; } // don't play damage sounds if health was adjusted for difficulty
        PlayDamageSound();
    }
    public void OnHeal(ICharacterStatus status) {
        // do nothing
    }

    public void OnDeath(ICharacterStatus _)  {
        // TODO --- implement death sounds
        if (!killed) {
            killed = true;
            PlayDeathSound();
        }
    }
    
    private float last_health_Value = -1f;
    public void StatusUpdated(ICharacterStatus status) {
        if (!killed && !only_death_sounds && last_health_Value > status.health) {
            PlayDeathSound();
        }
        last_health_Value = status.health;
    }

    private void PlayDamageSound() {
        // NOTE: gameObject == null if the gameObject was Destroyed 
        if (current_damage_sounds != null) { return; } // damage sound already playing
        ISFXSounds sounds;
        if (is_player) {
            sounds = damage_grunts_player;
        } else {
            sounds = damage_grunts;
        }
        AudioSource source = SFXSystem.inst.PlaySound(sounds, transform.position);
        current_damage_sounds = source.gameObject;
    }


    private void PlayDeathSound() {
        ISFXSounds sounds;
        if (is_player) {
            sounds = death_grunts_player;
        } else {
            sounds = death_grunts;
        }
        SFXSystem.inst.PlaySound(sounds, transform.position);
    }

}
