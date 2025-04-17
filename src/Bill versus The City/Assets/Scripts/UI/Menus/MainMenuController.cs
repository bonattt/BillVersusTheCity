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

    private Button start_game_button, demo_level_button, dev_room_button, settings_button, change_profile_button, exit_game_button;
    private Label profile_label;

    // public GameObject hud, managers;

    void Start() {
        MenuManager.DisableHUD();

        start_game_button = ui_doc.rootVisualElement.Q<Button>("StartGameButton");
        MenuManager.AddGenericEvents(start_game_button);
        start_game_button.clicked += StartGame;

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

    public void StartGame() {
        // hud.SetActive(true);
        // managers.SetActive(true);
        ScenesUtil.NextLevel(first_level);
        MenuManager.EnableHUD();
    }

    private string GetDemoLevel() {
        if (demo_level == null || "".Equals(demo_level)) {
            Debug.LogWarning("build has no demo-level!");
            return first_level;
        }
        return demo_level;
    }

    public void DemoLevelButtonClicked() {
        ScenesUtil.NextLevel(GetDemoLevel());
        MenuManager.EnableHUD();
    }

    public void DevRoomButtonClicked() {
        ScenesUtil.NextLevel(dev_room_level);
        MenuManager.EnableHUD();
    }

    public void ExitGameClicked() {
        Debug.LogWarning("EXIT GAME CLICKED!"); // TODO --- remove debug
        ScenesUtil.ExitGame();
    }
}
