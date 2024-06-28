

public interface IClockSubscriber {
    public void UpdateTime(float delta_unity, float seconds);

    public void UpdateDays(int days) {
        // do nothing
    }

    public void UpdateTimeSkip(float seconds) {
        this.UpdateTime(0, seconds);
    }
}