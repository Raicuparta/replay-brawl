using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServicesManager : MonoBehaviour {
    public static TurnBasedMatch Match;
    public static MatchData Data;
    public Transform Stats;
    public Transform Main;

    bool AuthOnStart = true;
    System.Action<bool> AuthCallback;

    // Always 1v1 match so only 1 opponent for now
    private const int Opponents = 1;
    // Default matching - matches everything as 0xffffffff
    private const uint Variant = 0xffffffff;

    void Start() {
        // Prevent this object from being destroyed when changing scenes
        // This is so we can load the match information from here later
        DontDestroyOnLoad(transform.gameObject);

        AuthCallback = (bool success) => {
            if (success) {
                Debug.Log("Login Success");
                // Activate menu buttons that require login
                // TODO maybe move the menu stuff somewhere else
                foreach (Transform child in Main) {
                    Button button = child.GetComponent<Button>();
                    if (child.name == "Login") {
                        // Show user name instead of Login button
                        button.GetComponentInChildren<Text>().text = PlayGamesPlatform.Instance.GetUserDisplayName();
                        button.interactable = false;
                    } else
                        button.interactable = true;
                }
            } else Debug.LogError("Error logging in");
        };

        // Configure and initialize Play Services
        PlayGamesClientConfiguration config =
              new PlayGamesClientConfiguration.Builder()
                .WithInvitationDelegate(OnGotInvitation)
                .WithMatchDelegate(OnGotMatch)
                .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.DebugLogEnabled = false;

        // try silent authentication
        if (AuthOnStart)
            PlayGamesPlatform.Instance.Authenticate(AuthCallback, true);
    }

    public void Login() {
        // Request authentication
        Debug.Log("Requesting authentication...");
        PlayGamesPlatform.Instance.localUser.Authenticate(AuthCallback);
    }

    public void Logout() {
        PlayGamesPlatform.Instance.SignOut();
        Debug.Log("Logged out");
        foreach (Transform child in Main) {
            Button button = child.GetComponent<Button>();
            if (child.name == "Login") {
                // Change the login button back to its original state
                button.GetComponentInChildren<Text>().text = "Login";
                button.interactable = true;
            } else
                button.interactable = false;
        }
        AuthOnStart = false;
    }

    public void Invite() {
        Debug.Log("Inviting...");
        PlayGamesPlatform.Instance.TurnBased.CreateWithInvitationScreen(Opponents, Opponents, Variant, OnMatchStarted);
    }

    public void Random() {
        Debug.Log("Randoming...");
        PlayGamesPlatform.Instance.TurnBased.CreateQuickMatch(Opponents, Opponents, Variant, OnMatchStarted);
    }

    public void Inbox() {
        Debug.Log("Inboxing...");
        PlayGamesPlatform.Instance.TurnBased.AcceptFromInbox(OnMatchStarted);
    }

    protected void StatsMenu() {
        Stats.gameObject.SetActive(true);
        Main.gameObject.SetActive(false);
    }

    public void Back() {
        Main.gameObject.SetActive(true);
        Stats.gameObject.SetActive(false);
    }

    protected void OnMatchStarted(bool success, TurnBasedMatch match) {
        if (!success) {
            Debug.LogError("Error starting match");
            return;
        }

        Debug.Log("Match started");

        if (match == null) {
            Debug.Log("Game can't be started without a match!");
            return;
        }

        Match = match;

        try {
            // Note that mMatch.Data might be null (when we are starting a new match).
            // MatchData.MatchData() correctly deals with that and initializes a
            // brand-new match in that case.
            Data = new MatchData(match.Data);
            if (Data.HostId == "null") Data.HostId = ServicesManager.Match.SelfParticipantId;

            Debug.Log("HERY round: " + Data.GetRound());
            for (int i = 0; i < match.Participants.Count; i++) {
                Participant p = match.Participants[i];
                Debug.Log("HERY participant " + i + " " + p.DisplayName + " id: " + p.ParticipantId);
            }


            StatsMenu();
        } catch (MatchData.UnsupportedMatchFormatException ex) {
            Debug.LogWarning("Your game is out of date. Please update your game\n" +
                "in order to play this match.");
            Debug.LogWarning("Failed to parse board data: " + ex.Message);
            return;
        }

        // if the match is in the completed state, acknowledge it
        if (Match.Status == TurnBasedMatch.MatchStatus.Complete) {
            PlayGamesPlatform.Instance.TurnBased.AcknowledgeFinished(Match,
                    (bool s) => {
                        if (!s) {
                            Debug.LogError("Error acknowledging match finish.");
                        }
                    });
        }
    }

    public void LoadMatch() {
        bool canPlay = (Match.Status == TurnBasedMatch.MatchStatus.Active &&
        Match.TurnStatus == TurnBasedMatch.MatchTurnStatus.MyTurn);

        if (!canPlay) {
            Debug.Log(ExplainWhyICantPlay(Match));
            return;
        }

        SceneManager.LoadScene("Stage");
    }

    protected void OnGotInvitation(Invitation invitation, bool shouldAutoAccept) {
        // Check if invite type is correct
        if (invitation.InvitationType != Invitation.InvType.TurnBased) return;

        Debug.Log("Got invitation");
        // TODO
    }

    protected void OnGotMatch(TurnBasedMatch match, bool shouldAutoLaunch) {
        Debug.Log("Got match");
        // TODO
    }

    private string ExplainWhyICantPlay(TurnBasedMatch match) {
        switch (match.Status) {
            case TurnBasedMatch.MatchStatus.Active:
                break;
            case TurnBasedMatch.MatchStatus.Complete:
                return "Match finished";
            case TurnBasedMatch.MatchStatus.Cancelled:
            case TurnBasedMatch.MatchStatus.Expired:
                return "This match was cancelled.";
            case TurnBasedMatch.MatchStatus.AutoMatching:
                return "This match is awaiting players.";
            default:
                return "This match can't continue due to an error.";
        }

        if (match.TurnStatus != TurnBasedMatch.MatchTurnStatus.MyTurn) {
            return "It's not your turn yet!";
        }

        return "Error";
    }
}
