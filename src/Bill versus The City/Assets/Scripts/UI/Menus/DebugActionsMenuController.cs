using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugActionsMenuController : MonoBehaviour {

    public UIDocument ui_doc;
    private VisualElement content_div;

    public const string SKIP_LEVEL = "Skip Level";
    public const string FAIL_LEVEL  = "Fail Level";
    public const string COMPLETE_OBJECTIVE  = "Complete Objective";
    public const string KILL_ENEMIES = "Kill Enemies";
    public const string GET_DOLLARS_100 = "Get $100";
    public const string GET_DOLLARS_1000 = "Get $1000";
    public const string KILL_PLAYER = "Kill Player (NOT IMPLEMENTED)";
    public const string RESET_HEALTH = "Reset Health (NOT IMPLEMENTED)";
    public const string RESET_GAME  = "Reset Game (NOT IMPLEMENTED)";

    private Dictionary<string, Action> GetCallbacks() {
        // can't set callbacks in a dict on the class, because I need to reference methods of this class
        return new Dictionary<string, Action>{
            {SKIP_LEVEL, SkipLevelClicked},
            {KILL_ENEMIES, KillEnemiesClicked},
            {KILL_PLAYER, KillPlayerClicked},
            {RESET_HEALTH, ResetHealthClicked},
            {GET_DOLLARS_100, CurriedGetDollars(100)},
            {GET_DOLLARS_1000, CurriedGetDollars(1000)},
            {RESET_GAME, ResetGameClicked},
            {FAIL_LEVEL, FailLevelClicked},
            {COMPLETE_OBJECTIVE, CompleteObjectiveClicked},
        };
    }

    private Action CurriedGetDollars(int dollars_amount) {
        Debug.LogWarning($"Get {dollars_amount} action!");
        return () => {
            // todo
            PlayerCharacter.inst.inventory.dollars_change_in_level += dollars_amount;
        };
    }

    void Start() {
        VisualElement root = ui_doc.rootVisualElement;
        content_div = root.Q<VisualElement>("Content");
        content_div.Clear();

        Dictionary<string, Action> callbacks = GetCallbacks();
        foreach (string action_name in callbacks.Keys) {
            content_div.Add(GetNewAction("", action_name, callbacks[action_name]));
        }

        Button cancel_button = root.Q<Button>("CancelButton");
        cancel_button.clicked += CloseClicked;
    }

    private VisualElement GetNewAction(string label_text, string button_text, Action callback) {
        Debug.LogWarning($"GetNewAction({button_text})"); // TODO --- remove debug
        VisualElement action_div = new VisualElement();
        action_div.AddToClassList("DebugAction");

        Label action_label = new Label();
        action_label.text = label_text;
        action_div.Add(action_label);

        Button action_button = new Button();
        action_button.text = button_text;
        action_button.clicked += callback;
        action_div.Add(action_button);

        return action_div;
    }

    public void SkipLevelClicked() {
        Close();
        Debug.LogWarning("SkipLevelClicked");
        LevelConfig.inst.CompleteLevel();
    }

    public void FailLevelClicked() {
        Close();
        Debug.LogWarning("FailLevelClicked");
        LevelConfig.inst.FailLevel();
    }

    public void CompleteObjectiveClicked() {
        Close();
        Debug.LogWarning("CompleteObjectiveClicked");
        LevelConfig.inst.LevelObjectivesCleared();
    }

    public void KillEnemiesClicked() {
        Close();
        Debug.LogWarning("KillEnemiesClicked");
        EnemiesManager.inst.DebugKillAll();
    }

    public void KillPlayerClicked() {
        Close();
        Debug.LogWarning("KillPlayerClicked: NOT IMPLEMENTED!");
    }

    public void ResetHealthClicked() {
        Close();
        Debug.LogWarning("ResetHealthClicked: NOT IMPLEMENTED!");
    }

    public void ResetGameClicked() {
        Close();
        Debug.LogWarning("ResetGameClicked: NOT IMPLEMENTED!");
    }

    public void CloseClicked() {
        Close();
    }

    private void Close() {
        MenuManager.inst.CloseMenu();
    }

}