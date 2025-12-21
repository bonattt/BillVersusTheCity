

using UnityEngine;

public interface IMenuManager {
    // manages sub menus
}

public interface ISubMenu {
    // managed by Menu Manager
    public GameObject menuObject { get; }
    public void MenuNavigation();
}