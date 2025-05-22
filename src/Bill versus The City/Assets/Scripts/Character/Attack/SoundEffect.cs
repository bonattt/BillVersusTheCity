using System;
using UnityEngine;

public class SoundEffect : IAttackHitEffect, 
        IAttackShootEffect, IAttackMissEffect, IWeaponEffect, IFirearmEffect {
    
    // private string sound_path;
    protected ISFXSounds default_sound;
    public Vector3 offset = new Vector3(0f, 0f, 0f);

    public SoundEffect(string sound_path) {
        this.default_sound = SFXLibrary.LoadSound(sound_path);
    }
    public void DisplayDamageEffect(GameObject hit_target,
            Vector3 hit_location, IAttack attack) {
        PlaySound(hit_location, attack.weapon);
    }

    public void DisplayWeaponEffect(Vector3 point, IWeapon weapon) {
        PlaySound(point, weapon);
    }

    public void DisplayFirearmEffect(Vector3 point, IFirearm weapon) {
        PlaySound(point, weapon);
    }

    public void DisplayEffect(Vector3 hit_location, IAttack attack) {
        PlaySound(hit_location, attack.weapon);
    }

    protected virtual string GetAttackSoundPath(IWeapon attack) {
        // gets the NON default sound from the attack. 
        // Returns null if the attack doesn't have appropriate sounds
        return null;
    }

    protected virtual ISFXSounds GetSound(IWeapon weapon) {
        string sound_path = GetAttackSoundPath(weapon);
        // ScriptableObjects make it hard to set a variable to null
        if (sound_path == null || sound_path == "") { 
            return this.default_sound;
        }
        else {
            return SFXLibrary.LoadSound(sound_path);
        }
    }

    public void PlaySound(Vector3 point, IWeapon weapon) {
        SFXSystem.inst.PlaySound(GetSound(weapon), point);
    }
}

public class GunshotSoundEffect : SoundEffect
{

    public GunshotSoundEffect() : base(AttackResolver.DEFAULT_GUNSHOT) { /* do nothing */ }

    protected override string GetAttackSoundPath(IWeapon weapon)
    {
        return weapon.attack_sound;
    }
}

public class MeleeAttackSoundEffect : SoundEffect
{

    public MeleeAttackSoundEffect() : base(AttackResolver.DEFAULT_MELEE_SOUND) { /* do nothing */ }

    protected override string GetAttackSoundPath(IWeapon weapon)
    {
        return weapon.attack_sound;
    }
}


public class ReloadStartSoundEffect : SoundEffect
{

    public ReloadStartSoundEffect() : base(ReloadSounds.RELOAD_START_SOUND_PATH) { /* do nothing */ }

    protected override string GetAttackSoundPath(IWeapon weapon)
    {
        try
        {
            return ((IFirearm)weapon).reload_start_sound;
        }
        catch (InvalidCastException)
        {
            Debug.LogError($"{this.GetType()} was given a non-firearm weapon: {weapon}");
            return null;
        }

    }
}

public class ReloadCompleteSoundEffect : SoundEffect {

    public ReloadCompleteSoundEffect() : base (ReloadSounds.RELOAD_COMPLETE_SOUND_PATH) { /* do nothing */ }

    protected override string GetAttackSoundPath(IWeapon weapon) {
        try {
            return ((IFirearm) weapon).reload_complete_sound;
        }
        catch (InvalidCastException)
        {
            Debug.LogError($"{this.GetType()} was given a non-firearm weapon: {weapon}");
            return null;
        }
    }
}

public class EmptyGunshotSoundEffect : SoundEffect {

    public EmptyGunshotSoundEffect() : base (AttackResolver.EMPTY_GUNSHOT_SOUND_PATH) { /* do nothing */ }

    protected override string GetAttackSoundPath(IWeapon weapon) {
        try {
            return ((IFirearm) weapon).empty_gunshot_sound;
        }
        catch (InvalidCastException)
        {
            Debug.LogError($"{this.GetType()} was given a non-firearm weapon: {weapon}");
            return null;
        }
    }
}