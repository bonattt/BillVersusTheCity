using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    public GameObject pause_menu_prefab, yes_no_popup_prefab, settings_menu_prefab, dialogue_prefab, weapon_menu_prefab;

    private Stack<GameObject> sub_menus = new Stack<GameObject>();

    private static MenuManager _inst;
    public static MenuManager inst {
        get { return _inst; }
    }

    private bool _paused = false;
    public bool paused {
        get { return _paused; }
        set {
            _paused = value;
            if (_paused) {
                Time.timeScale = 0f;
            }
            else {
                Time.timeScale = 1f;
            }
            if (PlayerCharacter.inst != null) {
                // Debug.Log("!!!???");  // TODO --- remove debug, after figuring out what this does
                PlayerCharacter.inst.is_active = ! _paused;
            }
        }
    }

    public GameObject open_menu {
        // gets the current open menu, or returns null if no menus are open
        get {
            if (sub_menus.Count == 0) {
                return null;
            } else {
                return sub_menus.Peek();
            }
        }
    }

    void Awake() {
        _inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        paused = false;
    }

    public void CloseAllMenus() {
        while (sub_menus.Count > 0) {
            CloseMenu();
        }
        paused = false;
    }

    public static void CloseMenuClick(ClickEvent _) {
        MenuManager.inst.CloseMenu();
    }

    public void CloseMenu() {
        // closes the top menu in the sub-menu manager
        if (sub_menus.Count == 0) {
            Debug.LogWarning("CloseMenu called with no menus open!");
            return;
        }
        Destroy(sub_menus.Pop());

        if (sub_menus.Count == 0) {
            paused = false;
        }
    }

    void OnDestroy() {
        CloseAllMenus();
    }
        
    void Update()
    {
        MenuNavigation();
    }

    public void TryOpenPauseMenu() {
        if (InputSystem.current.PauseMenuInput()) {
            OpenSubMenuPrefab(pause_menu_prefab);
        }
    }

    protected void MenuNavigation() {
        if (open_menu == null) {
            TryOpenPauseMenu();
        }
        else {
            ISubMenu menu_ctrl = open_menu.GetComponent<ISubMenu>();
            if (menu_ctrl == null) {
                DefaultMenuNavigation();
            }
            else {
                menu_ctrl.MenuNavigation();
            }
        }
    }

    public DialogueController OpenDialoge(string file_name) {
        // Opens a new dialogue from a dialogue file
        GameObject obj = OpenSubMenuPrefab(dialogue_prefab);
        DialogueController ctrl = obj.GetComponent<DialogueController>();
        ctrl.StartDialogue(file_name);
        return ctrl;
    }
    
    public void DefaultMenuNavigation() {
        if (InputSystem.current.MenuCancelInput()) {
            CloseMenu();
        }
    }

    public GameObject OpenSubMenuPrefab(GameObject prefab) {
        // PRIMARY method to open a new menu. Takes a unity prefab and opens a menu from it.
        GameObject new_menu = Instantiate(prefab) as GameObject;
        new_menu.transform.parent = transform;
        sub_menus.Push(new_menu);
        paused = true;

        return new_menu;
    }

    public static void PlayMenuClick(ClickEvent _) {
        PlayMenuSound("menu_click");
    }

    public static void PlayMenuCancelClick(ClickEvent _) {
        PlayMenuSound("menu_click");
    }

    public static void PlayMenuSound(string sound_name) {
        ISoundSet sound = SFXLibrary.LoadSound(sound_name);
        Vector3 target = Camera.main.transform.position;
        SFXSystem.inst.PlaySound(sound, target);
    } 

    public static void AddGenericEvents(Button button) {
        button.RegisterCallback<ClickEvent>(PlayMenuClick);
    }

    public static void AddGenericEvents(Button[] buttons) {
        // adds events present on all buttons to a while array of Buttons
        foreach (Button b in buttons) {
            AddGenericEvents(b);
        }
    }

    public YesNoPopupController OpenNewPopup() {
        GameObject new_menu = OpenSubMenuPrefab(yes_no_popup_prefab);
        YesNoPopupController script = new_menu.GetComponent<YesNoPopupController>();
        script.Configure();
        return script;
    }

    public void PlayerDefeatPopup() {
        // click event for when the restart level button is clicked
        YesNoPopupController popup = OpenNewPopup();
        popup.header_text = "Defeat!";
        popup.content_text = "You have been killed";
        popup.confirm_text = "Restart Level";
        popup.reject_text = "Quit";
        popup.allow_escape = false;
        
        popup.confirm_button.clicked += ScenesUtil.RestartLevel;
        popup.cancel_button.clicked += PauseMenuController.ExitGame;  // TODO --- move this helper somewhere more appropriate
        popup.UpdateLabels();
    }

    public void WinGamePopup() {
        YesNoPopupController popup = OpenNewPopup();
        popup.header_text = "Victory!";
        popup.content_text = "You have defeated every enemy!";
        popup.confirm_text = "Restart Level";
        popup.reject_text = "Keep Playing";

        popup.confirm_button.clicked += ScenesUtil.RestartLevel;
        popup.UpdateLabels();
    }
}
