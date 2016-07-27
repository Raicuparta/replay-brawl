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
    bool FinishedTurn = false;
    [SerializeField]
    HealthManager Player;
    [SerializeField]
    HealthManager Opponent;
    [SerializeField]
    GameManager GameManager;

    Canvas Menu;
    CanvasScaler Scaler;

    void Awake() {
        Menu = GetComponent<Canvas>();
        Scaler = GetComponent<CanvasScaler>();
        Title = transform.Find("Title").GetComponent<Text>();
        DefaultTitle = Title.text;
        Menu.enabled = false;
        Scaler.enabled = false;
    }

    void FixedUpdate() {
        if (Player.IsDead() || Opponent.IsWinner()) Defeat();
        else if (Opponent.IsDead() || Player.IsWinner()) Victory();
    }

    void Show(string title) {
        Menu.enabled = true;
        Scaler.enabled = true;
        Title.text = title;
    }

    void Hide() {
        Time.timeScale = 1;
        if (!Player.IsDead() && !Opponent.IsDead()) {
            Menu.enabled = false;
            Scaler.enabled = false;
        }
    }

    public void Toggle() {
        if (Menu.enabled) Hide();
        else Pause();
    }

    public void Pause() {
        Time.timeScale = 0;
        Show(DefaultTitle);
    }

    public void Victory() {
        FinishedTurn = true;
        Show(VictoryTitle);
    }

    public void Defeat() {
        FinishedTurn = true;
        Show(DefeatTitle);
    }

    public void Exit() {
        if (FinishedTurn) {
            GameManager.EndTurn();
        } else {
            GameManager.Cancel();
        }
        SceneManager.LoadScene("Menu");
    }
}
