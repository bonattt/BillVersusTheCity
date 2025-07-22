using System.Collections.Generic;

public interface ISettingsModule {

    public List<string> float_field_names { get; }
    public List<string> int_field_names { get; }
    public List<string> bool_field_names { get; }
    public List<string> other_field_names { get; }
    // public List<string> all_field_names { get; }
    public void Subscribe(ISettingsObserver sub);
    public void Unsubscribe(ISettingsObserver sub);
    public string AsJson(); // returns json data for the settings in this module
    public void LoadFromJson(DuckDict data, bool update_subscribers=true);  // sets the settings module from a JSON string
    public IEnumerable<ISettingsObserver> SubscriberIterator();  // get all subscribers. Used to transfer subscribers if a settings module is replaced
}


public interface ISettingsObserver {
    public void SettingsUpdated(ISettingsModule updated, string field);
}

public interface IDifficultyAdjusted : ISettingsObserver {
    public float difficulty_multiplier { get; } // currently set difficulty multiplier
    public void SetDiffcultyMultiplier(float difficulty_multiplier);
}