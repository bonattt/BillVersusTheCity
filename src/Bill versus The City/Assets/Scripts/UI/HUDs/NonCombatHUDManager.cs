using UnityEngine;
using UnityEngine.UIElements;

public class NonCombatHUDManager : MonoBehaviour
{
    // this class provides a singleton reference to the combat HUD, so initializer scripts can avoid re-instantiating the UI on level loads.
    public static NonCombatHUDManager inst { get; private set; }

    public UIDocument ui_doc;

    private static int _count = 0;

    void Awake() {
        inst = this;
    }

    void Start() {
        inst = this;
        gameObject.name += $" {++_count}";
        objective_label = ui_doc.rootVisualElement.Q<Label>("ObjectiveLabel");
        ConfigureForNonCombat();
        UpdateVictoryHUD();
    }

    private Label objective_label;
    private void ConfigureForNonCombat() {
        // hide elements of the HUD not used in non-combat 
        HideCountdownHUD();
        ui_doc.rootVisualElement.Q<VisualElement>("AmmoBeltContents").style.visibility =  Visibility.Hidden;
        ui_doc.rootVisualElement.Q<VisualElement>("EquipmentSlots").style.visibility =  Visibility.Hidden;
        ui_doc.rootVisualElement.Q<VisualElement>("EquippedWeapon").style.visibility =  Visibility.Hidden;
    }

    void OnDestroy() {
        // this is here mainly to avoid loading order issues i cannot articulate, but think might exist.
        if (inst == this) {
            inst = null;
        } else {
            // Debug.LogWarning("new Combat HUD instantiated before cleaning up the old HUD!"); // TODO --- remove debug
        }
    }
    
    void Update() {
        UpdateVictoryHUD();
    }

    public void UpdateVictoryHUD() {
        objective_label.text = "Use your truck to start the next level.";
    }
    
    public void HideCountdownHUD() {
        VisualElement countdown = ui_doc.rootVisualElement.Q<VisualElement>("CountdownWrapper");
        countdown.style.visibility = Visibility.Hidden;
    }
}
