using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AbstractSettingsModule : ISettingsModule {
    // implements ISettingsModule for settings modules    
    
    public virtual List<string> float_field_names { get => new List<string>(); }
    public virtual List<string> int_field_names { get => new List<string>(); }
    public virtual List<string> bool_field_names { get => new List<string>(); }
    public virtual List<string> other_field_names { get => new List<string>(); }
    public virtual List<string> all_field_names {
        get {
            return float_field_names.Concat(int_field_names).Concat(bool_field_names).Concat(other_field_names).ToList();
        }
    }
    private List<ISettingsObserver> subscribers = new List<ISettingsObserver>();

    public void Subscribe(ISettingsObserver sub) => subscribers.Add(sub);
    public void Unsubscribe(ISettingsObserver sub) => subscribers.Remove(sub);

    protected Dictionary<string, float> float_fields = new Dictionary<string, float>();
    protected Dictionary<string, int> int_fields = new Dictionary<string, int>();
    protected Dictionary<string, bool> bool_fields = new Dictionary<string, bool>();

    public virtual DuckDict AsDuckDict() {
        // returns json data for the settings in this module
        DuckDict dict = new DuckDict();
        foreach (string key in float_field_names) {
            dict.SetFloat(key, float_fields[key]);
        }
        foreach (string key in int_field_names) {
            dict.SetInt(key, int_fields[key]);
        }
        foreach (string key in bool_field_names) {
            dict.SetBool(key, bool_fields[key]);
        }
        // NOTE: other_field_names needs custom handling by subclasses, overrwrite this method
        return dict;
    }
    public string AsJson() {
        return AsDuckDict().Jsonify();
    }
    public virtual void LoadFromJson(DuckDict module_save_data, bool update_subscribers = true) {
        // sets the settings module from a JSON string
        foreach (string key in float_field_names) {
            float? float_value = module_save_data.GetFloat(key);
            if (float_value == null) { continue; }
            float_fields[key] = float_value.Value;
        }
        foreach (string key in int_field_names) {
            int? int_value = module_save_data.GetInt(key);
            if (int_value == null) { continue; }
            int_fields[key] = int_value.Value;
        }
        foreach (string key in bool_field_names) {
            bool? bool_value = module_save_data.GetBool(key);
            if (bool_value == null) { continue; }
            bool_fields[key] = bool_value.Value;
        }
        // NOTE: other_field_names needs custom handling by subclasses, overrwrite this method
        if (update_subscribers) {
            AllFieldsUpdated();
        }
    }

    public float GetFloat(string field) {
        return float_fields[field];
    }
    public int GetInt(string field) {
        return int_fields[field];
    }
    public bool GetBool(string field) {
        return bool_fields[field];
    }

    public void SetFloat(string field, float value) {
        float_fields[field] = value;
    }
    public void SetInt(string field, int value) {
        int_fields[field] = value;
    }
    public void SetBool(string field, bool value) {
        bool_fields[field] = value;
    }


    protected virtual float DefaultFloatValue(string field_name) => 1f;
    protected virtual int DefaultIntValue(string field_name) => 0;
    protected virtual bool DefaultBoolValue(string field_name) => false;

    public virtual void SetToNewGameDefault() { /* do nothing by default */}

    public void UpdateSubscribers(string field) {
        foreach (ISettingsObserver sub in subscribers) {
            sub.SettingsUpdated(this, field);
        }
    }

    public void AllFieldsUpdated() {
        // updates subscribers for ALL fields
        foreach (string field_name in all_field_names) {
            UpdateSubscribers(field_name);
        }
    }

    public IEnumerable<ISettingsObserver> SubscriberIterator() {
        foreach (ISettingsObserver sub in subscribers) {
            yield return sub;
        }
    }
}