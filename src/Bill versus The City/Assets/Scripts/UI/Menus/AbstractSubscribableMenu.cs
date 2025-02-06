
using System.Collections.Generic;

using UnityEngine;

public interface ICloseEventMenu {
    public void AddCloseCallback(IGameEventEffect callback);
    public void ResolveMenuClosedCallbacks();
}

public abstract class AbstractCloseEventMenu : MonoBehaviour, ISubMenu, ICloseEventMenu {
    // Abstract ISubMenu class that supports callbacks when the menu is closed
    private List<IGameEventEffect> callbacks = new List<IGameEventEffect>();
    public void AddCloseCallback(IGameEventEffect new_callback) => callbacks.Add(new_callback);
    public abstract void MenuNavigation();

    public void ResolveMenuClosedCallbacks() {
        Debug.Log($"{gameObject.name}.ResolveMenuClosedCallbacks"); // TODO --- remove debug
        foreach (IGameEventEffect cb in callbacks) {
            cb.ActivateEffect();
        }
    }

} 