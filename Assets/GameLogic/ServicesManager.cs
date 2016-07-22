using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServicesManager : MonoBehaviour {
    public static TurnBasedMatch Match;

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
                Transform menu = GameObject.Find("Canvas").transform; // TODO maybe move the menu stuff somewhere else
                foreach (Transform child in menu) {
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
        Transform menu = GameObject.Find("Canvas").transform;
        foreach (Transform child in menu) {
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

    protected void OnMatchStarted(bool success, TurnBasedMatch match) {
        if (!success) {
            Debug.LogError("Error starting match");
            return;
        }

        Debug.Log("Match started");
        Match = match;
        SceneManager.LoadScene("TicTacToss");
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
}
