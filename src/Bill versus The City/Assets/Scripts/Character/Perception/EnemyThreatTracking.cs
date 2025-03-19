using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThreatTracking : MonoBehaviour
{
    [SerializeField]
    [Tooltip("displays how suppressed the character is this frame. Suppression rises when bullets are near the character, and goes down when no bullets are nearby.")]
    private float suppression = 0f;

    [Tooltip("Debug field to force `is_suppressed` to always be true")]
    public bool always_suppressed = false;

    [Tooltip("distance at which a bullet is considered 'nearby'")]
    public float distance_threshold = 5f;

    public float suppression_rate = 0.1f;
    public float suppression_recovery_rate = 0.2f;

    [SerializeField]
    private bool _is_suppressed = false;
    public bool is_suppressed { get { return _is_suppressed || always_suppressed; }}

    [SerializeField]
    [Tooltip("tracks the number of bullets near this character on the current frame before updating supression")]
    private int bullets_near = 0;
    private IAttackTarget attacker;

    public bool draw_debug_lines = true;

    // // Start is called before the first frame update
    void Start()
    {
        attacker = GetComponent<IAttackTarget>();
        if (attacker == null) { Debug.LogError($"{gameObject.name}.EnemyThreatTracking.attacker is null!!"); }
    }

    // Update is called once per frame
    void Update()
    {   
        UpdateBulletsNear();
        UpdateSuppression();
    }

    protected void UpdateBulletsNear() {
        bullets_near = 0;
        foreach (IBullet bullet_near in BulletTracking.inst.NearbyPlayerBullets(transform.position, distance_threshold)) {
            if (bullet_near.attacker == attacker) {
                Debug.Log("skip bullet shot by myself!"); // TODO --- remove debug
                continue;
            }
            bullets_near += 1;
            Debug.DrawLine(transform.position, bullet_near.location.position, Color.magenta);
        }
    }

    protected void UpdateSuppression() {
        if (bullets_near == 0) {
            Debug.Log("no bullets near!"); // TODO --- remove debug
            ReduceSuppresion();
        }
        else {
            Debug.Log($"increase suppression {bullets_near}"); // TODO --- remove debug
            IncreaseSuppression();
        }
    }

    private void ReduceSuppresion() {
        if (suppression == 0) { return; } // avoid calling UpdateRecoveredFromSuppression if it's already been called
        suppression -= suppression_recovery_rate * Time.deltaTime;
        if (suppression <= 0) {
            suppression = 0f;
            UpdateRecoveredFromSuppression();
        }
    }

    private void IncreaseSuppression() {
        if (suppression >= 1f) { return; } // avoids calling UpdateBecomeSuppressed if it's already been called
        suppression += suppression_rate * Time.deltaTime;
        if (suppression >= 1f) {
            suppression = 1f;
            UpdateBecomeSuppressed();
        }
    }

    public void UpdateRecoveredFromSuppression() {
        // TODO --- not sure if I will have a subscriber
        _is_suppressed = false;
    }

    public void UpdateBecomeSuppressed() {
        // TODO --- not sure if I will have a subscriber
        _is_suppressed = true;
    }

}


public struct TrackedThreat {
    public ThreatType type;
    public float time;
    public Vector3 location;
}


public enum ThreatType {
    death,
    bullet
}
