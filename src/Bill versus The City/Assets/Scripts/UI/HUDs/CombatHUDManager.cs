using UnityEngine;
using UnityEngine.UIElements;

public class CombatHUDManager : MonoBehaviour
{
    // this class provides a singleton reference to the combat HUD, so initializer scripts can avoid re-instantiating the UI on level loads.
    public static CombatHUDManager inst { get; private set; }

    private LevelVictoryConditions _victory_type = LevelVictoryConditions.none;
    public LevelVictoryConditions victory_type {
        get { return _victory_type; }
        set {
            bool update_victory_type = _victory_type != value;
            _victory_type = value;
            if (update_victory_type) {
                UpdateVictoryHUD();
            }
        }
    }

    public UIDocument ui_doc;
    public TimerUIController timer_ui;

    private static int _count = 0;

    void Awake() {
        inst = this;
    }

    void Start() {
        inst = this;
        gameObject.name += $" {++_count}";
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
        _UpdateVictoryHUD();
    }

    private Label objective_label;
    public void UpdateVictoryHUD() {
        objective_label = ui_doc.rootVisualElement.Q<Label>("ObjectiveLabel");
        _UpdateVictoryHUD();
    }
    private void _UpdateVictoryHUD() {
        // NOTE: private only update call, used in unity Update loop to update text without the DOM lookup to set the a Label object.
        objective_label.text = GetVictoryDisplayString(_victory_type);
        Debug.LogWarning($"set objective text: objective {_victory_type}, text '{objective_label.text}'"); 
    }
    
    public static string GetVictoryDisplayString(LevelVictoryConditions condition_type) {
        switch (condition_type) {
            case LevelVictoryConditions.clear_enemies:
                if (EnemiesManager.inst.remaining_enemies == 0) {
                    return "Escape to your truck!";
                }
                return $"Defeat all enemies {GetEnemyCountString()}";
                
            case LevelVictoryConditions.escape_to_truck:
                return "Escape to your truck!";

            case LevelVictoryConditions.survive_countdown:
                return "Survive!";

            case LevelVictoryConditions.none:
                return "<null>";

            default:
                Debug.LogWarning($"unknown level victory condition {condition_type}");
                return null;
        }
    }

    public static string GetEnemyCountString() {
        return $"{EnemiesManager.inst.remaining_enemies} / {EnemiesManager.inst.total_enemies}";
    }

    public void HideCountdownHUD() {
        VisualElement countdown = ui_doc.rootVisualElement.Q<VisualElement>("CountdownWrapper");
        countdown.style.visibility = Visibility.Hidden;
    }

    public void ConfigureCountdown(ITimer timer, Color color) {
        VisualElement countdown = ui_doc.rootVisualElement.Q<VisualElement>("CountdownWrapper");
        countdown.style.visibility = Visibility.Visible;
        timer_ui.text_color = color;
        timer_ui.AttachTimer(timer);
    }

    // private void ClearVictoryHUD() {
    //     if (current_victory != null) {
    //         current_victory.transform.parent = null;
    //         Destroy(current_victory);
    //         current_victory = null;
    //     }
    // }

    // private void CreateVictoryHUD() {
    //     ClearVictoryHUD();
    //     GameObject prefab = GetPrefab(victory_type);
    //     if (prefab == null) {
    //         return; // do nothing, no HUD
    //     }
    //     GameObject new_hud = Instantiate(prefab);
    //     new_hud.transform.parent = transform;
    //     current_victory = new_hud;
    // }
    
    // private GameObject GetPrefab(LevelVictoryConditions condition) {
    //     switch (condition) {
    //         case LevelVictoryConditions.clear_enemies:
    //             return kill_enemies_objective_prefab;
                
    //         case LevelVictoryConditions.escape_to_truck:
    //             return escape_to_truck_objective_prefab;

    //         case LevelVictoryConditions.survive_countdown:
    //             return survive_objective_prefab;

    //         case LevelVictoryConditions.none:
    //             return null;

    //         default:
    //             Debug.LogWarning($"unknown level victory condition {condition}");
    //             return null;
    //     }
    // }
}
