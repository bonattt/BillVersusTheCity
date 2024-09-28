using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    public GameObject pause_menu_prefab;

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

    protected void MenuNavigation() {
        if (open_menu == null) {
            if (InputSystem.current.PauseMenuInput()) {
                OpenSubMenuPrefab(pause_menu_prefab);
            }
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
    
    public void DefaultMenuNavigation() {
        if (InputSystem.current.MenuCancelInput()) {
            CloseMenu();
        }
    }

    public void OpenSubMenuPrefab(GameObject prefab) {
        GameObject new_menu = Instantiate(prefab) as GameObject;
        if (open_menu == null) {
            new_menu.transform.parent = transform;
        } else {
            new_menu.transform.parent = open_menu.transform;
        }
        sub_menus.Push(new_menu);
        paused = true;
    }

    public static void PlayMenuClick(ClickEvent _) {
        PlayMenuSound("menu_click");
    }

    public static void PlayMenuSound(string sound_name) {
        ISoundSet sound = SFXLibrary.LoadSound(sound_name);
        Vector3 target = Camera.main.transform.position;
        SFXSystem.instance.PlaySound(sound, target);
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

}
