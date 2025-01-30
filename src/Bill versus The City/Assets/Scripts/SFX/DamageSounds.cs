using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSounds : MonoBehaviour, ICharStatusSubscriber
{
    public const string DAMAGE_GRUNT_SOUND_PATH = "damage_grunt";
    // private ISoundSet reload_start_sound, reload_complete_sound;
    private static ISoundSet damage_grunts;
    private ICharacterStatus status;
    public CharCtrl target_character;

    private bool killed = false;
    public bool only_death_sounds = true;

    void Awake() {
        damage_grunts = SFXLibrary.LoadSound(DAMAGE_GRUNT_SOUND_PATH);
    }

    // Start is called before the first frame update
    void Start()
    {
        status = target_character.GetStatus();
        status.Subscribe(this);
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

    public void OnDeath(ICharacterStatus _)  {
        // TODO --- implement death sounds
        if (!killed) {
            killed = true;
            PlayDamageSound();
        }
    }
    
    private float last_health_Value = -1f;
    public void StatusUpdated(ICharacterStatus status) {
        if (!killed && !only_death_sounds && last_health_Value > status.health) {
            PlayDamageSound();
        }
        last_health_Value = status.health;
    }

    private void PlayDamageSound() {
        SFXSystem.inst.PlaySound(damage_grunts, transform.position);
    }

}
