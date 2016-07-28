using UnityEngine;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;

public class GameManager : MonoBehaviour {
    public Capture Player;
    public Capture Opponent;
    public PauseMenu Menu;

    Transform Objects;
    TurnBasedMatch Match = null;
    MatchData Data = null;
    // True if current round is to be uploaded when finished
    public bool SubmitRound = false;

    void Start() {
        Match = ServicesManager.Match;
        Data = ServicesManager.Data;
        Objects = transform.Find("Objects");
        LaunchMatch();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // TODO actually pause
            Menu.Toggle();
        }
    }

    void LaunchMatch() {
        #if UNITY_EDITOR
        if (Data == null) {
            FightRound();
            return;
        }
        #endif
        // If there are no steps, it means we don't have a turn from the opponent
        // So we just straight to the Solo Round to make the first move
        if (Data.Steps == null) SoloRound();
        else FightRound();
    }

    // Round where the player plays alone to complete an objective
    void SoloRound() {
        Opponent.Reset();
        Player.Reset();
        Menu.Reset();
        SubmitRound = true;
        MakePlayerCollector();
        Opponent.gameObject.SetActive(false);
        foreach (Transform child in Objects) {
            child.gameObject.SetActive(true);
            child.tag = "Collectible";
        }
    }

    // Round where the player tries to stop the opponent
    void FightRound() {
        SubmitRound = false;
        Opponent.Steps = Data.Steps;
        Opponent.Replaying = true;
        MakeOpponentCollector();
    }

    void MakeOpponentCollector() {
        Debug.Log("Õpponent is collector");
        Player.gameObject.layer = 0;
        Opponent.gameObject.layer = 9;
    }

    void MakePlayerCollector() {
        Debug.Log("Player is collector");
        Player.gameObject.layer = 9;
        Opponent.gameObject.layer = 0;
    }

    string DecideNextToPlay() {
        if (Match.AvailableAutomatchSlots > 0) {
            // hand over to an automatch player
            return null;
        } else {
            // hand over to our (only) opponent
            Participant opponent = Util.GetOpponent(Match);
            return opponent == null ? null : opponent.ParticipantId;
        }
    }

    public void EndTurn() {
        if (SubmitRound) TakeTurn();
        else SoloRound();
    }

    string GetAdversaryParticipantId() {
        foreach (Participant p in Match.Participants) {
            if (!p.ParticipantId.Equals(Match.SelfParticipantId)) {
                return p.ParticipantId;
            }
        }
        Debug.LogError("Match has no adversary (bug)");
        return null;
    }

    void FinishMatch() {
        // bool winnerIsMe = mMatchData.Winner == mMyMark;
        bool winnerIsMe = false;

        // define the match's outcome
        MatchOutcome outcome = new MatchOutcome();
        outcome.SetParticipantResult(Match.SelfParticipantId,
            winnerIsMe ? MatchOutcome.ParticipantResult.Win : MatchOutcome.ParticipantResult.Loss);
        outcome.SetParticipantResult(GetAdversaryParticipantId(),
            winnerIsMe ? MatchOutcome.ParticipantResult.Loss : MatchOutcome.ParticipantResult.Win);

        // finish the match
        //SetStandBy("Sending...");
        PlayGamesPlatform.Instance.TurnBased.Finish(Match, Data.ToBytes(Player.Steps),
            outcome, (bool success) => {
                //EndStandBy();
                Debug.Log(success ? (winnerIsMe ? "YOU WON!" : "YOU LOST!") :
                "ERROR finishing match.");
            });
    }

    void TakeTurn() {
        //SetStandBy("Sending...");
        PlayGamesPlatform.Instance.TurnBased.TakeTurn(Match, Data.ToBytes(Player.Steps),
            DecideNextToPlay(), (bool success) => {
                //EndStandBy();
                Debug.Log(success ? "Turn taken" : "Error taking turn");
            });
    }

    public void Cancel() {
        PlayGamesPlatform.Instance.TurnBased.Cancel(Match,
            (bool success) => {
                //EndStandBy();
                Debug.Log(success ? "Cancelled" : "Error cancelling");
            });
    }

    public Collectible GetStageObject(int id) {
        Transform objects = transform.Find("Objects");
        foreach (Transform child in objects) {
            Collectible collectible = child.GetComponent<Collectible>();
            if (collectible.GetId() == id)
                return collectible;
        }
        Debug.LogError("No object found with ID " + id);
        return null;
    }
}
