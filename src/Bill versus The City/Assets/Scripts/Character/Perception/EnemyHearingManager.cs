

using System.Collections.Generic;
using UnityEngine;

public class EnemyHearingManager : IGenericObserver
{
    private static EnemyHearingManager _inst;
    public static EnemyHearingManager inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = new EnemyHearingManager();
            }
            return _inst;
        }
    }

    private List<EnemyPerception> perception;

    public EnemyHearingManager()
    {
        EnemiesManager.inst.Subscribe(this);
        UpdateObserver(EnemiesManager.inst);
    }

    public void UpdateObserver(IGenericObservable _) {
        perception = new List<EnemyPerception>();
        foreach (NavMeshAgentMovement enemy in EnemiesManager.inst.GetRemainingEnemies())
        {
            EnemyPerception new_perception = enemy.GetComponent<EnemyPerception>();
            if (new_perception == null)
            {
                Debug.LogError("Enemy without perception!!");
                continue;
            }
            perception.Add(new_perception);
        }
    }

    private List<IHearingObserver> subscribers = new List<IHearingObserver>();
    public void Subscribe(IHearingObserver sub) => subscribers.Add(sub);
    public void Unsubscribe(IHearingObserver sub) => subscribers.Remove(sub);
    private void UpdateSubscribers(ISound sound)
    {
        foreach (IHearingObserver sub in subscribers)
        {
            sub.UpdateSound(sound);
        }
    }

    public void NewSound(ISound sound)
    {
        foreach (HearingHit hit in GetHearingHits(sound))
        {
            hit.enemy.HearSound(hit);
        }
        UpdateSubscribers(sound);
    }

    public IEnumerable<EnemyPerception> Iterator() { foreach (EnemyPerception p in perception) yield return p; }

    protected IEnumerable<HearingHit> GetHearingHits(ISound sound)
    {
        foreach (EnemyPerception hearing_target in Iterator())
        {
            float real_distance = Vector3.Distance(sound.origin, hearing_target.transform.position);
            if (real_distance >= sound.range) { continue; }

            Vector3 direction = (hearing_target.transform.position - sound.origin).normalized;

            if (sound.ignore_walls) {
                yield return new HearingHit(sound, hearing_target, real_distance);
                continue;
            } else if (!sound.penetrate_walls) { continue; }

            RaycastHit[] raycast_hits = Physics.RaycastAll(sound.origin, direction, real_distance, LayerMaskSystem.inst.blocks_sounds);
            float adjusted_distance = real_distance - (sound.wall_effectiveness * raycast_hits.Length);
            if (adjusted_distance < sound.range)
            {
                yield return new HearingHit(sound, hearing_target, adjusted_distance);
            }
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

public class GameSound : ISound
{
    public const float DEFAULT_WALL_COST = 4f; // the effective distance increase for sounds passing through walls
    public const bool DEFAULT_ADJUST_ALARM_BASED_ON_DISTANCE = true;
    public const bool DEFAULT_IGNORE_WALLS = false;
    public const bool DEFAULT_PENETRATE_WALLS = true;

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

    public GameSound(Vector3 origin, float range, float alarm_level)
    {
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
}

public class HearingHit
{
    public ISound sound;
    public float effective_range;
    public EnemyPerception enemy;
    public HearingHit(ISound sound, EnemyPerception enemy, float effective_range)
    {
        this.sound = sound;
        this.effective_range = effective_range;
        this.enemy = enemy;
    }
}