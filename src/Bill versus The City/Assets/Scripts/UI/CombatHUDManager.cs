using UnityEngine;

public class CombatHUDManager : MonoBehaviour
{
    // this class provides a singleton reference to the combat HUD, so initializer scripts can avoid re-instantiating the UI on level loads.
    public static CombatHUDManager inst { get; private set; }

    void Awake() {
        inst = this;
    }
}
