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

        Debug.Log($"test metric 'pootis': {MetricSystem.instance.GetFloatMetric("pootis")}");
        Debug.Log($"test metric 'pingas': {MetricSystem.instance.GetIntMetric("pingas")}");

        MetricSystem.instance.IncrimentMetric("pootis", 1f);
        Debug.Log($"test metric 'pootis'+1: {MetricSystem.instance.GetFloatMetric("pootis")}");
        
        MetricSystem.instance.IncrimentMetric("pingas", 1);
        Debug.Log($"test metric 'pingas'+1: {MetricSystem.instance.GetIntMetric("pingas")}");

        MetricSystem.instance.IncrimentMetric("pootis", 21);
        Debug.Log($"test metric 'pootis'+21: {MetricSystem.instance.GetFloatMetric("pootis")}");

        Debug.Log($"has 'pootis': {MetricSystem.instance.HasFloatMetric("pootis")}");
        Debug.Log($"has 'pingas': {MetricSystem.instance.HasIntMetric("pingas")}");

        Debug.Log($"should not have 'pootis' as int: {MetricSystem.instance.HasIntMetric("pootis")}");
        Debug.Log($"should not have 'pingas' as float: {MetricSystem.instance.HasFloatMetric("pingas")}");
        Debug.Log($"should not have 'string': {MetricSystem.instance.HasFloatMetric("string")}");
        Debug.Log($"should not have 'string': {MetricSystem.instance.HasIntMetric("string")}");
    }
}
    