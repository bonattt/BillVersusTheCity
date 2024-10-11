using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

using UnityEngine;

public class TestDynamicJson : MonoBehaviour
{
    public string json_output = "";
    public float dynamic_float = -420.69f;
    public int dynamic_int = 42;
    public string dynamic_string = "string";
    public bool dynamic_bool = true;


    // Update is called once per frame
    void Update()
    {
        DuckDict dict = new DuckDict();
        dict.SetFloat("float", dynamic_float);
        dict.SetInt("int", dynamic_int);
        dict.SetString("string", dynamic_string);
        dict.SetBool("bool", dynamic_bool);

        DuckDict sub_dict = dict.Copy();
        dict.SetObject("object", sub_dict);

        json_output = dict.Jsonify();
    }
}
