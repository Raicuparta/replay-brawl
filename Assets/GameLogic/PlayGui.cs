using UnityEngine;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;

public class PlayGui : MonoBehaviour {
    public Capture Player;
    public Capture Opponent;
    public PauseMenu Menu;

    private TurnBasedMatch mMatch = null;
    private MatchData mMatchData = null;
    private string mFinalMessage = null;
    private char mMyMark = '\0';

    private bool mEndingTurn = false;

    // countdown to hide instructions
    private bool mShowInstructions = false;

    void Start() {
        LaunchMatch(ServicesManager.Match);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // TODO actually pause
            EndTurn();
            Menu.Toggle();
        }
    }

    private void Reset() {
        mMatch = null;
        mMatchData = null;
        mFinalMessage = null;
        mMyMark = '\0';
        mEndingTurn = false;
        mShowInstructions = false;
        Opponent.gameObject.SetActive(true);
        Opponent.Reset();
        //Player.Reset();
    }

    public void LaunchMatch(TurnBasedMatch match) {
        Reset();
        mMatch = match;

        if (mMatch == null) {
            Debug.Log("PlayGui can't be started without a match!");
            return;
        }
        try {
            // Note that mMatch.Data might be null (when we are starting a new match).
            // MatchData.MatchData() correctly deals with that and initializes a
            // brand-new match in that case.
            mMatchData = new MatchData(mMatch.Data);
        } catch (MatchData.UnsupportedMatchFormatException ex) {
            mFinalMessage = "Your game is out of date. Please update your game\n" +
                "in order to play this match.";
            Debug.LogWarning("Failed to parse board data: " + ex.Message);
            return;
        }

        // determine if I'm the 'X' or the 'O' player
        mMyMark = mMatchData.GetMyMark(match.SelfParticipantId);

        bool canPlay = (mMatch.Status == TurnBasedMatch.MatchStatus.Active &&
                mMatch.TurnStatus == TurnBasedMatch.MatchTurnStatus.MyTurn);

        if (canPlay) {
            mShowInstructions = true;
        } else {
            mFinalMessage = ExplainWhyICantPlay();
        }

        // if the match is in the completed state, acknowledge it
        if (mMatch.Status == TurnBasedMatch.MatchStatus.Complete) {
            PlayGamesPlatform.Instance.TurnBased.AcknowledgeFinished(mMatch,
                    (bool success) => {
                if (!success) {
                    Debug.LogError("Error acknowledging match finish.");
                }
            });
        }

        // set up the objects to show the match to the player
        SetupObjects(canPlay);
    }

    private string ExplainWhyICantPlay() {
        switch (mMatch.Status) {
            case TurnBasedMatch.MatchStatus.Active:
                break;
            case TurnBasedMatch.MatchStatus.Complete:
                return mMatchData.Winner == mMyMark ? "Match finished. YOU WIN!" :
                        "Match finished. YOU LOST!";
            case TurnBasedMatch.MatchStatus.Cancelled:
            case TurnBasedMatch.MatchStatus.Expired:
                return "This match was cancelled.";
            case TurnBasedMatch.MatchStatus.AutoMatching:
                return "This match is awaiting players.";
            default:
                return "This match can't continue due to an error.";
        }

        if (mMatch.TurnStatus != TurnBasedMatch.MatchTurnStatus.MyTurn) {
            return "It's not your turn yet!";
        }

        return "Error";
    }

    private void SetupObjects(bool canPlay) {
        string replay = mMatchData.Replay;
        if (replay != null) {
            Opponent.ReadFromString(mMatchData.Replay);
            Opponent.Replaying = true;
        }
        else {
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
        bool winnerIsMe = mMatchData.Winner == mMyMark;

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
            mFinalMessage = success ? (winnerIsMe ? "YOU WON!" : "YOU LOST!") :
                "ERROR finishing match.";
        });
    }

    void TakeTurn() {
        //SetStandBy("Sending...");
        mMatchData.Replay = Player.ToString();

        PlayGamesPlatform.Instance.TurnBased.TakeTurn(mMatch, mMatchData.ToBytes(),
                    DecideNextToPlay(), (bool success) => {
            //EndStandBy();
            mFinalMessage = success ? "Done for now!" : "ERROR sending turn.";
        });
    }
}
