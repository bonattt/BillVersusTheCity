using UnityEngine;

public class CombatHUDManager : MonoBehaviour
{
    // this class provides a singleton reference to the combat HUD, so initializer scripts can avoid re-instantiating the UI on level loads.
    public static CombatHUDManager inst { get; private set; }

    public GameObject kill_enemies_objective_prefab, escape_to_truck_objective_prefab, survive_objective_prefab;

    private GameObject current_victory;

    private LevelVictoryConditions _victory_type = LevelVictoryConditions.none;
    public LevelVictoryConditions victory_type {
        get { return _victory_type; }
        set {
            bool update_victory_type = _victory_type != value;
            _victory_type = value;

            if (update_victory_type) {
                CreateVictoryHUD();
            }
        }
    }

    void Awake() {
        inst = this;
    }

    private void ClearVictoryHUD() {
        if (current_victory != null) {
            current_victory.transform.parent = null;
            Destroy(current_victory);
            current_victory = null;
        }
    }

    private void CreateVictoryHUD() {
        ClearVictoryHUD();
        GameObject prefab = GetPrefab(victory_type);
        if (prefab == null) {
            return; // do nothing, no HUD
        }
        GameObject new_hud = Instantiate(prefab);
        new_hud.transform.parent = transform;
        current_victory = new_hud;
    }
    
    private GameObject GetPrefab(LevelVictoryConditions condition) {
        switch (condition) {
            case LevelVictoryConditions.clear_enemies:
                return kill_enemies_objective_prefab;
                
            case LevelVictoryConditions.escape_to_truck:
                return escape_to_truck_objective_prefab;

            case LevelVictoryConditions.survive_countdown:
                return survive_objective_prefab;

            case LevelVictoryConditions.none:
                return null;

            default:
                Debug.LogWarning($"unknown level victory condition {condition}");
                return null;
        }
    }
}
