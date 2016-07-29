using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    Scrollbar Bar;
    [SerializeField]
    float InitialTime = 60;
    float CurrentTime;
    float FinalTime = 0;
    public HealthManager Player;
    public HealthManager Opponent;

    void Awake() {
        Bar = GetComponent<Scrollbar>();
        CurrentTime = InitialTime;
    }

    void Update() {
        if (GameEnded()) {
            Reset();
            return;
        }
        if (CurrentTime <= 0) {
            Timeout();
            return;
        }
        CurrentTime -= Time.deltaTime;
        Bar.value = 1 - CurrentTime / InitialTime;
    }

    bool GameEnded() {
        return CurrentTime < InitialTime &&
        (!Player.IsNormal() ||
        !Opponent.IsNormal());
    }

    public float GetFinalTime() {
        Debug.Log("Final time: " + FinalTime);
        return FinalTime;
    }

    void Reset() {
        FinalTime = Mathf.Max(FinalTime, InitialTime - CurrentTime);
        CurrentTime = InitialTime;
    }

    void Timeout() {
        Player.Death();
    }
}
