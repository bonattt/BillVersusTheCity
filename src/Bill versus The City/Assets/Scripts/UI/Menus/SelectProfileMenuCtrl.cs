using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectProfileMenuCtrl : AbstractCloseEventMenu, ISubMenu
{
    public UIDocument ui_doc;
    private Button profile1, profile2, profile3;
    private Button select_button, cancel_button, rename_button, delete_button;

    private int? _profile_selected = null;
    public int? profile_selected {
        get {
            return _profile_selected;
        }
        set {
            if (value > ProfileButtons().ToList().Count || value <= 0) { throw new SelectProfileMenuCtrlException($"invalid profile selected {value}"); }
            _profile_selected = value;
            SelectProfileAndDeselectOthers(GetProfileButton(_profile_selected));
        }
    }

    protected IEnumerable<Button> ProfileButtons() {
        yield return profile1;
        yield return profile2;
        yield return profile3;
    }

    protected Button GetProfileButton(int? number) {
        switch (number) {
            case null:
                return null;
            case 1:
                return profile1;
            case 2:
                return profile2;
            case 3:
                return profile3;

            default:
                Debug.LogError($"unhandled profile number {number}!");
                return null;
        }
    }

    public static string GetProfileName(int profile_number) {
        string profile_name = SaveProfile.GetProfileName(profile_number);
        if (profile_name == null) {
            profile_name = "new save";
        }
        return profile_name;
    }

    public override void MenuNavigation() {
        MenuManager.inst.DefaultMenuNavigation();
    }

    public void UpdateProfileNames() {
        // sets the names on profile labels
        profile1.text = GetProfileName(1);
        profile2.text = GetProfileName(2);
        profile3.text = GetProfileName(3);
    }

    void Start()
    {
        profile1 = ui_doc.rootVisualElement.Q<Button>("save1");
        profile1.clicked += () => profile_selected = 1;
        
        profile2 = ui_doc.rootVisualElement.Q<Button>("save2");
        profile2.clicked += () => profile_selected = 2;

        profile3 = ui_doc.rootVisualElement.Q<Button>("save3");
        profile3.clicked += () => profile_selected = 3;

        profile_selected = SaveProfile.inst.profile_number;
        UpdateProfileNames();
        
        select_button = ui_doc.rootVisualElement.Q<Button>("SelectButton");
        select_button.clicked += SelectClicked;
        select_button.clicked += MenuManager.PlayMenuClick;
        
        cancel_button = ui_doc.rootVisualElement.Q<Button>("CancelButton");
        cancel_button.clicked += MenuManager.inst.CloseMenu;
        cancel_button.clicked += MenuManager.PlayMenuCancelClick;
        
        rename_button = ui_doc.rootVisualElement.Q<Button>("RenameButton");
        rename_button.clicked += RenameClicked;
        rename_button.clicked += MenuManager.PlayMenuClick;
        
        delete_button = ui_doc.rootVisualElement.Q<Button>("DeleteButton");
        delete_button.clicked += MenuManager.PlayMenuClick;
        delete_button.clicked += DeleteClicked;
    }

    protected void SelectProfileAndDeselectOthers(Button profile_button_selected) {
        foreach (Button profile_button in ProfileButtons()) {
            DeselectProfileButton(profile_button);
        }
        SelectProfileButton(profile_button_selected);
    }

    public void DeleteClicked() {
        if (profile_selected == null) { return; }
        int the_profile = (int) profile_selected;
        YesNoPopupController confirm_deletion = MenuManager.inst.OpenNewPopup();
        confirm_deletion.header_text = $"Delete Profile?";
        confirm_deletion.content_text = $"are you sure you want to delete the profile {GetProfileName((int) profile_selected)}? This cannot be undone.";
        confirm_deletion.confirm_button.clicked += () => ConfirmDeletion(the_profile);
    }
    public void ConfirmDeletion(int profile_deleted) {
        SaveProfile.DeleteProfile(profile_deleted);
        UpdateProfileNames();
    }

    public void RenameClicked() {
        SaveProfile.inst.profile_number = profile_selected;
        GameObject menu_obj = MenuManager.inst.OpenSubMenuPrefab(MenuManager.inst.create_new_save_profile_prefab);
        CreateProfileMenuCtrl new_menu = menu_obj.GetComponent<CreateProfileMenuCtrl>();
        new_menu.select_profile_menu = this;
        new_menu.menu_mode = ProfileEditMenuMode.rename;
    }

    public void SelectClicked() {
        SaveProfile.inst.profile_number = profile_selected;
        if (SaveProfile.inst.save_file.Exists()) {
            SaveProfile.inst.WriteCurrentProfileNumber();
            MenuManager.inst.CloseMenu();

        } else {
            GameObject menu_obj = MenuManager.inst.OpenSubMenuPrefab(MenuManager.inst.create_new_save_profile_prefab);
            CreateProfileMenuCtrl new_menu = menu_obj.GetComponent<CreateProfileMenuCtrl>();
            new_menu.select_profile_menu = this;
            new_menu.menu_mode = ProfileEditMenuMode.create;
        }
    }

    private void DeselectProfileButton(Button profile_button) {
        profile_button.RemoveFromClassList("profile_button_selected");
        profile_button.AddToClassList("profile_button_deselected");
    }

    private void SelectProfileButton(Button profile_button) {
        if (profile_button == null) {
            return; // do nothing, profile deselected
        }
        profile_button.RemoveFromClassList("profile_button_deselected");
        profile_button.AddToClassList("profile_button_selected");
    }

    private static string ClassesDisplay(VisualElement elem) {
        string display = $"Classes[";
        foreach (string claz in elem.GetClasses()) {
            display += $"{claz},";
        }
        return display + "]";
    }
}

[System.Serializable]
public class SelectProfileMenuCtrlException : System.Exception
{
    // public SelectProfileMenuCtrlException() { }
    public SelectProfileMenuCtrlException(string message) : base(message) { }
    // public SelectProfileMenuCtrlException(string message, System.Exception inner) : base(message, inner) { }
    // protected SelectProfileMenuCtrlException(
    //     System.Runtime.Serialization.SerializationInfo info,
    //     System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
