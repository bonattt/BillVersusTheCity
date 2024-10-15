using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneEvent : MonoBehaviour, IGameEvent
{

    public string next_scene = "Demo001--level01";
    
    public void ActivateEvent() {
        ScenesUtil.NextLevel(next_scene);
    }
}
