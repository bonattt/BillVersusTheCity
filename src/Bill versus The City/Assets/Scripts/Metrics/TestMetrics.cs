using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMetrics : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MetricSystem.instance.SetMetric("pootis", 420.69f);
        MetricSystem.instance.SetMetric("pingas", 42);
        MetricSystem.instance.SetMetric("string", "string");

        Debug.Log($"test metric 'pootis': {MetricSystem.instance.GetMetric("pootis")}");
        Debug.Log($"test metric 'pingas': {MetricSystem.instance.GetMetric("pingas")}");
        Debug.Log($"test metric 'string': {MetricSystem.instance.GetMetric("string")}");
    }
}
    