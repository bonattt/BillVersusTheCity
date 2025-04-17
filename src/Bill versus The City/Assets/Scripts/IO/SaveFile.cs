using System.IO;

using UnityEngine;

public class SaveFile {

    public string save_name { get; protected set; }
    public const string SAVE_FILE_DIR = ".save_files";

    // public static SaveFile current_save = null;
    private string _profile_name = null;
    public string profile_name {
        get {
            if (_profile_name == null) {
                LoadProfile(); // if there is no profile name, try reading it from a file. If there is still no profile name, just use the save name.
                if (_profile_name == null) {
                    return save_name;
                }
                return _profile_name;
            }
            else {
                return _profile_name;
            }
        }
        set {
            _profile_name = value;
        }
    }

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

    public static void DeleteSave(string save_name) {
        new SaveFile(save_name).Delete();
    }

    protected void Delete(){
        if (! Exists()) {
            return; // nothing to delete
        }
        File.Delete(filepath);
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

    public static bool SaveExists(string save_name) {
        return (new SaveFile(save_name)).Exists();
    }

    public bool Exists() {
        return File.Exists(filepath);
    }

    private void WriteSettingsData(DuckDict data) {
        data.SetObject("settings", JsonParser.ReadAsDuckDict(GameSettings.inst.AsJson()));
    }

    private void WriteProfileData(DuckDict data) {
        DuckDict profile_data = new DuckDict();
        profile_data.SetString("profile_name", profile_name);
        data.SetObject("profile", profile_data);
    }

    public void SaveAll() {
        DuckDict data = new DuckDict();
        WriteSettingsData(data);
        WriteProfileData(data);
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
        
        LoadProfileFromDuckDict(data);
    } 

    private void LoadProfile() {
        string json_text = File.ReadAllText(this.filepath);
        DuckDict data = JsonParser.ReadAsDuckDict(json_text);
        LoadProfileFromDuckDict(data);
    }

    private void LoadProfileFromDuckDict(DuckDict data) {
        if (!data.ContainsKey("profile")) return;
        DuckDict profile_data = data.GetObject("profile");

        if (!profile_data.ContainsKey("profile_name")) return;
        profile_name = profile_data.GetString("profile_name");
    }

    public DuckDict AsDuckDict() {
        string file_text = File.ReadAllText(this.filepath);
        DuckDict data = JsonParser.ReadAsDuckDict(file_text);
        return data;
    }

    public void LoadFromFile() {
        // reads the save file and loads it's json data
        string file_text = File.ReadAllText(this.filepath);
        this.LoadFromJson(file_text);
    }
}
