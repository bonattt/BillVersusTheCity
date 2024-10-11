using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStaticGameObject : MonoBehaviour
{
    // script flags a game object to not be destoyed on load
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Destroy(this);
    }
}
