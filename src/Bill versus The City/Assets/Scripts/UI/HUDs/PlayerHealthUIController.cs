using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;

public class PlayerHealthUIController : MonoBehaviour, ICharStatusSubscriber, IPlayerObserver
{

    private CharacterStatus _status = null;
    public CharacterStatus target_status { 
        get {
            if (_status == null) {
                PlayerCombat player = PlayerCharacter.inst.GetPlayerCombat(this);
                NewPlayerObject(player);  // as long as player != null, _status will be set
            }
            return _status;
        }
    }
    public Slider armor_slider, health_slider;
    public TMP_Text armor_text, health_text; 

    // Start is called before the first frame update
    void Start()
    {
        target_status.Subscribe(this);
        UpdateUI();
    }
    
    public void NewPlayerObject(PlayerCombat player) {
        if (_status != null) {
            _status.Unsubscribe(this);
        }
        if (player == null) {
            Debug.LogWarning("new player is null!");
            return;
        }
        _status = player.status;
        if (_status != null) {
            _status.Subscribe(this);
            UpdateUI();
        }
    }

    public void UpdateUI() {
        UpdateUI(target_status);
    }

    public void UpdateUI(ICharacterStatus status) {
        health_slider.minValue = 0;
        health_slider.maxValue = status.max_health;
        health_slider.value = status.health;
        health_text.text = $"{GetDisplayNumber(status.health)} / {(int) status.max_health}";

        if (status.armor == null || status.armor.armor_durability <= 0) {
            armor_slider.minValue = 0;
            armor_slider.maxValue = 1;
            armor_slider.value = 0;
            armor_text.text = "";
        } else {
            armor_slider.minValue = 0;
            armor_slider.maxValue = status.armor.armor_max_durability;
            armor_slider.value = status.armor.armor_durability;
            armor_text.text = $"{GetDisplayNumber(status.armor.armor_durability)} / {(int) status.armor.armor_max_durability}";
        }
    }

    private int GetDisplayNumber(float value) {
        // takes a float, and returns a displayble int for that float
        if (value < 1f && value > 0f) {
            // normally just casting to an int and truncating is fine, but for values less than 1 greater than 0, 
            // it's weird to display "0 HP" while the player is not yet dead, so we return 1 in this case
            return 1;
        }
        return (int) value;
    }
    
    public void OnDamage(ICharacterStatus status) {
        // do nothing
    }
    public void OnHeal(ICharacterStatus status) {
        // do nothing
    }

    public void StatusUpdated(ICharacterStatus status) {
        UpdateUI(status);
    }
    
}
