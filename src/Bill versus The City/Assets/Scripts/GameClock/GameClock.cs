using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClock : MonoBehaviour
{   
    public static readonly float SECONDS_PER_DAY = 60 * 60 * 24;

    private const float SECONDS_PER_MINUTE = 60;
    private const float SECONDS_PER_HOUR = 3600;
    private const float MINUTES_PER_HOUR = 3600;

    public float time_scale = 60f; // number of in-game seconds that pass per real-time second

    public bool paused = false;

    private List<IClockSubscriber> subscribers = new List<IClockSubscriber>();

    private static GameClock _instance;
    public static GameClock instance {
        get { return _instance; }
    }

    // current time of day in seconds.
    [SerializeField]
    private float _seconds = 0f;
    public float seconds {
        get {
            return _seconds;
        }
        private set {
            _seconds = value;
            while (_seconds >= SECONDS_PER_DAY) {
                _seconds -= SECONDS_PER_DAY;
                day += 1;
            }
        }
    }

    // seconds left in the day
    public float seconds_remaining {
        get {
            return SECONDS_PER_DAY - seconds;
        }
    }

    // current minute on the clock (out of 60 minutes in an hour)
    public int minute {
        get {
            return (int) (minutes / (hour * MINUTES_PER_HOUR));
        }
    }

    // exact number of minutes that have passed today
    public float minutes {
        get {
            return seconds / SECONDS_PER_MINUTE;
        }
    }

    // current hour on the clock (out of 24 hours in a day)
    public int hour {
        get {
            return (int) hours;
        }
    }

    // Exact number of hours that have passed today
    public float hours {
        get {
            return seconds / SECONDS_PER_HOUR;
        }
    }

    [SerializeField] private int _day = 0;
    public int day { 
        get {
            return _day;
        }
        private set {
            _day = value;
        }
    }

    void Awake() {
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (paused) { return; }
        float unity_time = Time.deltaTime;
        float delta_seconds = unity_time * time_scale;
        int days_initial = day;
        seconds += delta_seconds;
        int delta_days = this.day - days_initial;

        foreach (IClockSubscriber sub in subscribers) {
            sub.UpdateTime(unity_time, delta_seconds);
            if (delta_days != 0) {
                sub.UpdateDays(delta_days);
            }
        }
        
    }

    public void Subscribe(IClockSubscriber sub) {
        subscribers.Add(sub);
    }

    public void Unsubscribe(IClockSubscriber sub) {
        subscribers.Remove(sub);
    }

    public void SkipTime(float seconds_skipped) {
        this.seconds += seconds_skipped;
        _SkipTime(seconds_skipped);
    }

    public void SkipToTime(float target_time) {
        int day_initial = this.day;
        float seconds_passed;
        if (target_time < this.seconds) {
            seconds_passed = seconds_remaining + target_time;
        } else {
            seconds_passed = target_time - this.seconds;
        }
        this.seconds += seconds_passed;
        _SkipTime(seconds_passed, this.day - day_initial); 
    }

    private void _SkipTime(float seconds) {
        _SkipTime(seconds, 0);
    }
    private void _SkipTime(float seconds, int days_passed) {
        Debug.Log($"Skip Time {seconds}s for {subscribers.Count} subs.");
        foreach(IClockSubscriber sub in subscribers) {
            sub.UpdateTimeSkip(seconds);
            sub.UpdateDays(days_passed);
        }
    }

}

