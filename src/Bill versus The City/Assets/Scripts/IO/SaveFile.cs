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
            return Path.Join(directory_path, save_name);
        }
    }

    public static string directory_path {
        get {
            return Path.Join(Application.dataPath, SAVE_FILE_DIR);
        }
    }

    public SaveFile(string save_name) {
        this.save_name = save_name;
    }

    public static void SetupDirectory() {
        // validates the saves directory exists, and creates it if it's not there.
        if (! Directory.Exists(directory_path)) {
            Directory.CreateDirectory(directory_path);
        }
    }

    public static SaveFile NewSave(string save_name) {
        SaveFile new_save = new SaveFile(save_name);
        // initialize
        // TODO ---
        return new_save;
    }

    public static SaveFile Load(string save_name) {
        SaveFile save = new SaveFile(save_name);
        save.LoadFromFile();
        return save;
    }

    // public string AsJson() {
    //     DuckDict data = new DuckDict();
    //     return data.Jsonify();
    // }

    private void WriteSettingsData(DuckDict data) {
        data.SetObject("settings", JsonParser.ReadAsDuckDict(GameSettings.inst.AsJson()));
    }

    public void SaveAll() {
        DuckDict data = new DuckDict();
        WriteSettingsData(data);
        File.WriteAllText(this.filepath, data.Jsonify());
    }

    public void SaveSettings() {
        DuckDict data = new DuckDict();
        WriteSettingsData(data);
        File.WriteAllText(this.filepath, data.Jsonify());
    }

    public void SaveOnExit() {
        // saves SOME save data, such as settings 
        SaveSettings();
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
