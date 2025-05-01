

using UnityEngine;
using UnityEngine.UIElements;

public class HealthBarController : MonoBehaviour, ICharStatusSubscriber, IPlayerObserver {


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

    public UIDocument ui_doc;
    private VisualElement health_bar_fill, armor_bar_fill;

    /// Interface ///////////////////
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
    
    public void OnDamage(ICharacterStatus status) {
        // do nothing
    }
    public void OnHeal(ICharacterStatus status) {
        // do nothing
    }
    /// END INTERFACES ////////////
    
    void Start()
    {
        health_bar_fill = ui_doc.rootVisualElement.Q<VisualElement>("HealthBarFill");
        armor_bar_fill = ui_doc.rootVisualElement.Q<VisualElement>("ArmorBarFill");
        target_status.Subscribe(this);
        UpdateUI();
    }

    public void StatusUpdated(ICharacterStatus status) {
        UpdateUI(status);
    }

    public void UpdateUI() {
        UpdateUI(target_status);
    }

    public void UpdateUI(ICharacterStatus status) {
        UpdateHealthbar(health_bar_fill, status.health, status.max_health);
        if (status.armor == null) {
            UpdateHealthbar(armor_bar_fill, 0f, 1f);
        } else {
            UpdateHealthbar(armor_bar_fill, status.armor.armor_durability, status.armor.armor_max_durability);
        }
    }

    public static void UpdateHealthbar(VisualElement bar_fill, float stat, float stat_max) {
        // NOTE: setting the width to 100% for 100% health overflows the fill past the background
        bar_fill.style.width = Length.Percent(97 * (stat / stat_max));
    }
    
}