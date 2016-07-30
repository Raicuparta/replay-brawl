using UnityEngine;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;

public class GameManager : MonoBehaviour {
    public Capture Player;
    public Capture Opponent;
    public PauseMenu Menu;
    public Timer MatchTimer;

    public const int MaxRounds = 4;
    Transform Objects;
    TurnBasedMatch Match = null;
    MatchData Data = null;
    // True if current round is to be uploaded when finished
    public bool IsSoloRound = false;

    void Start() {
        Match = ServicesManager.Match;
        Data = ServicesManager.Data;
        Objects = transform.Find("Objects");
        LaunchMatch();
        Menu.SetRound(Data.GetRound());
        Menu.SetVictories(Data.Victories);
        Menu.SetTimes(Data.FinishTimes);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
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
        if (Data.Steps == null || Data.Steps.Count == 0) SoloRound();
        else FightRound();
    }

    // Round where the player plays alone to complete an objective
    void SoloRound() {
        Opponent.Reset();
        Player.Reset();
        Menu.Reset();
        IsSoloRound = true;
        MakePlayerCollector();
        Opponent.gameObject.SetActive(false);
        foreach (Transform child in Objects) {
            child.gameObject.SetActive(true);
            child.tag = "Collectible";
        }
    }

    // Round where the player tries to stop the opponent
    void FightRound() {
        IsSoloRound = false;
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
        string nextId = null;
        if (Match.AvailableAutomatchSlots == 0) {
            Participant next;
            // If this is a fight round, the next player is ourselves
            // If it is a solo round, the next player is the opponent
            if (IsSoloRound) next = Util.GetOpponent(Match);
            else next = Match.Self;
            nextId = next == null ? null : next.ParticipantId;
        }
        return nextId;
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
        bool winnerIsMe = Data.HostWins == IsHost();

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

    public void EndTurn() {
        // Checks if there's an updated version of the match and then sends the turn
        PlayGamesPlatform.Instance.TurnBased.GetAllMatches(OnGetAllMatches);
    }

    // true if the current player is the host (the original creator of the match)
    bool IsHost() {
        Debug.Log("IsHost: this is round " + Data.GetRound());
        return Data.GetRound() % 2 == 0;
    }

    // true if the host (not necessarily this player) won this round
    bool HostWins() {
        HealthManager player = Player.GetComponent<HealthManager>();
        HealthManager opponent = Opponent.GetComponent<HealthManager>();
        bool playerWins = opponent.IsDead() || player.IsWinner();
        bool opponentWins = player.IsDead() || opponent.IsWinner();
        return IsHost() ? playerWins : opponentWins;
    }

    void SendTurn() {
        //SetStandBy("Sending...");
        if (IsSoloRound) {
            Data.IncRound();
            Menu.SetRound(Data.GetRound());
            Data.AddFinishTime(MatchTimer.GetFinalTime());
        } else {
            Player.Steps.Clear();
            Data.AddVictory(HostWins());
        }
        Menu.SetVictories(Data.Victories);
        Menu.SetTimes(Data.FinishTimes);

        if (Data.HasWinner) {
            FinishMatch();
            return;
        }

        PlayGamesPlatform.Instance.TurnBased.TakeTurn(Match, Data.ToBytes(Player.Steps),
            DecideNextToPlay(), (bool success) => {
                //EndStandBy();
                Debug.Log(success ? "Turn taken" : "Error taking turn");

                if (!IsSoloRound && success) SoloRound();
            });
    }

    protected void OnGetAllMatches(TurnBasedMatch[] matches) {
        foreach (TurnBasedMatch match in matches) {
            Debug.Log("Match ID: " + match.MatchId);
            if (match.MatchId == Match.MatchId) {
                Match = match;
                break;
            }
        }
        SendTurn();
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
