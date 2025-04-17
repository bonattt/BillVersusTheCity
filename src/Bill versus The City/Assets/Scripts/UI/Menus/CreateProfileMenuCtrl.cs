
using UnityEngine;
using UnityEngine.UIElements;

public class CreateProfileMenuCtrl : MonoBehaviour, ISubMenu {

    public UIDocument ui_doc;
    private TextField text_entry;
    private Button confirm_button, cancel_button;

    public ProfileEditMenuMode menu_mode = ProfileEditMenuMode.create;

    public SelectProfileMenuCtrl select_profile_menu;

    void Start() {
        // TODO --- 
        text_entry = ui_doc.rootVisualElement.Q<TextField>();
        if (menu_mode == ProfileEditMenuMode.rename) {
            text_entry.value = SaveProfile.GetProfileName((int) select_profile_menu.profile_selected);
        }
        
        confirm_button = ui_doc.rootVisualElement.Q<Button>("ConfirmButton");
        confirm_button.clicked += ConfirmClicked;
        
        cancel_button = ui_doc.rootVisualElement.Q<Button>("CancelButton");
        cancel_button.clicked += CancelClicked;
    }

    public void ConfirmClicked() {
        if (select_profile_menu.profile_selected == null) {
            Debug.LogError("cannot create profile for null!");
            return;
        }

        MenuManager.PlayMenuClick();


        if (menu_mode == ProfileEditMenuMode.create) {
            SaveProfile.Create((int) select_profile_menu.profile_selected, text_entry.value);
        }
        else if (menu_mode == ProfileEditMenuMode.rename) {
            SaveProfile.Rename((int) select_profile_menu.profile_selected, text_entry.value);
        }
        else {
            Debug.LogError($"unhandled ProfileEditMenuMode {menu_mode}");
        }

        select_profile_menu.UpdateProfileNames(); // TODO --- refactor to set `int profile_selected` instead of `select_profile_menu`
        MenuManager.inst.CloseMenu();
    }

    public void CancelClicked() {
        MenuManager.PlayMenuCancelClick();
        MenuManager.inst.CloseMenu();
    }

    public void MenuNavigation() {
        MenuManager.inst.DefaultMenuNavigation();
    }
}

public enum ProfileEditMenuMode {
    rename,
    create,
}