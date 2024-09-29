using System.Collections.Generic;

public interface ISettingsModule {
    public void Subscribe(ISettingsObserver sub);
    public void Unsubscribe(ISettingsObserver sub);
    public List<ISettingsObserver> GetSubscribers();  // get all subscribers. Used to transfer subscribers if a settings module is replaced
}


public interface ISettingsObserver {
    public void SettingsUpdated(ISettingsModule updated, string field);
}

public interface IDifficultyAdjusted : ISettingsObserver {
    public float difficulty_multiplier { get; } // currently set difficulty multiplier
    public void SetDiffcultyMultiplier(float difficulty_multiplier);
}