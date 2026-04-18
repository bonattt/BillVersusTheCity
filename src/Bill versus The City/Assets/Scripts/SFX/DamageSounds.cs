using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSoundEffect : MonoBehaviour, ICharStatusSubscriber
{
    public const string DAMAGE_ENEMY_SOUND_PATH = "damage_chiptone";
    public const string DAMAGE_NPC_SOUND_PATH = "damage_chiptone";
    public const string DAMAGE_PLAYER_SOUND_PATH = "damage_grunt_player";
    public const string DEATH_SOUND_PATH = "death_grunt";
    public const string DEATH_PLAYER_SOUND_PATH = "death_grunt_player";
    public const string DEATH_NPC_SOUND_PATH = "death_npc";
    // private ISoundSet reload_start_sound, reload_complete_sound;
    private static ISFXSounds damage_grunts_enemy, damage_grunts_npc, damage_grunts_player, death_grunts_enemy, death_grunts_player, death_grunts_npc;

    private ICharacterStatus status;
    public CharCtrl target_character;

    public bool cancel_damage_sound_on_death = true;

    private GameObject current_damage_sounds = null;

    private bool killed = false;
    public bool only_death_sounds = true;

    void Awake() {
        damage_grunts_enemy = SFXLibrary.LoadSound(DAMAGE_ENEMY_SOUND_PATH);
        damage_grunts_npc = SFXLibrary.LoadSound(DAMAGE_NPC_SOUND_PATH);
        damage_grunts_player = SFXLibrary.LoadSound(DAMAGE_PLAYER_SOUND_PATH);
        death_grunts_enemy = SFXLibrary.LoadSound(DEATH_SOUND_PATH);
        death_grunts_player = SFXLibrary.LoadSound(DEATH_PLAYER_SOUND_PATH);
        death_grunts_npc = SFXLibrary.LoadSound(DEATH_NPC_SOUND_PATH);
    }

    // Start is called before the first frame update
    void Start()
    {
        status = target_character.GetStatus();
        status.Subscribe(this);


        // try {
        //     ManualCharacterMovement _ = (ManualCharacterMovement) target_character;
        //     is_player = true;

        // } catch (InvalidCastException) {
        //     is_player = false;
        // }

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
        ISFXSounds sounds = GetDamageSound();
        AudioSource source = SFXSystem.inst.PlaySound(sounds, transform.position);
        current_damage_sounds = source.gameObject; // gameObject will == null once this manager destroys itself
    }


    private void PlayDeathSound() {
        if (current_damage_sounds != null && cancel_damage_sound_on_death) {
            AudioSource source = current_damage_sounds.GetComponent<AudioSource>();
            source.Stop();
        } 
        ISFXSounds sounds = GetDeathSound();
        SFXSystem.inst.PlaySound(sounds, transform.position);
    }

    private ISFXSounds GetDamageSound() {
        if (target_character.is_player) {
            return damage_grunts_player;
        } else if (target_character.is_enemy) {
            return damage_grunts_enemy;
        } else if (target_character.is_civilian) {
            return damage_grunts_npc;
        } else {
            Debug.LogWarning($"{gameObject.name} took damage, but is not a player, enemy, or civilian!!");
            return null;
        }
    }
    private ISFXSounds GetDeathSound() {
        if (target_character.is_player) {
            return death_grunts_player;
        } else if (target_character.is_enemy) {
            return death_grunts_enemy;
        } else if (target_character.is_civilian) {
            return death_grunts_npc;
        } else {
            Debug.LogWarning($"{gameObject.name} died, but is not a player, enemy, or civilian!!");
            return null;
        }
    }

}
