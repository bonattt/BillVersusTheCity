using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class GrenadeFuse : MonoBehaviour {
    public Explosion explosion;
    public float fuse_length_seconds = 5f;
    public bool start_fuse_on_start = true;
    private float fuse_started_at = float.PositiveInfinity;

    public string fuse_sound_name = "WeaponEffects/GrenadeFuse";
    private ISFXSounds fuse_sound;
    private AudioSource sound_playing;

    public float fuse_progress {
        get {
            float progress = (Time.time - fuse_started_at) / fuse_length_seconds;
            return Mathf.Max(0f, Mathf.Min(progress, 1f));
        }
    }

    // Start is called before the first frame update
    void Start() {
        LoadSounds();
        if (start_fuse_on_start) {
            StartFuse();
        }
        explosion.destroy_on_explode = true;
    }

    private void LoadSounds() {
        fuse_sound = SFXLibrary.LoadSound(fuse_sound_name);
        if (fuse_sound == null) {
            Debug.LogError("failed to load fuse sound!"); 
        }
    }

    private void PlayFuseSound() {
        sound_playing = SFXSystem.inst.PlaySound(fuse_sound, transform.position);
        if (sound_playing == null) { Debug.LogError("playing sound returned null!"); return; }

        sound_playing.transform.parent = transform;
        sound_playing.spatialBlend = 1f;
        sound_playing.minDistance = 1f;
        sound_playing.maxDistance = explosion.explosion_attack.explosion_radius * 25f;
        Debug.LogWarning($"fuse sound radius {sound_playing.maxDistance}");
    }

    private void StopFuseSound() {
        if (sound_playing == null) {
            Debug.LogWarning("StopFuseSound called with no sound playing!");
            return;
        }
        sound_playing.Stop();
        Destroy(sound_playing);
        sound_playing = null;
    }

    public void StartFuse() {
        fuse_started_at = Time.time;
        PlayFuseSound();
    }

    // Update is called once per frame
    void Update() {
        if (fuse_started_at + fuse_length_seconds < Time.time) {
            explosion.Explode();
            StopFuseSound();
        }
        UpdateDebug();
    }

    //////////////////// DEBUG ////////////////////
    public float debug__fuse_progress = -1f;
    public void UpdateDebug() {
        debug__fuse_progress = fuse_progress;
    }
}
