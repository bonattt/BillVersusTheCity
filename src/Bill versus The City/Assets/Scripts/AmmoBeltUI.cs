using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class AmmoBeltUI : MonoBehaviour, IGenericObserver
{
    public UIDocument ui_doc;
    private VisualElement element_list;
    public AmmoContainer ammo_container;

    void Start() {
        element_list = ui_doc.rootVisualElement.Q<VisualElement>("contents"); 
        ammo_container.Subscribe(this);
        UpdateAmmo();
    }

    public void UpdateObserver(IGenericObservable _) {
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
