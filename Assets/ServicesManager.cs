using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

public class ServicesManager : MonoBehaviour {
    bool AuthOnStart = true;
    System.Action<bool> AuthCallback;

    // Always 1v1 match so only 1 opponent for now
    private const int Opponents = 1;
    // Default matching - matches everything as 0xffffffff
    private const uint Variant = 0xffffffff;

    void Start() {
        AuthCallback = (bool success) => {
            if (success) {
                Debug.Log("Login Success");
                // Activate menu buttons that require login
                foreach (Transform child in transform) {
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
        PlayGamesPlatform.Instance.localUser.Authenticate(AuthCallback);
    }

    public void Logout() {
        PlayGamesPlatform.Instance.SignOut();
        foreach (Transform child in transform) {
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
        PlayGamesPlatform.Instance.TurnBased.CreateWithInvitationScreen(Opponents, Opponents, Variant, OnMatchStarted);
    }

    public void Random() {
        PlayGamesPlatform.Instance.TurnBased.CreateQuickMatch(Opponents, Opponents, Variant, OnMatchStarted);
    }

    public void Inbox() {
        PlayGamesPlatform.Instance.TurnBased.AcceptFromInbox(OnMatchStarted);
    }

    protected void OnMatchStarted(bool success, TurnBasedMatch match) {
        if (!success) {
            Debug.Log("Error starting match");
            return;
        }

        // TODO
    }

    protected void OnGotInvitation(Invitation invitation, bool shouldAutoAccept) {
        // Check if invite type is correct
        if (invitation.InvitationType != Invitation.InvType.TurnBased) return;
        // TODO
    }

    protected void OnGotMatch(TurnBasedMatch match, bool shouldAutoLaunch) {
        // TODO
    }
}
