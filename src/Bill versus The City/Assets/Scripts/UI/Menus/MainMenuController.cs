using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    public UIDocument ui_doc;
    public string first_level = "Demo001--tutorial";

    private Button start_game_button, exit_game_button;

    // public GameObject hud, managers;

    void Start() {
        start_game_button = ui_doc.rootVisualElement.Q<Button>("StartGameButton");
        MenuManager.AddGenericEvents(start_game_button);
        start_game_button.clicked += StartGame;

        exit_game_button = ui_doc.rootVisualElement.Q<Button>("ExitGameButton");
        exit_game_button.clicked += ExitGame;
    }

    public void StartGame() {
        // hud.SetActive(true);
        // managers.SetActive(true);
        ScenesUtil.NextLevel(first_level);
    }

    public void ExitGame() {
        Debug.LogWarning("EXIT GAME CLICKED!"); // TODO --- remove debug
        ScenesUtil.ExitGame();
    }
}
