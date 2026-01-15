

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHearingManager : IGenericObserver {
    private static EnemyHearingManager _inst;
    public static EnemyHearingManager inst {
        get {
            if (_inst == null) {
                _inst = new EnemyHearingManager();
            }
            return _inst;
        }
    }

    private List<EnemyPerception> perception;

    public EnemyHearingManager() {
        EnemiesManager.inst.Subscribe(this);
        UpdateObserver(EnemiesManager.inst);
    }

    public void UpdateObserver(IGenericObservable _) {
        perception = new List<EnemyPerception>();
        foreach (NavMeshAgentMovement enemy in EnemiesManager.inst.GetRemainingEnemies()) {
            EnemyPerception new_perception = enemy.GetComponent<EnemyPerception>();
            if (new_perception == null) {
                Debug.LogError("Enemy without perception!!");
                continue;
            }
            perception.Add(new_perception);
        }
    }

    private List<IGlobalSoundsObserver> subscribers = new List<IGlobalSoundsObserver>();
    public void Subscribe(IGlobalSoundsObserver sub) => subscribers.Add(sub);
    public void Unsubscribe(IGlobalSoundsObserver sub) => subscribers.Remove(sub);
    private void UpdateSubscribers(ISound sound) {
        foreach (IGlobalSoundsObserver sub in subscribers) {
            sub.UpdateSound(sound);
        }
    }

    public void NewSound(ISound sound) {
        foreach (HearingHit hit in GetHearingHits(sound)) {
            hit.enemy.HearSound(hit);
        }
        UpdateSubscribers(sound);
    }

    public IEnumerable<EnemyPerception> Iterator() { foreach (EnemyPerception p in perception) yield return p; }

    protected IEnumerable<HearingHit> GetHearingHits(ISound sound) {
        foreach (EnemyPerception hearing_target in Iterator()) {
            float real_distance = Vector3.Distance(sound.origin, hearing_target.transform.position);
            if (real_distance >= sound.range) { continue; }

            Vector3 direction = (hearing_target.transform.position - sound.origin).normalized;

            if (sound.ignore_walls) {
                // calculate sound ignoring walls
                yield return new HearingHit(sound, hearing_target, real_distance);
            } else if (sound.penetrate_walls) {
                // calculate sound penetrating walls (but not ignoring)
                RaycastHit[] raycast_hits = Physics.RaycastAll(sound.origin, direction, real_distance, LayerMaskSystem.inst.blocks_sounds);
                float adjusted_distance = real_distance - (sound.wall_effectiveness * raycast_hits.Length);
                if (adjusted_distance < sound.range) {
                    yield return new HearingHit(sound, hearing_target, adjusted_distance);
                }
            } else {
                // calculate sound that cannot penetrate walls
                if (Physics.Raycast(sound.origin, direction, real_distance, LayerMaskSystem.inst.blocks_sounds)) {
                    // NOTE --- I am not checking the hits for matching the target, the blocks_sound LayerMask should prevent hits on the target
                    HearingHit hit = GetHearingHitWithNavMesh(sound, hearing_target);
                    if (hit != null) {
                        yield return hit;
                    } else {
                        continue; // hit wall, and no NavMeshPath to target found, so there is no HearingHit, just continue
                    }
                }
                // no raycast hit, or raycast hits target, so there is no wall blocking sound
                yield return new HearingHit(sound, hearing_target, real_distance);
            }
        }
    }

    public HearingHit GetHearingHitWithNavMesh(ISound sound, EnemyPerception hearing_target) {
        NavMeshAgent agent = hearing_target.GetComponent<NavMeshAgent>();
        if (agent == null) {
            Debug.LogError($"Cannot find NavMeshAgent component!");
            return null;
        }

        NavMeshPath path = new NavMeshPath();
        // agent.CalculatePath(sound.origin, path, NavMeshUtils.nav_mesh_sound_area_mask);
        Vector3 start = hearing_target.transform.position;
        NavMesh.CalculatePath(start, sound.origin, NavMeshUtils.nav_mesh_sound_area_mask, path);
        NavMeshUtils.DrawPath(path, Color.yellow, duration:2f);
        bool path_found = path.status == NavMeshPathStatus.PathComplete;
        if (path_found) {
            float effective_range = NavMeshUtils.GetPathLength(path);
            HearingHit hit = new HearingHit(sound, hearing_target, effective_range);
            return hit;
        } else {
            return null;
        }
    }
}

public class HearingHit {
    public ISound sound;
    public float effective_range;
    public EnemyPerception enemy;
    public HearingHit(ISound sound, EnemyPerception enemy, float effective_range) {
        this.sound = sound;
        this.effective_range = effective_range;
        this.enemy = enemy;
    }

    public override string ToString() {
        return $"HearingHit({sound}, {enemy}, range:{effective_range})";
    }
}
