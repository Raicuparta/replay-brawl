using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GooglePlayGames.BasicApi.Multiplayer;

public class StatsPanel : MonoBehaviour {
    const string StatusMyTurn = "Your turn";
    const string StatusTheirTurn = "Opponent's turn";
    const string StatusFinished = "Match has ended";
    Text Username;
    Text Times;
    Text Victories;
    Text Status;
    Button Start;
    public int Index;

    void Awake() {
        Username = transform.Find("Username").GetComponent<Text>();
        Times = transform.Find("Times").GetComponent<Text>();
        Victories = transform.Find("Victories").GetComponent<Text>();
        Status = transform.parent.Find("Status").GetComponent<Text>();
        Start = transform.parent.Find("Start").GetComponent<Button>();
    }

    void OnEnable() {
        Debug.Log("Enable stats panel");
        TurnBasedMatch match = ServicesManager.Match;
        MatchData data = ServicesManager.Data;
        Participant player = match.Participants[Index];
        Username.text = player.DisplayName;
        string newLine = System.Environment.NewLine;

        Times.text = "";
        for (int i = 0; i < data.FinishTimes.Count; i++) {
            float time = Mathf.Round(data.FinishTimes[i] * 100) / 100;
            if (i % 2 == Index) Times.text += time + newLine;
        }

        Victories.text = "";
        for (int i = 0; i < data.Victories.Count; i++) {
            bool victory = data.Victories[i];
            if (i % 2 == Index) Victories.text += (victory ? "V" : "X") + newLine;
        }

        Start.interactable = false;
        if (match.Status != TurnBasedMatch.MatchStatus.Active) {
            Status.text = StatusFinished;
            return;
        }

        if (match.TurnStatus == TurnBasedMatch.MatchTurnStatus.MyTurn) {
            Status.text = StatusMyTurn;
            Start.interactable = true;
        } else {
            Status.text = StatusTheirTurn;
            Start.interactable = false;
        }
    }
}
