using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadSounds : MonoBehaviour, IReloadSubscriber
{
    public string reload_start_sound_path = "Prototype/start-reload-chiptone";
    public string reload_complete_sound_path = "Prototype/complete-reload-chiptone";
    private ISound reload_start_sound, reload_complete_sound;
    private IReloadManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<IReloadManager>();
        manager.Subscribe(this);
        reload_start_sound = SFXLibrary.LoadSound(reload_start_sound_path);
        if (reload_start_sound == null) {
            Debug.LogWarning($"Reload Start Sound null for {gameObject}");
        }
        reload_complete_sound = SFXLibrary.LoadSound(reload_complete_sound_path);
        if (reload_complete_sound == null) {
            Debug.LogWarning($"Reload Complete Sound null for {gameObject}");
        }
    }
    
    void OnDestroy() {
        manager.Unsubscribe(this);
    }

    
    public void StartReload(IReloadManager manager, IWeapon weapon) {
        SFXSystem.instance.PlaySound(reload_start_sound, transform.position);
    }

    public void FinishReload(IReloadManager manager, IWeapon weapon) {
        SFXSystem.instance.PlaySound(reload_complete_sound, transform.position);
    }

    public void CancelReload(IReloadManager manager, IWeapon weapon) {
        // do nothing
    }

}
