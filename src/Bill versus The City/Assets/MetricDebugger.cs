using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetricDebugger : MonoBehaviour
{
    
    public List<string> metrics = new List<string>();

    // Update is called once per frame
    void Update()
    {
        metrics = new List<string>();
        foreach (string key in MetricSystem.instance.ListMetrics()) {
            metrics.Add($"{key}: {MetricSystem.instance.GetMetric(key)}");
        }
    }
}
