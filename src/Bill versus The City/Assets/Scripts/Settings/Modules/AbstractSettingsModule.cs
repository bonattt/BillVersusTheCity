using System.Collections;
using System.Collections.Generic;

public abstract class AbstractSettingsModule : ISettingsModule {
    // implements ISettingsModule for settings modules    
    private List<ISettingsObserver> subscribers = new List<ISettingsObserver>();

    public void Subscribe(ISettingsObserver sub) => subscribers.Add(sub);
    public void Unsubscribe(ISettingsObserver sub) => subscribers.Remove(sub);

    public void UpdateSubscribers(string field) {
        foreach(ISettingsObserver sub in subscribers) {
            sub.SettingsUpdated(this, field);
        }
    }
}