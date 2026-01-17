

using System;
using UnityEngine;

public class GunRangeTarget : MonoBehaviour, IAttackTarget, ICharacterStatus {
    /**
      *  A mostly empty class implementing IAttackTarget to allow attack hit effects to trigger on gun-range targets
      */
    ////////////////////////////////
    /// implements IAttackTarget ///
    ////////////////////////////////
    public bool is_player { get => false; }
    public ICharacterStatus GetStatus() { return this; } // TODO --- remove this, make ICharacterStatus extend this interface instead. 
    public GameObject GetHitTarget() { return gameObject; }
    public Transform GetAimTarget() { return transform; }
    public void OnAttackHitRecieved(IAttack attack) {
        // if damage numbers are turned off, show them anyways
        if (!GameSettings.inst.debug_settings.GetBool("show_damage_numbers")) {
            IAttackHitEffect health_damage_numbers = new SpawnDamageNumberEffect();
            health_damage_numbers.DisplayDamageEffect(GetHitTarget(), GetHitTarget().transform.position, attack);
        }

    }
    public void OnAttackHitDealt(IAttack attack, IAttackTarget target) { /* do nothing */ }
    public void FlashBangHit(float intensity) { /* do nothing */ }
    
    ///////////////////////////////////
    /// implements ICharacterStatus ///
    ///////////////////////////////////
    
    public float health {
        get => 100f;
        set { /* do nothing */ }
    }
    public float max_health {
        get => 100f;
        set { /* do nothing */ }
    }
    public bool adjusting_difficulty { get => false; } // flag set to true while values are adjusted for difficulty, to avoid triggering on-damage effects when a character's health is adjusted for difficulty level
    public IArmor armor { get => null; }

    public void ApplyNewArmor(IArmor armor_template) { throw new NotImplementedException(); }
    public void ApplyNewArmor(ScriptableObject armor_template) { throw new NotImplementedException(); }
    public void ApplyExistingArmor(IArmor existing_armor) { throw new NotImplementedException(); }
    public void RemoveArmor() { throw new NotImplementedException(); }

    public void Subscribe(ICharStatusSubscriber sub)  { throw new NotImplementedException(); }
    public void Unsubscribe(ICharStatusSubscriber sub)  { throw new NotImplementedException(); }
    public void UpdateStatus()  { throw new NotImplementedException(); }

}