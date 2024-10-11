using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharCtrl
{
    public static PlayerMovement inst {
        get; private set;
    }

    // returns a list of all the nodes which enemies raycast to see the player
    public Transform vision_target;
    private List<Transform> _vision_nodes = null;
    public List<Transform> vision_nodes {
        get {
            if (_vision_nodes == null) {
                _vision_nodes = new List<Transform>{transform};
                foreach (Transform child in vision_target) {
                    _vision_nodes.Add(child);
                }
            }
            return _vision_nodes;
        }
    }

    public override void SetupCharacter() {
        // called from `CharCtrl.Start()`
        base.SetupCharacter();
        ((CharacterStatus) char_status).is_player = true;
        if (inst != null) {
            Debug.LogWarning("updating Player singleton!");
        }
        SetAsInstance();
    }

    void Awake() {
        // called in Awake to initialize before other scripts Start's.
        // called again in Start so new Player objects can replace the old.
        SetAsInstance();
    }

    private void SetAsInstance() {
        if(inst != null && inst != this) {
            Debug.LogWarning("overwriting current player");
        }
        inst = this;
        if (PlayerObservable.inst != null) {
            // if we run into this loading-order issue, the scripts will still initialize themselves off PlayerMovement.inst in Start
            PlayerObservable.inst.PlayerUpdated(this);
        }
    }

    public override bool AttackInput() {
        if (attack_controller.current_weapon.auto_fire) {
            return InputSystem.current.AttackHoldInput();
        }
        return InputSystem.current.AttackClickInput();
    }

    public override bool ReloadInput() {
        if (this.reloading) {
            return false;
        }
        return InputSystem.current.ReloadInput();
    }

    public override bool SprintInput() {
        return InputSystem.current.SprintInput();
    }

    public override bool CancelReloadInput() {
        if (! this.reloading) {
            return false;
        }
        return InputSystem.current.ReloadInput() || InputSystem.current.AttackClickInput();
    }
    
    public override bool AimInput() {
        return InputSystem.current.AimAttackInput();
    }

    public override bool CancelAimInput() {
        if (this.aiming) {
            return !AimInput();
        }
        return false;
    }
    
    protected override void Move() {
        LookWithAction();
        Vector3 move = MoveVector();
        move = ModifyMoveVector(move);
        controller.SimpleMove(move);
    }

    protected override void PostUpdate() {
        if (InputSystem.current.NextWeaponModeInput()) {
            attack_controller.current_weapon.NextWeaponSetting();
            if (attack_controller.current_weapon.HasWeaponSettings()) {
                ISoundSet sound = SFXLibrary.LoadSound("weapon_mode_switch");
                SFXSystem.instance.PlaySound(sound, transform.position);

            }
            attack_controller.UpdateSubscribers();
        }
    }

    protected Vector3 ModifyMoveVector(Vector3 move) {
        return move * this.movement_speed;
    }

    public override Vector3 LookTarget() {
        Vector3 mouse_pos = InputSystem.current.MouseWorldPosition();
        Vector3 look_target = new Vector3(mouse_pos.x, transform.position.y, mouse_pos.z);
        return look_target;
    }
    
    public override Vector3 MoveVector() {
        // float move_x, move_y;
        float move_x = InputSystem.current.MoveXInput();
        float move_y = InputSystem.current.MoveYInput();
        if (move_x != 0 && move_y != 0) {
            // remove crabwalk
            move_x = move_x / 1.4f;
            move_y = move_y / 1.4f;
        }
        Vector3 move_vector = new Vector3(move_x, 0, move_y);

        return move_vector;
    }

    
    protected override void CharacterDeath() {
        MenuManager.inst.PlayerDefeatPopup();
    }
}
