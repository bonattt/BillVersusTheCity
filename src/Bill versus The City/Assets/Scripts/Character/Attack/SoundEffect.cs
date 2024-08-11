using UnityEngine;

public class SoundEffect : IAttackHitEffect, 
        IAttackShootEffect, IAttackMissEffect {
    
    private string sound_path;
    private ISoundSet sound;
    public Vector3 offset = new Vector3(0f, 0f, 0f);

    public SoundEffect(string sound_path) {
        this.sound = SFXLibrary.LoadSound(sound_path);
    }
    
    public void DisplayDamageEffect(GameObject hit_target,
            Vector3 hit_location, float damage) {
        PlaySound(hit_location);
    }

    public void DisplayEffect(Vector3 hit_location) {
        PlaySound(hit_location);
    }

    public void PlaySound(Vector3 point) {
        SFXSystem.instance.PlaySound(sound, point);
    }
}