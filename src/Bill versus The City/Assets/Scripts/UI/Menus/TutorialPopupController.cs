using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using UnityEngine;
using UnityEngine.UIElements;



public class TutorialPopupController : AbstractCloseEventMenu
{
    public string tutorial_name, header_text, tutorial_text;
    public UIDocument ui_doc;
    private Label tutorial_label, header_label;
    private Button okay_button;

    private Toggle skip_this_tutorial, skip_all_tutorials;

    private bool initialized = false;

    void Start() {
        VisualElement root = ui_doc.rootVisualElement;
        tutorial_label = root.Q<Label>("TutorialLabel");
        header_label = root.Q<Label>("HeaderLabel");

        okay_button = root.Q<Button>();
        okay_button.clicked += CloseMenu;
        initialized = true;

        skip_this_tutorial = root.Q<Toggle>("DontShowAgain");
        skip_all_tutorials = root.Q<Toggle>("SkipAllTutorials");

        UpdateUI();
    }

    public void ApplyConfig(TutorialConfig config) {
        tutorial_name = config.tutorial_name;
        tutorial_text = config.tutorial_text;
        header_text = config.header_text;
    }

    public void UpdateUI() {
        if (!initialized) { return; }  // don't update if UI Document not parsed yet.
        header_label.text = header_text;
        tutorial_label.text = tutorial_text;
    }


    public override void MenuNavigation()
    {
        if (InputSystem.current.MenuCancelInput()) {
            CloseMenu();
        }
    }

    public void CloseMenu() {
        if (skip_this_tutorial.value) {
            GameSettings.inst.general_settings.skipped_tutorials.Add(tutorial_name);
        }
        if (skip_all_tutorials.value) {
            GameSettings.inst.general_settings.skip_all_tutorials = true;
        }
        SaveProfile.inst.save_file.SaveSettings(); // skip-tutorials is stored in settings
        MenuManager.inst.CloseMenu();
    }
}


public struct TutorialConfig {
    // struct for containing tutorial data
    public string tutorial_name, header_text, tutorial_text;

    public TutorialConfig(string tutorial_name, string header_text, string tutorial_text) {
        this.tutorial_name = tutorial_name;
        this.header_text = header_text; 
        this.tutorial_text = tutorial_text;
    }

    public TutorialConfig(string tutorial_name, string tutorial_text) {
        this.tutorial_name = tutorial_name;
        this.header_text = $"Tutorial: {ToTitleCase(tutorial_name)}"; 
        this.tutorial_text = tutorial_text;
    }

    private static string ToTitleCase(string str) {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(str.ToLower());;
    }
}
