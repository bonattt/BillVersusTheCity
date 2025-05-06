

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
        ui_doc = GetComponent<UIDocument>();
        progress_dial = ui_doc.rootVisualElement.Q<RadialProgress>();
        string reload_ui_class = $"{style_prefix}_reload_progress";
        progress_dial.AddToClassList(reload_ui_class);
        progress_dial.Q<Label>().AddToClassList($"{reload_ui_class}_label");
        progress_dial.progress_label_type = RadialProgressText.custom_text;
        progress_dial.progress_label = "reload";

        reload_manager = reload_target.GetComponent<IReloadManager>();
        reload_manager.Subscribe(this);
        UpdateDialVisibility(reload_manager);
    }
    
    void Update() {
        if (reload_manager.reloading) {
            progress_dial.progress = 100 * reload_manager.reload_progress;
        }
    }

    void OnDestroy() {
        reload_manager.Unsubscribe(this);
    }

    private void UpdateDialVisibility(IReloadManager manager) {
        if (manager.reloading) {
            progress_dial.style.visibility = Visibility.Visible;
            progress_dial.progress = 100 * manager.reload_progress;
        } else {
            progress_dial.style.visibility = Visibility.Hidden;
        }
    }
    
    public void StartReload(IReloadManager manager, IWeapon weapon) {
        UpdateDialVisibility(manager);
    }
    public void ReloadFinished(IReloadManager manager, IWeapon weapon) {
        UpdateDialVisibility(manager);
    }
    public void ReloadCancelled(IReloadManager manager, IWeapon weapon) {
        UpdateDialVisibility(manager);
    }
}