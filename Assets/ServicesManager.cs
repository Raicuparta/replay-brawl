using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;

public class ServicesManager : MonoBehaviour {
    bool AuthOnStart = true;
    System.Action<bool> mAuthCallback;

    void Start() {
        mAuthCallback = (bool success) => {
            // TODO Auth callback
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
            PlayGamesPlatform.Instance.Authenticate(mAuthCallback, true);
    }

    void OnEnable() {
        // Request authentication
        PlayGamesPlatform.Instance.localUser.Authenticate(mAuthCallback);
    }

    protected void OnGotInvitation(Invitation invitation, bool shouldAutoAccept) {
        // Check if invite type is correct
        if (invitation.InvitationType != Invitation.InvType.TurnBased) return;
        // TODO
    }

    protected void OnGotMatch(TurnBasedMatch match, bool shouldAutoLaunch) {
        // TODO
    }

    public void SetAuthOnStart(bool authOnStart) {
        AuthOnStart = authOnStart;
    }
}
