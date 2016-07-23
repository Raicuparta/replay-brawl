using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    // Text that shows on top of the menu
    Text Title;
    const string VictoryTitle = "Victory!";
    const string DefeatTitle = "Defeat :(";
    string DefaultTitle;
    [SerializeField]
    HealthManager Player;
    [SerializeField]
    HealthManager Opponent;

    Canvas Menu;

    void Awake() {
        Menu = GetComponent<Canvas>();
        Title = transform.Find("Title").GetComponent<Text>();
        DefaultTitle = Title.text;
    }

    void FixedUpdate() {
        if (Player.IsDead()) Defeat();
        else if (Opponent.IsDead()) Victory();
    }

    void Show(string title) {
        Menu.enabled = true;
        Title.text = title;
    }

    void Hide() {
        Menu.enabled = false;
    }

    public void Toggle() {
        if (Menu.enabled) Hide();
        else Pause();
    }

    public void Pause() {
        Show(DefaultTitle);
    }

    public void Victory() {
        Show(VictoryTitle);
    }

    public void Defeat() {
        Show(DefeatTitle);
    }

    public void Exit() {
        SceneManager.LoadScene("Menu");
    }

    public void NextRound() {
        // TODO
    }
}
