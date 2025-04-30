using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CrouchHUDController : MonoBehaviour, IPlayerObserver
{
    public UIDocument ui_doc; 
    private Label label;
    private VisualElement crouch_hud;
    private PlayerCombat player_combat;
    // Start is called before the first frame update
    void Start()
    {
        crouch_hud = ui_doc.rootVisualElement.Q<VisualElement>("CrouchingHUD");
        player_combat = PlayerCharacter.inst.GetPlayerCombat(this);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHUD();
    }

    private void UpdateHUD() {
        if (this.player_combat.movement.crouch_percent != 0) {
            crouch_hud.style.visibility = Visibility.Visible;
        } else {
            crouch_hud.style.visibility = Visibility.Hidden;
        }
        
    }

    public void NewPlayerObject(PlayerCombat player_combat) {
        this.player_combat = player_combat;
    }
}
