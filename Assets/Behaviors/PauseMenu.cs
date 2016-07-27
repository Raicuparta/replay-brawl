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
        Time.timeScale = 1;
        Menu = GetComponent<Canvas>();
        Scaler = GetComponent<CanvasScaler>();
        Title = transform.Find("Title").GetComponent<Text>();
        DefaultTitle = Title.text;
        Menu.enabled = false;
        Scaler.enabled = false;
    }

    void FixedUpdate() {
        if (Player.IsDead() || Opponent.IsWinner()) EndTurn(DefeatTitle);
        else if (Opponent.IsDead() || Player.IsWinner()) EndTurn(VictoryTitle);
    }

    void Show(string title) {
        Menu.enabled = true;
        Scaler.enabled = true;
        Title.text = title;
    }

    public void Hide() {
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

    public void EndTurn(string title) {
        FinishedTurn = true;
        GameManager.EndTurn();
        Show(title);
    }

    public void Exit() {
        if (!FinishedTurn) GameManager.Cancel();
        SceneManager.LoadScene("Menu");
    }
}
