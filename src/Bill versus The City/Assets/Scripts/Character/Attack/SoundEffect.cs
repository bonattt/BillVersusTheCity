using UnityEngine;

public class SoundEffect : IAttackHitEffect, 
        IAttackShootEffect, IAttackMissEffect {
    
    // private string sound_path;
    protected ISoundSet default_sound;
    public Vector3 offset = new Vector3(0f, 0f, 0f);

    public SoundEffect(string sound_path) {
        this.default_sound = SFXLibrary.LoadSound(sound_path);
    }
    public void DisplayDamageEffect(GameObject hit_target,
            Vector3 hit_location, IAttack attack) {
        PlaySound(hit_location, attack);
    }

    public void DisplayEffect(Vector3 hit_location, IAttack attack) {
        PlaySound(hit_location, attack);
    }

    protected virtual string GetAttackSoundPath(IAttack attack) {
        // gets the NON default sound from the attack. 
        // Returns null if the attack doesn't have appropriate sounds
        return null;
    }

    protected virtual ISoundSet GetSound(IAttack attack) {
        string sound_path = GetAttackSoundPath(attack);
        // ScriptableObjects make it hard to set a variable to null
        if (sound_path == null || sound_path == "") { 
            return this.default_sound;
        }
        else {
            return SFXLibrary.LoadSound(sound_path);
        }
    }

    public void PlaySound(Vector3 point, IAttack attack) {
        SFXSystem.instance.PlaySound(GetSound(attack), point);
    }
}

public class GunshotSoundEffect : SoundEffect {

    public GunshotSoundEffect() : base (AttackResolver.DEFAULT_GUNSHOT) {

    }

    protected override string GetAttackSoundPath(IAttack attack) {
        return attack.weapon.gunshot_sound;
    }
}