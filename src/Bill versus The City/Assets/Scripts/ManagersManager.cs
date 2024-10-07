using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersManager : MonoBehaviour
{
    // script for managing the parent object for all manager scripts
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
