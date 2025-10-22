using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CrouchHUDController : MonoBehaviour, IPlayerObserver
{
    public UIDocument ui_doc; 
    private Label label;
    private VisualElement is_crouching_hud;
    private VisualElement crouch_controls_element;
    private Label crouch_controls_label;

    private float lock_controls_hud_seconds = 0.10f;
    private float controls_hud_locked_until = -1f;
    private ActionCode previous_action_code = ActionCode.none;
    private PlayerCombat player_combat;
    // Start is called before the first frame update
    void Start()
    {
        is_crouching_hud = ui_doc.rootVisualElement.Q<VisualElement>("CrouchingHUD");
        crouch_controls_element = ui_doc.rootVisualElement.Q<VisualElement>("CrouchOrJumpIndicator");
        crouch_controls_label = crouch_controls_element.Q<Label>("ActionLabel");
        player_combat = PlayerCharacter.inst.GetPlayerCombat(this);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIsCrouchingHUD();
        UpdateControlsHUD();
    }

    private void UpdateIsCrouchingHUD() {
        if (this.player_combat.movement.crouch_percent != 0) {
            is_crouching_hud.style.visibility = Visibility.Visible;
        } else {
            is_crouching_hud.style.visibility = Visibility.Hidden;
        }

    }
    
    private void UpdateControlsHUD() {
        if (controls_hud_locked_until > Time.time) { return; } // UI locked, just return
        if (!player_combat.movement.combat_enabled) {
            crouch_controls_element.style.visibility = Visibility.Hidden;
            crouch_controls_label.text = "disabled";
            return;
        }
        ActionCode action_code = player_combat.movement.GetCrouchAction();
        if (action_code == previous_action_code) { return; } // no change, just return

        previous_action_code = action_code;
        controls_hud_locked_until = Time.time + lock_controls_hud_seconds;
        if (action_code == ActionCode.jump) {
            crouch_controls_element.style.visibility = Visibility.Visible;
            crouch_controls_label.text = "Jump";
        } else if (action_code == ActionCode.dive) {
            crouch_controls_element.style.visibility = Visibility.Visible;
            crouch_controls_label.text = "Dive";
            // } else if (action_code == ActionCode.crouch) {
            //     crouch_controls_element.style.visibility = Visibility.Hidden;
            //     crouch_controls_label.text = "crouch";
        } else {
            crouch_controls_element.style.visibility = Visibility.Hidden;
            crouch_controls_label.text = "n/a";
        }
        
    }

    public void NewPlayerObject(PlayerCombat player_combat) {
        this.player_combat = player_combat;
    }
}
