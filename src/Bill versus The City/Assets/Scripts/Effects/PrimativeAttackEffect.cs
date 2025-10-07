// Controller on the prefab gun-shot effect built with unity primatives

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimativeAttackEffect : MonoBehaviour, IExplosionEffect {
    public float start_scale = 0.001f;
    public float max_scale = 0.1f;
    public float scale_rate = 1;

    void Start() {
        transform.localScale = new Vector3(start_scale, start_scale, start_scale);
    }

    void Update() {
        float frame_scale = scale_rate * Time.deltaTime;
        Vector3 scale_vector = new Vector3(frame_scale, frame_scale, frame_scale);
        transform.localScale += scale_vector;
        if (transform.localScale.x >= max_scale) {
            Destroy(this.gameObject);
        }

    }

    public void ConfigureFromExplosion(Explosion explosion) {
        // Debug.LogWarning($"increase scale_rate ({explosion.explosion_attack.explosion_radius} / {max_scale}) => {explosion.explosion_attack.explosion_radius / max_scale}"); // TODO --- remove debug
        scale_rate = scale_rate * (explosion.explosion_attack.explosion_radius / max_scale);
        // Debug.LogWarning($"scale_rate: {scale_rate}"); // TODO --- remove debug
        // scale_rate = scale_rate * scale_rate; // 
        max_scale = explosion.explosion_attack.explosion_radius * 2f; // scale is a diameter, so x2
    }
}
