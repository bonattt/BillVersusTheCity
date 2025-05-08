

using UnityEngine;
using UnityEngine.UIElements;

public class HubTravelMenuCtrl : MonoBehaviour {
    public string next_scene;
    private UIDocument ui_doc;
    private Button next_level_button, travel_button, cancel_button;

    void Start() {
        ui_doc = GetComponent<UIDocument>();

        next_level_button = ui_doc.rootVisualElement.Q<Button>("NextLevelButton");
        travel_button = ui_doc.rootVisualElement.Q<Button>("TravelButton");
        cancel_button = ui_doc.rootVisualElement.Q<Button>("CancelButton");

        MenuManager.AddGenericEvents(next_level_button);
        MenuManager.AddGenericEvents(travel_button);
        MenuManager.AddGenericEvents(cancel_button);

        next_level_button.clicked += MenuManager.inst.CloseAllMenus;
        next_level_button.clicked += NextLevelClicked;
        travel_button.clicked += MenuManager.inst.CloseAllMenus;
        travel_button.clicked += HubTravelClicked;
        cancel_button.clicked += MenuManager.inst.CloseAllMenus;

    }

    public void NextLevelClicked() {
        LevelConfig.inst.CompleteLevel();
    }

    public void HubTravelClicked() {
        Debug.LogWarning("TODO --- implement hub-map travel");
    }


}