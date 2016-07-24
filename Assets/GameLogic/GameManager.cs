using UnityEngine;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;

public class GameManager : MonoBehaviour {
    public Capture Player;
    public Capture Opponent;
    public PauseMenu Menu;

    private TurnBasedMatch mMatch = null;
    private MatchData mMatchData = null;
    private string mFinalMessage = null;

    private bool mEndingTurn = false;

    // countdown to hide instructions
    private bool mShowInstructions = false;

    void Start() {
        mMatch = ServicesManager.Match;
        mMatchData = ServicesManager.Data;
        SetupObjects();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // TODO actually pause
            Menu.Toggle();
        }
    }

    private void Reset() {
        mMatch = null;
        mMatchData = null;
        mFinalMessage = null;
        mEndingTurn = false;
        mShowInstructions = false;
        Opponent.gameObject.SetActive(true);
        Opponent.Reset();
        //Player.Reset();
    }

    private void SetupObjects() {
        string replay = mMatchData.Replay;
        Debug.Log("Setup Objects");
        if (replay != null) {
            Debug.Log("Read opponent data");
            Opponent.ReadFromString(mMatchData.Replay);
            Opponent.Replaying = true;
        } else {
            Opponent.gameObject.SetActive(false);
        }
    }

    string DecideNextToPlay() {
        if (mMatch.AvailableAutomatchSlots > 0) {
            // hand over to an automatch player
            return null;
        } else {
            // hand over to our (only) opponent
            Participant opponent = Util.GetOpponent(mMatch);
            return opponent == null ? null : opponent.ParticipantId;
        }
    }

    public void EndTurn() {
        mEndingTurn = true;

        // do we have a winner?
        if (mMatchData.HasWinner) {
            FinishMatch();
        } else {
            TakeTurn();
        }
    }

    string GetAdversaryParticipantId() {
        foreach (Participant p in mMatch.Participants) {
            if (!p.ParticipantId.Equals(mMatch.SelfParticipantId)) {
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
        outcome.SetParticipantResult(mMatch.SelfParticipantId,
            winnerIsMe ? MatchOutcome.ParticipantResult.Win : MatchOutcome.ParticipantResult.Loss);
        outcome.SetParticipantResult(GetAdversaryParticipantId(),
            winnerIsMe ? MatchOutcome.ParticipantResult.Loss : MatchOutcome.ParticipantResult.Win);

        // finish the match
        //SetStandBy("Sending...");
        PlayGamesPlatform.Instance.TurnBased.Finish(mMatch, mMatchData.ToBytes(),
            outcome, (bool success) => {
                //EndStandBy();
                Debug.Log(success ? (winnerIsMe ? "YOU WON!" : "YOU LOST!") :
                "ERROR finishing match.");
            });
    }

    void TakeTurn() {
        //SetStandBy("Sending...");
        mMatchData.Replay = Player.ToString();

        PlayGamesPlatform.Instance.TurnBased.TakeTurn(mMatch, mMatchData.ToBytes(),
            DecideNextToPlay(), (bool success) => {
                //EndStandBy();
                Debug.Log(success ? "Turn taken" : "Error taking turn");
            });
    }

    public void Cancel() {
        PlayGamesPlatform.Instance.TurnBased.Cancel(mMatch,
            (bool success) => {
                //EndStandBy();
                Debug.Log(success ? "Cancelled" : "Error cancelling");
            });
    }
}
