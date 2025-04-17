
using UnityEngine;
using UnityEngine.UIElements;

public class CreateProfileMenuCtrl : MonoBehaviour, ISubMenu {

    public UIDocument ui_doc;
    private TextField text_entry;
    private Button confirm_button, cancel_button;

    public SelectProfileMenuCtrl select_profile_menu;

    void Start() {
        // TODO --- 
        text_entry = ui_doc.rootVisualElement.Q<TextField>();
        
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
        SaveProfile.Create((int) select_profile_menu.profile_selected, text_entry.value);
        select_profile_menu.UpdateProfileNames();
        // select_profile_menu.profile_selected = select_profile_menu.profile_selected; // reselect the save file

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