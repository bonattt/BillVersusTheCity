using System.Collections.Generic;

using UnityEngine;

public class DebugSaveJson : MonoBehaviour
{
    public bool update_output = true;

    public string save_name, save_file, save_profile, raw_json;
    public List<string> json_lines;

    // Update is called once per frame
    void Update()
    {
        if (update_output) {
            update_output = false;
            UpdateOutput();
        }
    }

    private static string DisplayStr(string base_str)  {
        if (base_str == null) {
            return "<null>";
        }
        return base_str;
    }

    private void UpdateOutput() {
        if (SaveProfile.inst.save_file == null) {
            save_profile = "";
            save_name = "";
            save_file = "null";
            json_lines = new List<string>();
            raw_json = "null";
        }
        else
        {
            save_profile = DisplayStr(SaveProfile.inst.save_file.profile_name);
            save_name = DisplayStr(SaveProfile.inst.save_file.save_name);
            save_file = DisplayStr(SaveProfile.inst.save_file.filepath);
            json_lines = new List<string>();
            raw_json = SaveProfile.inst.save_file.AsDuckDict().Jsonify();
            foreach (string line in FormatJson(raw_json).Split("\n"))
            {
                json_lines.Add(line);
            }
        }
    }

    private static string FormatJson(string json_in) {
        return json_in;
    }
}
