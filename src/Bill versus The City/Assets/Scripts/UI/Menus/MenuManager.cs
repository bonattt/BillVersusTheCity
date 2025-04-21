using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    public GameObject debug_action_menu_prefab, dialogue_prefab, pause_menu_prefab, settings_menu_prefab, tutorial_popup_prefab, 
            weapon_menu_prefab, yes_no_popup_prefab, select_save_file_prefab, create_new_save_profile_prefab;

    private Stack<GameObject> sub_menus = new Stack<GameObject>();

    public bool disable_pause_menu = false;

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
        paused = sub_menus.Count != 0;
    }

    public static GameObject DisableHUD() {
        Debug.LogWarning($"REFACTOR NEEDED: extract DisableHUD to a HUD management class, which should not be individual to a scene, not static!"); // TODO --- 
        GameObject hud = GetHUD();
        if (hud != null) {
            hud.SetActive(false);
        }
        return hud;
    }

    public static GameObject EnableHUD() {
        Debug.LogWarning($"REFACTOR NEEDED: extract EnableHUD to a HUD management class, which should not be individual to a scene, not static!"); // TODO --- 
        GameObject hud = GetHUD();
        if (hud != null) {
            hud.SetActive(true);
        }
        return hud;
    } 

    private static GameObject GetHUD() {
        return GameObject.Find("HUD-UI(Clone)");
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
        GameObject sub_menu = sub_menus.Pop();
        ICloseEventMenu callback_menu = sub_menu.GetComponent<ICloseEventMenu>();
        if (callback_menu != null) {
            callback_menu.ResolveMenuClosedCallbacks();
        }
        Destroy(sub_menu);

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
        if (disable_pause_menu) { return; }
        else if (InputSystem.current.PauseMenuInput()) {
            OpenSubMenuPrefab(pause_menu_prefab);
        }
        else if (GameSettings.inst.debug_settings.allow_debug_actions && InputSystem.current.DebugInput()) {
            OpenSubMenuPrefab(debug_action_menu_prefab);
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

    public TutorialPopupController OpenTutorial(TutorialConfig tutorial_config) {
        // NOTE: this method should be called through TutorialLibrary.OpenTutorial method, which managed more tutorial stuff
        GameObject menu = OpenSubMenuPrefab(tutorial_popup_prefab);
        TutorialPopupController tutorial = menu.GetComponent<TutorialPopupController>();
        tutorial.ApplyConfig(tutorial_config);
        return tutorial;
    }

    private static int dialogue_counter = 0;
    public DialogueController OpenDialoge(string file_name) {
        // Opens a new dialogue from a dialogue file
        GameObject obj = OpenSubMenuPrefab(dialogue_prefab);
        obj.name = $"Dialogue {++dialogue_counter} ({file_name})";
        DialogueController ctrl = obj.GetComponent<DialogueController>();
        ctrl.StartDialogue(file_name);
        return ctrl;
    }

    public GameObject OpenWeaponSelectMenu() {
        GameObject menu = OpenSubMenuPrefab(weapon_menu_prefab);
        return menu;
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

    public static void PlayMenuClick(ClickEvent _) => PlayMenuClick();
    public static void PlayMenuClick() {
        PlayMenuSound("menu_click");
    }

    public static void PlayMenuCancelClick(ClickEvent _) => PlayMenuCancelClick();
    public static void PlayMenuCancelClick() {
        PlayMenuSound("menu_click");
    }

    public static void PlayMenuSound(string sound_name) {
        ISounds sound = SFXLibrary.LoadSound(sound_name);
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
        CloseAllMenus();
        YesNoPopupController popup = OpenNewPopup();
        popup.header_text = "Defeat!";
        popup.content_text = "You have been killed";
        popup.confirm_text = "Restart Level";
        popup.reject_text = "Quit";
        popup.allow_escape = false;
        
        popup.confirm_button.clicked += ScenesUtil.RestartLevel;
        popup.cancel_button.clicked += ScenesUtil.ExitToMainMenu;  // TODO --- move this helper somewhere more appropriate
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
