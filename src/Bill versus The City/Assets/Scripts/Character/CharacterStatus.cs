using System.Collections;
using System.Collections.Generic;

public class CharacterStatus : ICharacterStatus {
    
    public float health { get; set; }
    public float max_health { get; set; }
    public float armor { get; set; }
    public float armor_hardness { get; set; }
    private List<ICharStatusSubscriber> subscribers = new List<ICharStatusSubscriber>();

    public CharacterStatus() : this(100f) { /* do nothing */ }
    public CharacterStatus(float max_health) {
        this.health = max_health;
        this.max_health = max_health;
        this.armor = 0;
        this.armor_hardness = 0;
    }

    public void Subscribe(ICharStatusSubscriber sub) => subscribers.Add(sub);

    public void Unsubscribe(ICharStatusSubscriber sub) => subscribers.Remove(sub);

    public void UpdateStatus() {
        foreach(ICharStatusSubscriber sub in subscribers) {
            sub.StatusUpdated(this);
        }
    }
}