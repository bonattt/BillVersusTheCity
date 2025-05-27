using System.Collections;
using System.Collections.Generic;

public abstract class AbstractSettingsModule : ISettingsModule {
    // implements ISettingsModule for settings modules    
    private List<ISettingsObserver> subscribers = new List<ISettingsObserver>();

    public void Subscribe(ISettingsObserver sub) => subscribers.Add(sub);
    public void Unsubscribe(ISettingsObserver sub) => subscribers.Remove(sub);

    public abstract DuckDict AsDuckDict(); // returns json data for the settings in this module
    public virtual string AsJson() {
        return AsDuckDict().Jsonify();
    }
    public abstract void LoadFromJson(DuckDict data_module);  // sets the settings module from a JSON string
    public virtual void SetToNewGameDefault() { /* do nothing by default */}

    public void UpdateSubscribers(string field)
    {
        foreach (ISettingsObserver sub in subscribers)
        {
            sub.SettingsUpdated(this, field);
        }
    }

    public abstract List<string> all_fields { get; }

    public void AllFieldsUpdates() {
        // updates subscribers for ALL fields
        foreach (string field_name in all_fields) {
            UpdateSubscribers(field_name);
        }
    }

    public List<ISettingsObserver> GetSubscribers() {
        return new List<ISettingsObserver>(subscribers);
    }
}