using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class AmmoBeltUI : MonoBehaviour, IGenericObserver, IPlayerObserver
{
    public UIDocument ui_doc;
    private VisualElement element_list;
    private AmmoContainer ammo_container;

    void Start() {
        element_list = ui_doc.rootVisualElement.Q<VisualElement>("AmmoBeltContents"); 
        PlayerCharacter.inst.SubscribeToPlayer(this);
        NewPlayerObject(PlayerCharacter.inst.GetPlayerCombat(this));
    }

    void OnDestroy() {
        ammo_container.Unusubscribe(this);
        PlayerCharacter.inst.UnsubscribeFromPlayer(this);
    }

    public void UpdateObserver(IGenericObservable _) {
        // called on changes to the AmmoContainer 
        UpdateAmmo();
    }
    
    public void NewPlayerObject(PlayerCombat player) {
        // called if a new player object is created
        if (ammo_container != null) {
            ammo_container.Unusubscribe(this);
        }
        if (player == null) {
            Debug.LogWarning("new player is null!");
            return;
        }
        ammo_container = player.ammo;
        ammo_container.Subscribe(this);
        UpdateAmmo();
    }

    public void UpdateAmmo() {
        element_list.Clear();
        for (int i = 0; i < AmmoContainer.display_order.Length; i++) {
            AmmoType type = AmmoContainer.display_order[i];
            if (! ammo_container.HasAmmoType(type)) {
                continue;
            }
            Label label = new Label(ammo_container.GetTextDisplay(type));
            label.AddToClassList("ammo_type");
            element_list.Add(label);
        }
    }
}
