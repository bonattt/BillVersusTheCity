

public interface ISettingsModule {
    public void Subscribe(ISettingsObserver sub);
    public void Unsubscribe(ISettingsObserver sub);
}


public interface ISettingsObserver {
    public void SettingsUpdated(ISettingsModule updated, string field);
}
