using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    public bool always_select_profile = false;
    public UIDocument ui_doc;
    public string first_level = "Demo001--tutorial";
    public string demo_level = "";
    public string dev_room_level = "Demo001--test";

    private Button new_game_button, continue_game_button, demo_level_button, dev_room_button, settings_button, change_profile_button, exit_game_button;
    private Label profile_label;
    private GameObject hud;

    // public GameObject hud, managers;

    void Start() {
        OpenMainMenu();

        new_game_button = ui_doc.rootVisualElement.Q<Button>("StartNewGameButton");
        MenuManager.AddGenericEvents(new_game_button);
        new_game_button.clicked += StartNewGameClicked;

        continue_game_button = ui_doc.rootVisualElement.Q<Button>("ContinueGameButton");
        MenuManager.AddGenericEvents(continue_game_button);
        continue_game_button.clicked += ContinueGameClicked;
        if (SaveDataExists()) {
            continue_game_button.SetEnabled(true);
        } else {
            continue_game_button.SetEnabled(false);
        }

        demo_level_button = ui_doc.rootVisualElement.Q<Button>("DemoLevelButton");
        demo_level_button.clicked += DemoLevelButtonClicked;

        dev_room_button = ui_doc.rootVisualElement.Q<Button>("DevRoomButton");
        dev_room_button.clicked += DevRoomButtonClicked;

        settings_button = ui_doc.rootVisualElement.Q<Button>("SettingsButton");
        settings_button.clicked += () => MenuManager.inst.OpenSubMenuPrefab(MenuManager.inst.settings_menu_prefab);

        change_profile_button = ui_doc.rootVisualElement.Q<Button>("ChangeProfileButton");
        change_profile_button.clicked += MenuManager.PlayMenuClick;
        change_profile_button.clicked += OpenProfileSelection;

        exit_game_button = ui_doc.rootVisualElement.Q<Button>("ExitGameButton");
        exit_game_button.clicked += ExitGameClicked;

        profile_label = ui_doc.rootVisualElement.Q<Label>("ProfileLabel");
        if (always_select_profile || SaveProfile.inst.save_file == null || SaveProfile.inst.profile_number == null) {
            SaveProfile.SetupDirectory();
            OpenProfileSelection();
        }
        UpdateProfileLabel();
    }

    public void UpdateProfileLabel() {
        string profile_name;
        if (SaveProfile.inst.save_file == null || SaveProfile.inst.profile_number == null) {
            SaveProfile.SetupDirectory();
            profile_name = "N/A";
        }
        else {
            profile_name = SaveProfile.inst.save_file.profile_name;
        }
        profile_label.text = $"Profile: {profile_name}";
    }

    public void OpenProfileSelection() {
        Debug.LogWarning("TODO: implement OpenProfileSelection!"); // TODO --- implement
        GameObject new_menu = MenuManager.inst.OpenSubMenuPrefab(MenuManager.inst.select_save_file_prefab);
        SelectProfileMenuCtrl select_profile_menu = new_menu.GetComponent<SelectProfileMenuCtrl>();
        select_profile_menu.AddCloseCallback(new SimpleActionEvent(UpdateProfileLabel));
    }

    public DuckDict LoadProgressData() {
        return SaveProfile.inst.save_file.AsDuckDict().GetObject("progress");
    }

    public bool SaveDataExists() {
        // returns true if a game has already been started on a given profile.
        // DuckDict progress_data = LoadProgressData();
        // Debug.LogWarning("`progress_data != null` is not the correct check for `SaveDataExists()`"); // TODO --- 
        // Debug.LogWarning("refactor: move `SaveDataExists()` to SaaveFile"); // TODO --- refactor: move `SaveDataExists()` to SaveFile
        return SaveProfile.inst.save_file.IsGameStarted(); // progress_data != null; 
    }

    public void StartNewGameClicked() {
        if (SaveDataExists()) {
            YesNoPopupController popup = MenuManager.inst.OpenNewPopup();
            popup.header_text = "Erase Progress?";
            popup.content_text = "Starting a new game will erase your progress on this profile, are you sure? (this cannot be undone!)";
            popup.UpdateLabels();
            popup.confirm_button.clicked += StartNewGameConfirmed;
        } else {
            StartNewGameConfirmed();
        }
    }

    public void StartNewGameConfirmed() {
        CloseMainMenu();
        PlayerCharacter.inst.StartNewGame();
        ScenesUtil.NextLevel(first_level);
        // SetCallback(0.5f, () => ScenesUtil.NextLevel(first_level)); // TODO --- remove debug

    }

    public void ContinueGameClicked() {
        // TODO
        Debug.LogWarning("Continue not (fully) implemented!");
        CloseMainMenu();
        DuckDict progress_data = SaveProfile.inst.save_file.AsDuckDict().GetObject("progress");
        PlayerCharacter.inst.LoadProgress(progress_data);
        string scene_name = progress_data.GetObject("level").GetString("current_scene");
        Debug.Log($"continue game at scene '{scene_name}"); // TODO --- remove debug
        ScenesUtil.NextLevel(scene_name); // TODO --- load and store current level 
    }

    private string GetDemoLevel() {
        if (demo_level == null || "".Equals(demo_level)) {
            Debug.LogWarning("build has no demo-level!");
            return first_level;
        }
        return demo_level;
    }

    public void DemoLevelButtonClicked() {
        CloseMainMenu();
        ScenesUtil.NextLevel(GetDemoLevel());
        // SetCallback(0.5f, () => ScenesUtil.NextLevel(GetDemoLevel())); // TODO --- remove debug
    }

    public void DevRoomButtonClicked() {
        CloseMainMenu();
        ScenesUtil.NextLevel(dev_room_level);
        // SetCallback(0.5f, () => ScenesUtil.NextLevel(dev_room_level)); // TODO --- remove debug
    }

    public void ExitGameClicked() {
        ScenesUtil.ExitGame();
    }

    private void CloseMainMenu() {
        hud = MenuManager.EnableHUD();
        MenuManager.inst.disable_pause_menu = false;
    }

    private void OpenMainMenu() {
        hud = MenuManager.DisableHUD();
        MenuManager.inst.disable_pause_menu = true;
    }

    private float callback_time = float.PositiveInfinity; // TODO --- remove debug
    private Action Callback = null; // TODO --- remove debug
    private void SetCallback(float delay, Action cb) { // TODO --- remove debug
        Callback = cb;
        callback_time = Time.time + delay;
    } // TODO --- remove debug

    void Update() { // TODO --- remove debug
        if (callback_time <= Time.time) {
            callback_time = float.PositiveInfinity;
            Callback();
            Callback = null;
        } 
    } // TODO --- remove debug

}
