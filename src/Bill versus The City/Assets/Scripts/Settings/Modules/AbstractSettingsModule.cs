using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class AbstractSettingsModule : ISettingsModule {
    // implements ISettingsModule for settings modules    
    private List<ISettingsObserver> subscribers = new List<ISettingsObserver>();

    public void Subscribe(ISettingsObserver sub) => subscribers.Add(sub);
    public void Unsubscribe(ISettingsObserver sub) => subscribers.Remove(sub);

    protected Dictionary<string, float> float_fields = new Dictionary<string, float>();
    protected Dictionary<string, int> int_fields = new Dictionary<string, int>();
    protected Dictionary<string, bool> bool_fields = new Dictionary<string, bool>();

    public virtual DuckDict AsDuckDict() {
        // returns json data for the settings in this module
        DuckDict dict = new DuckDict();
        foreach (string key in float_fields.Keys) {
            if (key.Equals(DifficultySettings.PLAYER_HEALTH)) { Debug.LogWarning($"PlayerHealth written to file: {float_fields[key]}"); } // TODO --- remove debug
            dict.SetFloat(key, float_fields[key]);
        }
        foreach (string key in int_fields.Keys) {
            dict.SetInt(key, int_fields[key]);
        }
        foreach (string key in bool_fields.Keys) {
            dict.SetBool(key, bool_fields[key]);
        }
        return dict;
    }
    public virtual string AsJson() {
        return AsDuckDict().Jsonify();
    }
    public virtual void LoadFromJson(DuckDict data_module) {
        // sets the settings module from a JSON string
        foreach (string key in data_module.Keys) {
            switch (data_module[key]) {
                case null:
                    Debug.LogWarning($"field '{key}' is null in save data"); // TODO --- remove debug
                    break; // null values cannot be typed, so they are not added to a dict. Rely on default values for these cases
                case DuckValueFloat float_value:
                    if (key.Equals(DifficultySettings.PLAYER_HEALTH)) { Debug.LogWarning($"PlayerHealth loaded from file: {float_value.GetFloat()}"); } // TODO --- remove debug
                    float_fields[key] = float_value.GetFloat();
                    break;
                case DuckValueInt int_value:
                    int_fields[key] = int_value.GetInt();
                    break;

                case DuckValueBool bool_value:
                    bool_fields[key] = bool_value.GetBool();
                    break;

                default:
                    Debug.LogWarning($"field '{key}' is not a handled type, and should be handled in subclass");
                    break;
            }
        }
    }


    protected virtual float DefaultFloatValue(string field_name) => 1f;
    protected virtual int DefaultIntValue(string field_name) => 0;
    protected virtual bool DefaultBoolValue(string field_name) => true;

    public virtual void SetToNewGameDefault() { /* do nothing by default */}

    public void UpdateSubscribers(string field) {
        foreach (ISettingsObserver sub in subscribers) {
            sub.SettingsUpdated(this, field);
        }
    }

    public abstract List<string> all_fields { get; }

    public void AllFieldsUpdated() {
        // updates subscribers for ALL fields
        foreach (string field_name in all_fields) {
            UpdateSubscribers(field_name);
        }
    }

    public IEnumerable<ISettingsObserver> SubscriberIterator() {
        foreach (ISettingsObserver sub in subscribers) {
            yield return sub;
        }
    }
}