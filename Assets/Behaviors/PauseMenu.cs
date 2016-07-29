﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour {
    // Text that shows on top of the menu
    Text Title;
    Text RoundText;
    Text Victories;
    const string VictoryTitle = "Victory!";
    const string DefeatTitle = "Defeat :(";
    const int MaxRounds = 6;
    string DefaultTitle;
    bool FinishedTurn = false;
    [SerializeField]
    HealthManager Player;
    [SerializeField]
    HealthManager Opponent;
    [SerializeField]
    GameManager Manager;

    Canvas Menu;
    CanvasScaler Scaler;

    void Awake() {
        Time.timeScale = 1;
        Menu = GetComponent<Canvas>();
        Scaler = GetComponent<CanvasScaler>();
        Title = transform.Find("Title").GetComponent<Text>();
        RoundText = transform.Find("Round").GetComponent<Text>();
        Victories = transform.Find("Victories").GetComponent<Text>();
        DefaultTitle = Title.text;
        Menu.enabled = false;
        Scaler.enabled = false;
    }

    void FixedUpdate() {
        if (Player.IsDead() || Opponent.IsWinner()) EndTurn(DefeatTitle);
        else if (Opponent.IsDead() || Player.IsWinner()) EndTurn(VictoryTitle);
    }

    public void Reset() {
        FinishedTurn = false;
    }

    void Show(string title) {
        Time.timeScale = 0;
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
        if (FinishedTurn) return;
        if (Menu.enabled) Hide();
        else Pause();
    }

    public void Pause() {
        Show(DefaultTitle);
        SetChildVisible("Next", false);
        SetChildVisible("Resume", true);
    }

    public void EndTurn(string title) {
        if (FinishedTurn) return;
        FinishedTurn = true;
        SetChildVisible("Next", !Manager.IsSoloRound);
        Manager.EndTurn();
        Show(title);
        SetChildVisible("Resume", false);
    }

    public void Exit() {
        if (!FinishedTurn) Manager.Cancel();
        SceneManager.LoadScene("Menu");
    }

    void SetChildVisible(string name, bool visible) {
        transform.Find(name).gameObject.SetActive(visible);
    }

    public void SetRound(int round) {
        RoundText.text = "Round " + round +" of " + MaxRounds;
    }

    public void SetVictories(List<bool> victories) {
        if (victories == null) return;
        Victories.text = "";
        foreach (bool hostWins in victories) {
            Victories.text += " " + (hostWins ? "H" : "O");
        }
    }
}
