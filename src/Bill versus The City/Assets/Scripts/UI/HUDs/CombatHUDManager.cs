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
    public LevelConfig level_config;

    private static int _count = 0;

    void Awake() {
        inst = this;
    }

    void Start() {
        inst = this;
        gameObject.name += $" {++_count}";
        UpdateVictoryHUD();
        UpdateCombatMode(force_update:true);
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
        UpdateCountdownUI();
        UpdateCombatMode();
    }

    private Label objective_label;
    public void UpdateVictoryHUD() {
        if (objective_label == null) {
            // NOTE: prevents loading order issues
            objective_label = ui_doc.rootVisualElement.Q<Label>("ObjectiveLabel");
        }
        if (level_config.has_objective_display_override) {
            objective_label.text = level_config.GetOverrideObjectiveDisplay();
        } else {
            objective_label.text = GetVictoryDisplayString(_victory_type);
        }
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

            case LevelVictoryConditions.hub_level:
                return "Head to your truck";

            default:
                Debug.LogWarning($"unknown level victory condition {condition_type}");
                return null;
        }
    }

    public static string GetEnemyCountString() {
        return $"{EnemiesManager.inst.remaining_enemies} / {EnemiesManager.inst.total_enemies}";
    }

    private bool _had_countdown = false; // cached `level_config.has_countdown` to only do work when it changes
    private bool _countdown_setup = false; // cached `level_config.has_countdown` to only do work when it changes
    public void UpdateCountdownUI() {
        if (level_config == null) { Debug.LogWarning("level config is null!"); return; } // TODO --- remove debug
        if (level_config.has_countdown != _had_countdown || !_countdown_setup) {
            if (level_config.has_countdown) {
                ConfigureCountdown();
            } else {
                HideCountdownHUD();
            }
            _countdown_setup = true;
        }
        _had_countdown = level_config.has_countdown;
    }

    public void HideCountdownHUD() {
        VisualElement countdown = ui_doc.rootVisualElement.Q<VisualElement>("CountdownWrapper");
        countdown.style.visibility = Visibility.Hidden;
    }

    public void ConfigureCountdown() {
        VisualElement countdown = ui_doc.rootVisualElement.Q<VisualElement>("CountdownWrapper");
        countdown.style.visibility = Visibility.Visible;
        timer_ui.text_color = GetCountdownColor(level_config);
        timer_ui.AttachTimer(level_config.countdown);
    }

    public static Color GetCountdownColor(LevelConfig level_config) {
        if (level_config.victory_conditions_preset == LevelVictoryConditions.survive_countdown) {
            return Color.green;
        } else if (level_config.failure_conditions_preset == LevelFailuerConditions.countdown) {
            return Color.red;
        } else {
            Debug.LogWarning($"unhandled level config conditions for countdown color. victory: {level_config.victory_conditions_preset}, failure: {level_config.failure_conditions_preset}");
            return Color.yellow;
        }
    }

    private bool _was_combat_enabled = false; // cached `level_config.combat_enabled` to only do work when it changes
    public void UpdateCombatMode(bool force_update=false) {
        if (level_config == null) {
            ConfigureForNonCombat();
            Debug.LogWarning("no level config, disabling combat HUD");
            return;
        }

        if (_was_combat_enabled != level_config.combat_enabled || force_update) {
            if (level_config.combat_enabled) {
                ConfigureForCombat();
            }
            else {
                ConfigureForNonCombat();
            }

        }
        _was_combat_enabled = level_config.combat_enabled;
    }
    
    private void ConfigureForNonCombat() {
        // hide elements of the HUD not used in non-combat 
        ui_doc.rootVisualElement.Q<VisualElement>("AmmoBeltContents").style.visibility =  Visibility.Hidden;
        ui_doc.rootVisualElement.Q<VisualElement>("EquipmentSlots").style.visibility =  Visibility.Hidden;
        ui_doc.rootVisualElement.Q<VisualElement>("EquippedWeapon").style.visibility =  Visibility.Hidden;
    }

    private void ConfigureForCombat() {
        // hide elements of the HUD not used in non-combat 
        ui_doc.rootVisualElement.Q<VisualElement>("AmmoBeltContents").style.visibility =  Visibility.Visible;
        ui_doc.rootVisualElement.Q<VisualElement>("EquipmentSlots").style.visibility =  Visibility.Visible;
        ui_doc.rootVisualElement.Q<VisualElement>("EquippedWeapon").style.visibility =  Visibility.Visible;
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
