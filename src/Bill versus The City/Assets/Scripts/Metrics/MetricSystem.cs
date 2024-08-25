using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

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

    // public Dictionary<string, float> float_metrics = new Dictionary<string, float>();
    // public Dictionary<string, int> int_metrics = new Dictionary<string, int>();

    public Dictionary<string, dynamic> all_metrics = new Dictionary<string, dynamic>();  //{
    //     {"time_played", 0f},
    // };

    public dynamic GetMetric(string metric) {
        return all_metrics[metric];
    }

    public void SetMetric(string metric, dynamic value) {
        all_metrics[metric] = value;
    }


}
