using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PlayerHealthUIController : MonoBehaviour, ICharStatusSubscriber
{

    public CharacterStatus target_status;
    public Slider armor_slider, health_slider;
    public TMP_Text armor_text, health_text; 

    // Start is called before the first frame update
    void Start()
    {
        target_status.Subscribe(this);
        UpdateUI();
    }

    public void UpdateUI() {
        UpdateUI(target_status);
    }

    public void UpdateUI(ICharacterStatus status) {
        health_slider.minValue = 0;
        health_slider.maxValue = status.max_health;
        health_slider.value = status.health;
        health_text.text = $"{(int) status.health} / {(int) status.max_health}";

        if (status.armor == null || status.armor.armor_durability <= 0) {
            armor_slider.minValue = 0;
            armor_slider.maxValue = 1;
            armor_slider.value = 0;
            armor_text.text = "";
        } else {
            armor_slider.minValue = 0;
            armor_slider.maxValue = status.armor.armor_max_durability;
            armor_slider.value = status.armor.armor_durability;
            armor_text.text = $"{(int) status.armor.armor_durability} / {(int) status.armor.armor_max_durability}";
        }
    }

    public void StatusUpdated(ICharacterStatus status) {
        UpdateUI(status);
    }
    
}
