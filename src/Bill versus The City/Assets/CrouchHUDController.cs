using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CrouchHUDController : MonoBehaviour, IPlayerObserver
{
    private UIDocument ui_doc; 
    private Label label;
    private PlayerCombat player_combat;
    // Start is called before the first frame update
    void Start()
    {
        ui_doc = GetComponent<UIDocument>();
        label = ui_doc.rootVisualElement.Q<Label>();
        player_combat = PlayerCharacter.inst.GetPlayerCombat(this);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLabel();
    }

    private void UpdateLabel() {
        if (this.player_combat.movement.crouch_percent != 0) {
            label.style.visibility = Visibility.Visible;
        } else {
            label.style.visibility = Visibility.Hidden;
        }
        
    }

    public void NewPlayerObject(PlayerCombat player_combat) {
        this.player_combat = player_combat;
        
    }
}
