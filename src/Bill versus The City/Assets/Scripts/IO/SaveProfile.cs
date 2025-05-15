

using System.IO;
using UnityEngine;

public class SaveProfile {
    // public const string SAVE_SLOT_1 = "save_1";
    public const string PROFILE_FILE = ".save_files\\profile";
    private static SaveProfile _inst;
    public static SaveProfile inst {
        get {
            if (_inst == null) {
                _inst = new SaveProfile();
            }
            return _inst;
        }
    }

    private int? _profile_number = null;
    public int? profile_number {
        get {
            return _profile_number;
        }
        set {
            _profile_number = value;
            if (_profile_number == null) {
                save_file = null;
            }
            else {
                save_file = new SaveFile(GetSaveName((int) _profile_number));
            }
        }
    }
    public SaveFile save_file { get; private set; }

    public static string GetSaveName(int slot_number) {
        return $"save_{slot_number}";
    }

    protected static string profile_filepath {
        get {
            return Path.Join(Application.dataPath, PROFILE_FILE);;
        }
    }

    public static void Create(int profile_slot, string profile_name) {
        SaveFile save = new SaveFile(GetSaveName(profile_slot));
        save.profile_name = profile_name;
        save.SaveAll();
    }

    public static void Rename(int profile_slot, string profile_name) {
        SaveFile save = new SaveFile(GetSaveName(profile_slot));
        save.LoadFromFile();
        save.profile_name = profile_name;
        save.SaveAll();
    }

    public static bool ProfileExists(int? profile_number) {
        if (profile_number == null) { return false; }
        return ProfileExists((int) profile_number);
    }
    public static bool ProfileExists(int profile_number) {
        string save_name = GetSaveName(profile_number);
        return SaveFile.SaveExists(save_name);
    }

    public static string GetProfileName(int profile_number) {
        SaveFile save = new SaveFile(GetSaveName(profile_number));
        if (! save.Exists()) {
            return null;
        }
        return save.profile_name;
    }


    public SaveProfile() {
        profile_number = ReadCurrentProfileNumber();
    }

    public static int? ReadCurrentProfileNumber() {
        // reads the current profile number from file
        try {
            string file_text = File.ReadAllText(profile_filepath);
            return (int?) int.Parse(file_text);
        } catch(FileNotFoundException) {
            return null; 
        } catch(DirectoryNotFoundException) {
            return null;
        }
    }

    public string LoadSaveData() {
        // loads save data, and returns the current scene name
        GameSettings.inst.LoadFromJson(save_file.AsDuckDict().GetObject("settings"));

        DuckDict progress_data = save_file.AsDuckDict().GetObject("progress");
        PlayerCharacter.inst.LoadProgress(progress_data);
        string scene_name = progress_data.GetObject("level").GetString("current_scene");
        return scene_name;
    }
    
    public static void SetupDirectory() {
        SaveFile.SetupDirectory();
    }

    public void WriteCurrentProfileNumber() {
        File.WriteAllText(profile_filepath, $"{profile_number}");
    }

    public static void DeleteProfile(int profile_number) {
        SaveFile.DeleteSave(GetSaveName(profile_number));
    }

    // public static void Initialize() {
        
    //     SaveFile.SetupDirectory();
    //     // sets and mutates settings singleton
    //     try {
    //         SaveFile current_save = SaveFile.Load(SaveProfile.GetSaveName(1));
    //     } catch (FileNotFoundException) {
    //         Debug.LogWarning($"no save at {SaveProfile.GetSaveName(1)}, creating a new save");
    //         // DirectoryNotFoundException: Could not find a part of the path "C:\MY-documents\git-repos\BillVersusTheCity\src\Bill versus The City\Build\Bill versus The City_Data\.save_files\save_1".
    //         SaveProfile.GetSaveName(1) = SaveFile.NewSave(SaveProfile.GetSaveName(1));
    //     }

    // }
}