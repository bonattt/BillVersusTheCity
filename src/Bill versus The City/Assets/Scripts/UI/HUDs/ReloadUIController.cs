

using UnityEngine;
using UnityEngine.UIElements;

public class ReloadUIController : MonoBehaviour, IReloadSubscriber {

    [Tooltip("prefix added to `_reload_progress` and `_reload_progress_label` as classes on the RadialProgress element and it's label.")]
    public string style_prefix = "test";
    private UIDocument ui_doc;
    private RadialProgress progress_dial;

    public GameObject reload_target;
    private IReloadManager reload_manager;

    void Start() {
        Debug.LogWarning($"ReloadUIController.Start()"); // TODO --- remove debug
        ui_doc = GetComponent<UIDocument>();
        progress_dial = ui_doc.rootVisualElement.Q<RadialProgress>();
        string reload_ui_class = $"{style_prefix}_reload_progress";
        progress_dial.AddToClassList(reload_ui_class);
        progress_dial.Q<Label>().AddToClassList($"{reload_ui_class}_label");
        progress_dial.progress_label_type = RadialProgressText.custom_text;
        progress_dial.progress_label = "alknsdl";

        reload_manager = reload_target.GetComponent<IReloadManager>();
        reload_manager.Subscribe(this);
        Debug.LogWarning($"subscribed! to {reload_target.name}'s {reload_manager}"); // TODO --- remove debug
    }
    
    void Update() {
        if (reload_manager.reloading) {
            progress_dial.progress = 100 * reload_manager.reload_progress;
        }
    }

    void OnDestroy() {
        reload_manager.Unsubscribe(this);
    }

    private void UpdateDialState(IReloadManager manager, IWeapon _) {
        if (manager.reloading) {
            Debug.LogWarning("Visible!"); // TODO --- remove debug
            progress_dial.style.visibility = Visibility.Visible;
            progress_dial.progress = 100 * manager.reload_progress;
        } else {
            Debug.LogWarning("Hidden!"); // TODO --- remove debug
            progress_dial.style.visibility = Visibility.Hidden;
        }
    }
    
    public void StartReload(IReloadManager manager, IWeapon weapon) {
        Debug.LogWarning("ReloadUIController.StartReload"); // TODO --- remove debug
        UpdateDialState(manager, weapon);
    }
    public void ReloadFinished(IReloadManager manager, IWeapon weapon) {
        Debug.LogWarning("ReloadUIController.ReloadFinished"); // TODO --- remove debug
        UpdateDialState(manager, weapon);
    }
    public void ReloadCancelled(IReloadManager manager, IWeapon weapon) {
        Debug.LogWarning("ReloadUIController.ReloadCancelled"); // TODO --- remove debug
        UpdateDialState(manager, weapon);
    }
}