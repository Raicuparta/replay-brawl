using UnityEngine;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;

public class GameManager : MonoBehaviour {
    public Capture Player;
    public Capture Opponent;
    public PauseMenu Menu;

    private TurnBasedMatch Match = null;
    private MatchData Data = null;
    private string FinalMessage = null;

    private bool EndingTurn = false;

    // countdown to hide instructions
    private bool ShowInstructions = false;

    void Start() {
        Match = ServicesManager.Match;
        Data = ServicesManager.Data;
        LaunchMatch();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // TODO actually pause
            Menu.Toggle();
        }
    }

    private void Reset() {
        Match = null;
        Data = null;
        FinalMessage = null;
        EndingTurn = false;
        ShowInstructions = false;
        Opponent.gameObject.SetActive(true);
        Opponent.Reset();
        //Player.Reset();
    }

    void LaunchMatch() {
        if (Data.Steps != null) {
            Opponent.Steps = Data.Steps;
            Opponent.Replaying = true;
            MakeOpponentCollector();
        } else {
            Opponent.gameObject.SetActive(false);
        }
    }
    
    void MakeOpponentCollector() {
        Player.gameObject.layer = 0;
        Opponent.gameObject.layer = 9;
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
        EndingTurn = true;
        // do we have a winner?
        if (Data.HasWinner) {
            FinishMatch();
        } else {
            TakeTurn();
        }
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
