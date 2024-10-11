using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObservable : MonoBehaviour
{
    // manager used to update observers whenever a new player object is created

    private List<IPlayerObserver> subscribers = new List<IPlayerObserver>();

    public static PlayerObservable inst { get; private set; }
    void Awake() {
        inst = this;
    }

    public void SubscribeToPlayer(IPlayerObserver sub) => subscribers.Add(sub);
    public void UnsubscribeToPlayer(IPlayerObserver sub) => subscribers.Remove(sub);

    public void PlayerUpdated(PlayerMovement new_player) {
        foreach(IPlayerObserver sub in subscribers) {
            sub.NewPlayerObject(new_player);
        }
    }
}


public interface IPlayerObserver {
    public void NewPlayerObject(PlayerMovement player);
}