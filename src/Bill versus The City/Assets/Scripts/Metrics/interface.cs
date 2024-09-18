

public interface IMetricSubscriber {
    public void IntMetricUpdated(MetricSystem metric_system, string metric);
    public void FloatMetricUpdated(MetricSystem metric_system, string metric);
}