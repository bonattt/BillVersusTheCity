using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneEvent : MonoBehaviour, IGameEvent, IInteractionEffect
{

    public string next_scene = "Demo001--level01";
    
    public void ActivateEvent() {
        Effect();
    }
    
    public void Interact(GameObject actor) {
        Effect();
    }


    private void Effect() {
        ScenesUtil.NextLevel(next_scene);
    }
}
