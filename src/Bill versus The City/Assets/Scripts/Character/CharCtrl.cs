using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterActionKey {
    attack,
    sprint,
}

public abstract class CharCtrl : MonoBehaviour, ICharStatusSubscriber, IAttackTarget
{
    protected CharacterController controller;
    protected AttackController attack_controller;
    protected ICharacterStatus char_status;

    public float last_attack_time { get; private set; }
    public float rotation_degrees_per_second = 400;
    public float rotation_speed = 0.85f;
    

    void Start() {
        SetupCharacter(); 
    }
    public virtual void SetupCharacter() {
        SetupCharacter(new CharacterStatus());
    }

    public virtual void SetupCharacter(ICharacterStatus status) {
        char_status = status;
        char_status.Subscribe(this);
        controller = GetComponent<CharacterController>();
        attack_controller = GetComponent<AttackController>();
    }

    // Update is called once per frame
    void Update()
    {   
        Move();
        TryToAttack();
    }

    private void Move() {
        MoveNormal();
        // else {
        //     MoveWithAction();
        // }
    }

    private void TryToAttack() {
        if (AttackInput() && CanAttack()) {
            PerformAttack();
        }
    }

    protected virtual void PerformAttack() {
        last_attack_time = Time.time;
        attack_controller.FireAttack(ShootVector());
    }
    
    // public void SetMovementAction(IMovementAction action) {
    //     this.movement_action = action;
    //     action.new_this_frame = true;
    //     action.remaining_duration = action.action_duration;
    // }

    private void MoveNormal() {
        LookWithAction();
        Vector3 dir = MoveVector();
        controller.SimpleMove(dir);
    }

    // private void MoveWithAction() {
    //     LookWithAction();
    //     movement_action.action_used_for += Time.deltaTime;
    //     Vector3 dir;
    //     if (movement_action.overrides_move_vector) {
    //         dir = movement_action.move_vector;   
    //     } else {
    //         dir = MoveVector();
    //     }
    //     dir *= movement_action.move_speed_multiplier;

    //     controller.SimpleMove(dir);
    // }

    // public bool MoveActionNameMatch(string name) {
    //     return movement_action != null 
    //         && movement_action.action_name.Equals(name);
    // }
    private void LookWithAction() {
        // Handles adjusting the player character's rotation with hooks to
        // allow a movement_action to override the value
        Vector3 forward; 
        // if (movement_action == null) {
        //     forward = LookVector();
        // }
        // else if (movement_action.overrides_look_vector) {
        //     forward = movement_action.look_vector;
        // } 
        // else if (movement_action.overrides_look_target) {
        //     forward = VectorFromLookTarget(movement_action.look_target);
        // }
        // else {
            forward = LookVector();
        // }
        LookRotate(forward);
    }

    // private void LookNormal() {
    //     Vector3 forward = LookVector();
    //     LookRotate(forward);
    // }

    private void LookRotate(Vector3 forward) {
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        Quaternion target_rot = Quaternion.AngleAxis(angle - 90, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, target_rot, rotation_speed);
    }
 
    protected virtual Vector3 LookVector() {
        // Returns a vector of the direction to look in
        return VectorFromLookTarget(LookTarget());
    }

    protected Vector3 VectorFromLookTarget(Vector3 look_target) {
        Vector3 v = look_target - transform.position;
        Vector3 forward = new Vector3(
            v.x, 0, v.z
        );
        return forward;
    }

    public abstract Vector3 LookTarget();  // Vector3 position to look at.

    public virtual Vector3 ShootVector() {
        return LookVector();
    }

    public virtual bool AttackInput() {
        // Indicates that a main attack should be made this frame.
        return false;
    }

    public virtual bool CanAttack() {
        // Indicates whether an attack can be started this frame;
        return true;
    }

    public abstract Vector3 MoveVector();

    public void OnAttackHitDealt(IAttack attack, IAttackTarget target) {
        // TODO
        Debug.Log($"{this} scored a hit with attack {attack}!");
    }

    public void OnAttackHitRecieved(IAttack attack) {
        // char_status.GetAttackTarget().OnAttackHitRecieved(attack, attacker);
        Debug.Log($"{this} was hit by an attack {attack}!");
    }     

    public void StatusUpdated(ICharacterStatus status) {
        if (char_status.health <= 0) {
            CharacterDeath();
        }
    }

    protected virtual void CharacterDeath() {
        Debug.LogWarning($"Character '{gameObject.name}' has died!");
        // TODO ---
    }

    public ICharacterStatus GetStatus() {
        return this.char_status;
    }

    public GameObject GetHitTarget() {
        return this.gameObject;
    }
}
