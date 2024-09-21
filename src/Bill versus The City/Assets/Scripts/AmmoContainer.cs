using System.Collections;
using System.Collections.Generic;
using System.Linq; 

using UnityEngine;

public class AmmoContainer : MonoBehaviour, IGenericObservable, IInteractionEffect
{
    public int handgun_max = 80;
    public int handgun_start = 80;
    public int rifle_max = 30;
    public int rifle_start = 30;
    public int shotgun_max = 10;
    public int shotgun_start = 10;

    public int debug_rifle, debug_shotgun;

    private Dictionary<AmmoType, int> ammo_count = new Dictionary<AmmoType, int>();
    private Dictionary<AmmoType, int> ammo_max = new Dictionary<AmmoType, int>();

    void Start() {
        Initialize();
        UpdateSubscribers();
    }
    
    private List<IGenericObserver> subscribers = new List<IGenericObserver>();
    public void Subscribe(IGenericObserver sub) => this.subscribers.Add(sub);
    public void Unusubscribe(IGenericObserver sub) => this.subscribers.Remove(sub);
    public void UpdateSubscribers() {
        foreach (IGenericObserver sub in subscribers) {
            sub.UpdateObserver(this);
        }
    }

    public void Initialize() {
        // ammo_count[AmmoType.handgun] = handgun_start;
        // ammo_max[AmmoType.handgun] = handgun_max;
        ammo_count[AmmoType.rifle] = rifle_start;
        ammo_max[AmmoType.rifle] = rifle_max;
        ammo_count[AmmoType.shotgun] = shotgun_start;
        ammo_max[AmmoType.shotgun] = shotgun_max;
    }

    public void TransferAmmoFrom(AmmoContainer other) {
        // iterate over only keys that are in both containiers
        foreach (AmmoType type in ammo_max.Keys.Intersect(other.ammo_max.Keys)) {
            TransferAmmoFrom(type, other);
        }        
    }

    public void TransferAmmoFrom(AmmoType type, AmmoContainer other) {
        int needed_ammo = AmmoNeeded(type);
        if (other.ammo_count[type] >= needed_ammo) {
            other.ammo_count[type] -= needed_ammo;
            this.ammo_count[type] += needed_ammo;
        }
        else {
            this.ammo_count[type] += other.ammo_count[type];
            other.ammo_count[type] = 0;
        }
        UpdateSubscribers();
        other.UpdateSubscribers();
    }

    public int GetMax(AmmoType type) {
        return ammo_max[type];
    }

    public int GetCount(AmmoType type) {
        return ammo_count[type];
    }

    public void UseAmmo(AmmoType type, int amount) {
        SetCount(type, GetCount(type) - amount);
    }

    public void SetCount(AmmoType type, int amount) {
        if (amount <= 0) {
            ammo_count[type] = 0;
        }
        else if (amount >= ammo_max[type]) {
            ammo_count[type] = ammo_max[type];
        }
        else {
            ammo_count[type] = amount;
        }
        UpdateSubscribers();
    }

    public bool HasAmmoType(AmmoType type) {
        return ammo_count.ContainsKey(type) && ammo_max.ContainsKey(type);
    }

    public int AmmoNeeded(AmmoType type) {
        // gets the ammount of ammo needed to fill this container with the given ammo type
        return this.ammo_max[type] - this.ammo_count[type];
    }

    public void Interact(GameObject obj) {
        AmmoContainer other = obj.GetComponent<AmmoContainer>();
        if (other == null) {
            Debug.LogError($"unable to find AmmoContainer on {obj}!");
        }
        other.TransferAmmoFrom(this);
    }

    void Update() {
        UpdateDebug();
    }

    public void UpdateDebug() {
        debug_rifle = ammo_count[AmmoType.rifle];
        debug_shotgun = ammo_count[AmmoType.shotgun];
    }
}
