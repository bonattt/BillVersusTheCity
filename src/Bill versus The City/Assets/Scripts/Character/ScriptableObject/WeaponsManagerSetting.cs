using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Weapon Manager", menuName ="Data/WeaponManagerSetting")]
public class WeaponManagerSetting : ScriptableObject
{
    /** Class for initializing which weapons the player has unlocked
      * 
      */


    public DetailedWeapon _handgun, _rifle, _shotgun, _smg;
    public IWeapon handgun { get { return (IWeapon) _handgun; } }
    public IWeapon rifle { get { return (IWeapon) _rifle; } }
    public IWeapon shotgun { get { return (IWeapon) _shotgun; } }
    public IWeapon smg { get { return (IWeapon) _smg; } }

    
    public bool _handgun_unlocked, _rifle_unlocked, _shotgun_unlocked, _smg_unlocked;
    public bool handgun_unlocked { get { return _handgun_unlocked; } }
    public bool rifle_unlocked { get { return _rifle_unlocked; } }
    public bool shotgun_unlocked { get { return _shotgun_unlocked; } }
    public bool smg_unlocked { get { return _smg_unlocked; } }
}
