using System.Collections;
using System.Collections.Generic;
using System.Linq; 

using UnityEngine;

public class AmmoContainer : MonoBehaviour, IGenericObservable, IInteractionEffect
{
    public int handgun_max = 85;  // 5 mags of 17 shots
    public int handgun_start = 85;
    public int magnum_max = 50;
    public int magnum_start = 50;
    public int rifle_max = 30;
    public int rifle_start = 30;
    public int shotgun_max = 10;
    public int shotgun_start = 10;

    public int debug_rifle, debug_shotgun;

    public string interaction_sound_path = "reload_finished";

    // guarantees a consisten order to which fields are displayed
    // guarantees a consisten order to which fields are displayed
    public static readonly AmmoType[] display_order = new AmmoType[]{AmmoType.handgun, AmmoType.rifle, AmmoType.shotgun, AmmoType.magnum};

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

    public string GetTextDisplay(AmmoType type) {
        // gets a line of text to display the contents of the container for this ammo type (eg. "Shotgun 10 / 20")
        return $"{AmmoTypeDisplay.DisplayValue(type)}: {GetCount(type)} / {GetMax(type)}";
    }

    public void Initialize() {
        ammo_count[AmmoType.handgun] = handgun_start;
        ammo_max[AmmoType.handgun] = handgun_max;
        ammo_count[AmmoType.magnum] = magnum_start;
        ammo_max[AmmoType.magnum] = magnum_max;
        ammo_count[AmmoType.rifle] = rifle_start;
        ammo_max[AmmoType.rifle] = rifle_max;
        ammo_count[AmmoType.shotgun] = shotgun_start;
        ammo_max[AmmoType.shotgun] = shotgun_max;
    }

    public bool TransferAmmoFrom(AmmoContainer other) {
        bool ammo_transfered = false;  // has any ammo been transfered?
        // iterate over only keys that are in both containiers
        foreach (AmmoType type in ammo_max.Keys.Intersect(other.ammo_max.Keys)) {
            bool _ammo_transfered = TransferAmmoFrom(type, other);
            if (_ammo_transfered) {
            }
            ammo_transfered |= _ammo_transfered;
        }
        return ammo_transfered;
    }

    public bool TransferAmmoFrom(AmmoType type, AmmoContainer other) {
        // transfers ammo of the given type from the other container to this one.
        // if no ammo can be transfered, return false.
        // any ammo is actually transfere, return true.
        int needed_ammo = AmmoNeeded(type);
        if (needed_ammo == 0) { 
            return false; 
        } // we don't need this ammo type, so none is transfered 
        else if (other.ammo_count[type] == 0) { 
            return false; 
        } // other container doesn't have this ammo, so none is transfered
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
        return true; // some amount of ammo was transfered, so return true.
    }

    public int GetMax(AmmoType type) {
        if (! ammo_count.ContainsKey(type)) {
            return 0;
        }
        return ammo_max[type];
    }

    public int GetCount(AmmoType type) {
        if (! ammo_count.ContainsKey(type)) {
            return 0;
        }
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

    private static AmmoContainer FindAmmoContainer(GameObject obj) {
        // tries to find an AmmoContainer script on the given object or a parent
        AmmoContainer container = obj.GetComponent<AmmoContainer>();
        while (container == null && obj.transform.parent != null) {
            obj = obj.transform.parent.gameObject;
            container = obj.GetComponent<AmmoContainer>();
        }
        return container;
    }

    public void Interact(GameObject obj) {
        AmmoContainer other = FindAmmoContainer(obj);
        if (other == null) {
            Debug.LogError($"unable to find AmmoContainer on {obj}!");
        }
        bool ammo_transfered = other.TransferAmmoFrom(this);
        if (ammo_transfered) {
            // play sound, but only if ammo is actually transfered.
            PlayInteractionSound();
        }
    }

    public void PlayInteractionSound() {
        if (interaction_sound_path.Equals("")) {
            return; // do not play sound if there is no sound
        }
        ISFXSounds sounds = SFXLibrary.LoadSound(interaction_sound_path);
        SFXSystem.inst.PlaySound(sounds, transform.position);
    }

    void Update() {
        UpdateDebug();
    }

    public void UpdateDebug() {
        debug_rifle = ammo_count[AmmoType.rifle];
        debug_shotgun = ammo_count[AmmoType.shotgun];
    }
}
