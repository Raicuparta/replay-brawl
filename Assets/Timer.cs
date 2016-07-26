using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    Scrollbar Bar;
    [SerializeField]
    float InitialTime = 60;
    float CurrentTime;
    [SerializeField]
    HealthManager Player;

    void Awake() {
        Bar = GetComponent<Scrollbar>();
        CurrentTime = InitialTime;
	}
	
	void Update() {
        if (!Player.IsNormal()) return;
        if (CurrentTime <= 0) {
            Timeout();
            return;
        }
        CurrentTime -= Time.deltaTime;
        Bar.value = 1 - CurrentTime / InitialTime;
    }

    void Timeout() {
        Player.Death();
    }
}
