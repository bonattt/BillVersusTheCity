using UnityEngine;

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
    public bool alerts_police { get; set; }
    public Vector3 origin { get; set; }
    public GameObject sound_source { get; set; }
}

public class GameSound : ISound {
    public const float DEFAULT_WALL_COST = 4f; // the effective distance increase for sounds passing through walls
    public const bool DEFAULT_ALERTS_POLICE = false; // whether the sound will start a police timer,
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
    public bool alerts_police { get; set; }
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
        this.alerts_police = DEFAULT_ALERTS_POLICE;
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