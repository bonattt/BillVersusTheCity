using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerCharacter {
    /**
      * Non-unity singleton script for tracking player infomration
      * PlayerCharacter can remain consistent accross scenes, both combat, exploration, and dialogue
      *   and track all player stats together in a single place.
      */

    private static PlayerCharacter _inst = null;
    public static PlayerCharacter inst { 
        get {
            if (_inst == null) {
                _inst = new PlayerCharacter();
            }
            return _inst;
        }
    }

    private bool _is_active = true;
    public bool is_active {
        get { return _is_active; }
        set {
            _is_active = value;
            if (combat != null) {
                combat.is_active = value;
            }
        }
    }

    public List<Transform> vision_nodes { get { return combat.movement.vision_nodes; }}

    public GameObject game_object = null;
    private PlayerCombat combat = null;
    public Transform player_transform { get { 
        if (combat == null) { return null; }
        return combat.transform; 
    }}
    public CharacterController character_controller { get { return combat.character_controller; }}

    public void SetGameObject(GameObject obj) {
        this.game_object = obj;
        combat = obj.GetComponent<PlayerCombat>();
    }

    private HashSet<IPlayerObserver> subscribers = new HashSet<IPlayerObserver>();

    public void SubscribeToPlayer(IPlayerObserver sub) => subscribers.Add(sub);
    public void UnsubscribeFromPlayer(IPlayerObserver sub) => subscribers.Remove(sub);

    public void PlayerUpdated(PlayerCombat new_player) {
        // ensure the old combat script is gone
        Debug.LogWarning("new PlayerCombat set");
        if (this.combat != null) {
            UnityEngine.Object.Destroy(this.combat.gameObject);
        }
        this.combat = new_player;

        // update subscribers with new player object
        foreach(IPlayerObserver sub in subscribers) {
            sub.NewPlayerObject(new_player);
        }
    }

    public PlayerCombat GetPlayerCombat(IPlayerObserver sub) {
        // Anything which is accessing subresources of PlayerCombat through callbacks needs to access those resources through
        // this method, which subscribes it to updates to those resources.
        // Objects that just access end-of-chain data can query it directly from PlayerCharacter, since if it changes, player 
        // character will have the up to date data. So long as it is not stored, this is fine.
        SubscribeToPlayer(sub);
        return this.combat;
    }
}

public interface IPlayerObserver {
    public void NewPlayerObject(PlayerCombat player);
}