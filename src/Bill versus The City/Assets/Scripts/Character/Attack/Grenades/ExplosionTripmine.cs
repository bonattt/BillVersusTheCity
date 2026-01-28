using UnityEngine;

public class ExplosionTripmine : MonoBehaviour {
    public DamageExplosion attack;

    void OnTriggerEnter(Collider other) {
        attack.Explode();
    }
}