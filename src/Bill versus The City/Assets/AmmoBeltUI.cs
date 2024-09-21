using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class AmmoBeltUI : MonoBehaviour, IGenericObserver
{
    public UIDocument ui_doc;
    private VisualElement element_list;
    public AmmoContainer ammo_container;

    // guarantees a consisten order to which fields are displayed
    public AmmoType[] display_order = new AmmoType[]{AmmoType.handgun, AmmoType.rifle, AmmoType.shotgun};

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
        for (int i = 0; i < display_order.Length; i++) {
            AmmoType type = display_order[i];
            if (! ammo_container.HasAmmoType(type)) {
                continue;
            }
            Label label = new Label($"{AmmoTypeDisplay.DisplayValue(type)}: {ammo_container.GetCount(type)} / {ammo_container.GetMax(type)}");
            label.AddToClassList("ammo_type");
            element_list.Add(label);
        }
    }
}
