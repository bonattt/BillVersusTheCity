using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LagSpikeTest : MonoBehaviour
{
    public float lag_chance = 0.05f;
    public float min_spike_seconds = 0.01f;
    public float max_spike_seconds = 0.1f;

    private bool LagThisFrame() {
        return UnityEngine.Random.Range(0f, 1f) <= lag_chance;
    }

    private int LagTime() {
        // return a number of miliseconds to sleep for
        return (int) (1000 * UnityEngine.Random.Range(min_spike_seconds, max_spike_seconds));
    }

    // Update is called once per frame
    void Update()
    {
        if (LagThisFrame()) {
            Thread.Sleep(LagTime());
        }
    }
}
