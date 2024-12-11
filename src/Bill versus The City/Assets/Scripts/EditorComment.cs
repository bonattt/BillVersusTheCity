using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorComment : MonoBehaviour
{
    // Script that just stores a string comment in the UnityEditor, for documentation    
    public string comment = "";
    
    void Start()
    {
        Destroy(this);
    }
}
