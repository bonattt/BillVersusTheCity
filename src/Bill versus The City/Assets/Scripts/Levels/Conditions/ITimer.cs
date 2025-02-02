using UnityEngine;

public interface ITimer {
    // interface for getting the minutes and seconds from a timer, for UI displaying
    
    public float remaining_time { get; }
    public bool paused { get; set; }
    public int remaining_minutes { get { return (int) (Mathf.Round(remaining_time) / 60); }}

    // returns the seconds left in the current minute
    public int remaining_seconds_remainder { get { return (int) (Mathf.Round(remaining_time) - (remaining_minutes * 60)); }}
    public int remaining_seconds_full { get { return (int) Mathf.Round(remaining_time); }}
}