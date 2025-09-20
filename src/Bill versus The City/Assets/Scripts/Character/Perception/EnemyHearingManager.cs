

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

    private List<IHearingObserver> subscribers = new List<IHearingObserver>();
    public void Subscribe(IHearingObserver sub) => subscribers.Add(sub);
    public void Unsubscribe(IHearingObserver sub) => subscribers.Remove(sub);
    private void UpdateSubscribers(ISound sound) {
        foreach (IHearingObserver sub in subscribers) {
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
        Debug.LogWarning($"sound: {sound}"); // TODO --- remove debug
        foreach (EnemyPerception hearing_target in Iterator()) {
            float real_distance = Vector3.Distance(sound.origin, hearing_target.transform.position);
            if (real_distance >= sound.range) { continue; }

            Vector3 direction = (hearing_target.transform.position - sound.origin).normalized;

            if (sound.ignore_walls) {
                Debug.LogWarning("Sound.ignore_walls"); // TODO --- remove debug
                // calculate sound ignoring walls
                yield return new HearingHit(sound, hearing_target, real_distance);
            } else if (sound.penetrate_walls) {
                Debug.LogWarning("Sound.penetrate_walls"); // TODO --- remove debug
                // calculate sound penetrating walls (but not ignoring)
                RaycastHit[] raycast_hits = Physics.RaycastAll(sound.origin, direction, real_distance, LayerMaskSystem.inst.blocks_sounds);
                float adjusted_distance = real_distance - (sound.wall_effectiveness * raycast_hits.Length);
                if (adjusted_distance < sound.range) {
                    yield return new HearingHit(sound, hearing_target, adjusted_distance);
                }
            } else {
                // calculate sound that cannot penetrate walls
                Debug.LogWarning("Sound normal"); // TODO --- remove debug

                if (Physics.Raycast(sound.origin, direction, real_distance, LayerMaskSystem.inst.blocks_sounds)) {
                    Debug.LogWarning("Sound: use navmesh!"); // TODO --- remove debug
                    // NOTE --- I am not checking the hits for matching the target, the blocks_sound LayerMask should prevent hits on the target
                    HearingHit hit = GetHearingHitWithNavMesh(sound, hearing_target);
                    if (hit != null) {
                        yield return hit;
                    } else {
                        continue; // hit wall, and no NavMeshPath to target found, so there is no HearingHit, just continue
                    }
                }
                Debug.LogWarning("Sound no raycast hits!"); // TODO --- remove debug
                // no raycast hit, or raycast hits target, so there is no wall blocking sound
                yield return new HearingHit(sound, hearing_target, real_distance);

                // TODO --- remove debug (uncomment above, delete below)

                // Debug.LogWarning("Sound: use navmesh!"); // TODO --- remove debug
                // // NOTE --- I am not checking the hits for matching the target, the blocks_sound LayerMask should prevent hits on the target
                // HearingHit hit = GetHearingHitWithNavMesh(sound, hearing_target);
                // if (hit != null) {
                //     yield return hit;
                // } else {
                //     continue; // hit wall, and no NavMeshPath to target found, so there is no HearingHit, just continue
                // }
            }

        }
    }

    protected HearingHit GetHearingHitWithNavMesh(ISound sound, EnemyPerception hearing_target) {
        Debug.LogWarning("GetHearingHitWithNavMesh"); // TODO --- remove debug
        NavMeshAgent agent = hearing_target.GetComponent<NavMeshAgent>();
        if (agent == null) {
            Debug.LogError($"Cannot find NavMeshAgent component!");
            return null;
        }

        NavMeshPath path = new NavMeshPath();
        // agent.CalculatePath(sound.origin, path, NavMeshUtils.nav_mesh_sound_area_mask);
        Vector3 start = hearing_target.transform.position;
        Debug.LogWarning($"NavMesh.AllAreas: {NavMesh.AllAreas}, 0b{Convert.ToString(NavMesh.AllAreas, 2)}"); // TODO --- remove debug
        Debug.LogWarning($"NavMeshUtils.nav_mesh_sound_area_mask: {NavMeshUtils.nav_mesh_sound_area_mask}, 0b{Convert.ToString(NavMeshUtils.nav_mesh_sound_area_mask, 2)}"); // TODO --- remove debug
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

public interface ISound
{
    // interface for sounds in game space, which can alert enemies (as opposed to ISound, for sounds played BY the game)
    public string sound_name { get; set; }
    public float alarm_level { get; set; }
    public float range { get; set; }
    public float time { get; }
    public bool ignore_walls { get; set; }
    public bool penetrate_walls { get; set; }
    public bool adjust_alarm_based_on_distance { get; set; }
    public float wall_effectiveness { get; set; }
    public Vector3 origin { get; set; }
    public GameObject sound_source { get; set; }
}

public class GameSound : ISound {
    public const float DEFAULT_WALL_COST = 4f; // the effective distance increase for sounds passing through walls
    public const bool DEFAULT_ADJUST_ALARM_BASED_ON_DISTANCE = true;
    public const bool DEFAULT_IGNORE_WALLS = false;
    public const bool DEFAULT_PENETRATE_WALLS = false;

    // class for sounds in game space, which can alert enemies (as opposed to ISound, for sounds played BY the game)
    public string sound_name { get; set; }
    public float alarm_level { get; set; }
    public float range { get; set; }
    public float time { get; private set; }
    public bool ignore_walls { get; set; }
    public bool penetrate_walls { get; set; }
    public bool adjust_alarm_based_on_distance { get; set; }
    public float wall_effectiveness { get; set; }
    public Vector3 origin { get; set; }
    public GameObject sound_source { get; set; }

    public GameSound(Vector3 origin, float range, float alarm_level) {
        this.time = Time.time;
        this.sound_name = "unnamed sound";
        this.ignore_walls = DEFAULT_IGNORE_WALLS;
        this.penetrate_walls = DEFAULT_PENETRATE_WALLS;
        this.adjust_alarm_based_on_distance = DEFAULT_ADJUST_ALARM_BASED_ON_DISTANCE;
        this.origin = origin;
        this.range = range;
        this.alarm_level = alarm_level;
        this.wall_effectiveness = DEFAULT_WALL_COST;
    }

    public override string ToString() {
        return $"sound({sound_name}): {{'origin': {origin}, 'penetrate_walls': {penetrate_walls}}}";
    }

    public string Jsonify() {
        DuckDict dict = new DuckDict();
        dict.SetString("name", sound_name);
        dict.SetString("origin", $"{origin}");
        dict.SetBool("ignore_walls", ignore_walls);
        dict.SetBool("penetrate_walls", penetrate_walls);
        dict.SetBool("adjust_alarm_based_on_distance", adjust_alarm_based_on_distance);
        dict.SetFloat("range", range);
        dict.SetFloat("alarm_level", alarm_level);
        dict.SetFloat("wall_effectiveness", wall_effectiveness);
        return dict.Jsonify();
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