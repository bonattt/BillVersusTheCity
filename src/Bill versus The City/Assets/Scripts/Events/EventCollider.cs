

using UnityEngine;

public class EventCollider : MonoBehaviour {

    public GameObject event_effect;
    private IGameEventEffect _event_effect;
    public bool trigger_once = true;
    private bool has_triggered = false;

    void Start() {
        _event_effect = event_effect.GetComponent<IGameEventEffect>();
        if (_event_effect == null) {
            Debug.LogError($"IGameEventEffect is missing from {event_effect.name} for trigger {gameObject.name}");
        }
    }

    void OnCollisionEnter(Collision collision) {
        TryTriggerEffect(collision.gameObject);
    }

    void OnTriggerEnter(Collider other) {
        TryTriggerEffect(other.gameObject);
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        TryTriggerEffect(hit.gameObject);
    }

    public void TryTriggerEffect(GameObject obj) {
        if (ShouldTriggerEffect(obj)) {
            _event_effect.ActivateEffect();
            has_triggered = true;
        } else {
            Debug.LogWarning($"event collider did NOT trigger for {obj.name}"); // TODO --- remove debug
        }
    }

    public bool ShouldTriggerEffect(GameObject obj) {
        if (has_triggered && trigger_once) {
            return false;
        }
        CharCtrl ctrl = obj.GetComponentInParent<CharCtrl>();
        if (ctrl == null) { return false; }
        return ctrl.is_player;
    }

    public void Reset() {
        has_triggered = false;
    }

}