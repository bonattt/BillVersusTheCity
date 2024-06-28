using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClockDebugg : MonoBehaviour
{
    
    [SerializeField] private int second;
    [SerializeField] private int minute;
    [SerializeField] private int hour;
    [SerializeField] private int day;
    [SerializeField] private int seconds_remaining;
    

    // Update is called once per frame
    void Update()
    {
        this.second = (int) GameClock.instance.seconds;
        this.minute = (int) GameClock.instance.minute;
        this.hour = (int) GameClock.instance.hour;
        this.day = (int) GameClock.instance.day;
        this.seconds_remaining = (int) GameClock.instance.seconds_remaining;
    }
}
