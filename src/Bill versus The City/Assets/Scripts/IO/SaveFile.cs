using System.IO;

using UnityEngine;

public class SaveFile {

    public string save_name { get; protected set; }
    public const string SAVE_FILE_DIR = ".save_files";

    public bool IsGameStarted() {
        // retuns true if a game has already been started on this save. (progress.level != null)
        DuckDict data = AsDuckDict();
        if (!data.ContainsKey("progress")) { return false; }
        DuckDict progress_data = data.GetObject("progress");
        if (progress_data == null || !progress_data.ContainsKey("level")) { return false; }
        return true;
    }

    // public static SaveFile current_save = null;
    private string _profile_name = null;
    public string profile_name {
        get {
            if (_profile_name == null) {
                // if there is no profile name, try reading it from a file. If there is still no profile name, just use the save name.
                LoadProfile();
            }
            // if profile is still null after loading, return `save_name`
            if (_profile_name == null) {
                _profile_name = save_name;
            }
            return _profile_name;
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
        return new SaveFile(save_name).Exists();
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

    private void WriteProgressData(DuckDict data, string current_scene, string next_scene) {
        DuckDict progress_data = data.GetObject("progress");
        if (progress_data == null) {
            progress_data = new DuckDict();
            data.SetObject("progress", progress_data);
        }
        WriteInventoryProgressData(progress_data);
        WriteLevelProgressData(progress_data, current_scene, next_scene);
    }

    private void WriteInventoryProgressData(DuckDict progress_data) {
        progress_data.SetObject("inventory", PlayerCharacter.inst.inventory.GetProgressData());
    }
    public void WriteLevelProgressData(DuckDict progress_data, string current_scene, string next_scene) {
        DuckDict level_data = new DuckDict();
        level_data.SetString("current_scene", current_scene);
        level_data.SetString("next_scene", next_scene);
        progress_data.SetObject("level", level_data);
    }

    public void SaveAll() {
        float save_start_time = Time.time;
        DuckDict data = AsDuckDict();
        WriteSettingsData(data);
        WriteProfileData(data);
        WriteInventoryProgressData(data);

        File.WriteAllText(this.filepath, data.Jsonify());

        float save_end_time = Time.time;
        Debug.Log($"SaveAll execution took {save_end_time - save_start_time} seconds");
    }

    // public void SaveProgress() => SaveProgress(ScenesUtil.GetCurrentSceneName());
    public void SaveProgress(string current_scene) => SaveProgress(current_scene, null);
    public void SaveProgress(string current_scene, string next_scene) {
        // TODO --- 
        DuckDict data = AsDuckDict();
        WriteProgressData(data, current_scene, next_scene);
        File.WriteAllText(filepath, data.Jsonify());
    }


    public void SaveSettings() {
        float save_start_time = Time.time;
        DuckDict data = AsDuckDict();
        WriteSettingsData(data);
        WriteProfileData(data);
        File.WriteAllText(this.filepath, data.Jsonify());
        
        float save_end_time = Time.time;
        Debug.Log($"SaveSettings execution took {save_end_time - save_start_time} seconds");
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
        GameSettings.inst.LoadFromJson(data);
        
        LoadProfileFromDuckDict(data);
    } 

    private void LoadProfile() {
        try {
            string json_text = File.ReadAllText(this.filepath);
            DuckDict data = JsonParser.ReadAsDuckDict(json_text);
            LoadProfileFromDuckDict(data);
        } catch (IOException) {
            _profile_name = null;
        }
    }

    private void LoadProfileFromDuckDict(DuckDict data) {
        if (!data.ContainsKey("profile")) return;
        DuckDict profile_data = data.GetObject("profile");

        if (!profile_data.ContainsKey("profile_name")) return;
        profile_name = profile_data.GetString("profile_name");
    }

    public DuckDict AsDuckDict() {
        DuckDict data;
        try {
            string file_text = File.ReadAllText(this.filepath);
            data = JsonParser.ReadAsDuckDict(file_text);
        } catch (IOException e) {
            Debug.LogWarning($"save file '{save_name}' not found at '{filepath}' ({e}).");
            data = new DuckDict();
            // DuckDict error_data = new DuckDict();
            // error_data.SetString("error", $"{e.GetType()}");
            // // string stack_trace = e.StackTrace;
            // // stack_trace = stack_trace.Replace("\n", "\\n");
            // error_data.SetString("stack_trace", "TODO");
            // data.SetObject("error", error_data);
        }
        return data;
    }

    public void LoadFromFile() {
        // reads the save file and loads it's json data
        string file_text = File.ReadAllText(this.filepath);
        this.LoadFromJson(file_text);
    }
}
