using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadSounds : MonoBehaviour, IReloadSubscriber
{
    public const string RELOAD_START_SOUND_PATH = "reload_start";
    public const string RELOAD_COMPLETE_SOUND_PATH = "reload_finish";
    public const string RELOAD_CANCEL_SOUND_PATH = "reload_cancel";
    // private ISoundSet reload_start_sound, reload_complete_sound;
    private static SoundEffect reload_start_effect, reload_complete_effect, reload_cancel_effect;
    private IReloadManager manager;

    private bool killed = false;

    void Awake() {
        // Resource.Load cannot be called in static class constructor
        reload_start_effect = new ReloadStartSoundEffect();
        reload_complete_effect = new ReloadCompleteSoundEffect();
        reload_cancel_effect = new SoundEffect(RELOAD_CANCEL_SOUND_PATH);
    }

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<IReloadManager>();
        manager.Subscribe(this);
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
        manager.Unsubscribe(this);
    }

    public void OnDeath(ICharacterStatus _)  {
        killed = true;
    }
    
    public void StartReload(IReloadManager manager, IWeapon weapon) {
        // SFXSystem.instance.PlaySound(reload_start_sound, transform.position);
        if (killed) { return; } // don't play sounds if enemy is killed while reloading
        reload_start_effect.DisplayWeaponEffect(transform.position, weapon);
    }

    public void ReloadFinished(IReloadManager manager, IWeapon weapon) {
        // SFXSystem.instance.PlaySound(reload_complete_sound, transform.position);
        if (killed) { return; } // don't play sounds if enemy is killed while reloading
        reload_complete_effect.DisplayWeaponEffect(transform.position, weapon);
    }

    public void ReloadCancelled(IReloadManager manager, IWeapon weapon) {
        if (killed) { return; } // don't play sounds if enemy is killed while reloading
        reload_cancel_effect.DisplayWeaponEffect(transform.position, weapon);
    }

}
