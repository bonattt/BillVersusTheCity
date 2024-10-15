using System.IO;

using UnityEngine;

public class SaveFile {

    public string save_name { get; protected set; }
    public const string SAVE_SLOT_1 = "save_1";
    public const string SAVE_FILE_DIR = ".save_files";

    public static SaveFile current_save = null;

    public string filepath {
        get {
            // TODO --- if save_name is ever user-generated, this will need to be sanitized
            return Path.Join(Application.dataPath, $"{SAVE_FILE_DIR}\\{save_name}");
        }
    }

    public SaveFile(string save_name) {
        this.save_name = save_name;
    }

    public static SaveFile Load(string save_name) {
        SaveFile save = new SaveFile(save_name);
        save.LoadFromFile();
        return save;
    }

    public string AsJson() {
        DuckDict data = new DuckDict();
        data.SetObject("settings", JsonParser.ReadAsDuckDict(GameSettings.inst.AsJson()));
        return data.Jsonify();
    }

    public void Save() {
        File.WriteAllText(this.filepath, AsJson());
    }

    public void SaveOnExit() {
        // saves SOME save data, such as settings 
        Save();
    }

    public void LoadFromJson(string json_str) {
        // takes a json string, and loads the data
        // TODO ---
        DuckDict data = JsonParser.ReadAsDuckDict(json_str);
        DuckDict settings_data = data.GetObject("settings");
        GameSettings.inst.LoadFromJson(settings_data.Jsonify());
    } 

    public void LoadFromFile() {
        // reads the save file and loads it's json data
        string file_text = File.ReadAllText(this.filepath);
        this.LoadFromJson(file_text);
    }

}
