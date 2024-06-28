

public interface ICharacterStatus {
    public float health { get; set; }
    public float max_health { get; set; }
    public float armor { get; set; }
    public float armor_hardness { get; }

    public void Subscribe(ICharStatusSubscriber sub);

    public void Unsubscribe(ICharStatusSubscriber sub);

    public void UpdateStatus();
}


public interface ICharStatusSubscriber {
    public void StatusUpdated(ICharacterStatus status);
}


public interface IAttack {
    // TODO
}

public interface IAttackTarget {
    // TODO
}


public interface IWeapon {
    // TODO

    
}


public interface IBullet {
    // TODO
}