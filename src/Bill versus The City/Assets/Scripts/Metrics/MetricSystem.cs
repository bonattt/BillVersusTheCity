using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

using UnityEngine;

public class MetricSystem {

    private static MetricSystem _instance = null;
    public static MetricSystem instance {
        get {
            if (_instance == null) {
                _instance = new MetricSystem();
            }
            return _instance;
        }
    }

    public Dictionary<string, float> float_metrics = new Dictionary<string, float>();
    public Dictionary<string, int> int_metrics = new Dictionary<string, int>();

    private List<IMetricSubscriber> subscribers = new List<IMetricSubscriber>();
    public void SubscribeToMetrics(IMetricSubscriber new_sub) => subscribers.Add(new_sub); 
    public void UnsubscribeFromMetrics(IMetricSubscriber sub) => subscribers.Remove(sub); 

    private void UpdateMetricSubsInt(string metric) {
        foreach (IMetricSubscriber sub in subscribers) {
            sub.IntMetricUpdated(this, metric);
        }
    }

    private void UpdateMetricSubsFloat(string metric) {
        foreach (IMetricSubscriber sub in subscribers) {
            sub.FloatMetricUpdated(this, metric);
        }
    }

    public float GetFloatMetric(string metric) {
        if (! HasFloatMetric(metric)) {
            float_metrics[metric] = 0f;
        }
        return float_metrics[metric];
    }

    public int GetIntMetric(string metric) {
        if (! HasIntMetric(metric)) {
            int_metrics[metric] = 0;
        }
        return int_metrics[metric];
    }

    public void SetMetric(string metric, int value) {
        SetIntMetric(metric, value);
    }
    
    public void SetMetric(string metric, float value) {
        SetFloatMetric(metric, value);
    }

    public void SetFloatMetric(string metric, float value) {
        float_metrics[metric] = value;
        if (HasIntMetric(metric)) {
            Debug.LogWarning($"setting float metric '{metric}' = {value} for existing int metric '{metric}' = {GetIntMetric(metric)}");
        }
        UpdateMetricSubsFloat(metric);
    }

    public void SetIntMetric(string metric, int value) {
        int_metrics[metric] = value;
        if (HasFloatMetric(metric)) {
            Debug.LogWarning($"setting int metric '{metric}' = {value} for existing int metric '{metric}' = {GetFloatMetric(metric)}");
        }
        UpdateMetricSubsInt(metric);
    }

    public List<string> ListFloatMetrics() {
        return new List<string>(float_metrics.Keys);
    }


    public List<string> ListIntMetrics() {
        return new List<string>(int_metrics.Keys);
    }

    public bool HasIntMetric(string metric) {
        return int_metrics.ContainsKey(metric);
    } 

    public bool HasFloatMetric(string metric) {
        return float_metrics.ContainsKey(metric);
    } 

    public void IncrimentMetric(string metric, float value) {
        float initial_value = GetFloatMetric(metric);
        SetFloatMetric(metric, initial_value + value);
    }

    public void IncrimentMetric(string metric, int value) {
        int initial_value = GetIntMetric(metric);
        SetIntMetric(metric, initial_value + value);
    }
}


[System.Serializable]
public class MetricSystemException : System.Exception
{
    public MetricSystemException() { }
    public MetricSystemException(string message) : base(message) { }
    public MetricSystemException(string message, System.Exception inner) : base(message, inner) { }
    protected MetricSystemException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}