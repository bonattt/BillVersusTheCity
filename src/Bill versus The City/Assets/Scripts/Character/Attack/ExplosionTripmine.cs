using UnityEngine;

public class ExplosionTripmine : MonoBehaviour {
    public Explosion attack;

    void OnTriggerEnter(Collider other) {
        attack.Explode();
    }
}