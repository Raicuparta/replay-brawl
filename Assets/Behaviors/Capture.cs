using UnityEngine;
using UnityStandardAssets._2D;
using System.Collections.Generic;

public class Capture : MonoBehaviour {
    public bool Recording = false;
    public bool Replaying = false;

    Rigidbody2D Body;
    PlatformerCharacter2D Player;
    Platformer2DUserControl PlayerControl;
    Vector2 StartPosition;
    Attack OpponentAttack;
    int Score = 0;

    [SerializeField]
    GameManager Manager;

    [System.NonSerialized]
    public List<Step> Steps;
    [System.NonSerialized]
    public int TickCount = 0;

    public struct Step {
        public float x;
        public float y;
        public bool attack;
        public int collect;
    }

    void Awake() {
        Steps = new List<Step>();
        Body = GetComponent<Rigidbody2D>();
        Player = GetComponent<PlatformerCharacter2D>();
        PlayerControl = GetComponent<Platformer2DUserControl>();
        StartPosition = transform.position;
        //transform.position = Vector2.zero;
        OpponentAttack = GetComponent<Attack>();
    }

    void FixedUpdate() {
        if (Recording || Replaying) TickCount++;
        if (Recording && PlayerControl) Record();
        if (Replaying) Replay();
    }

    void Record() {
        Step step = new Step();
        step.x = transform.position.x;
        step.y = transform.position.y;
        //Debug.Log("Recorded position for " + DebugCount + ": " + transform.position.x + ", " + transform.position.y);
        step.attack = PlayerControl.GetAttack();
        step.collect = Player.GetLastCollected();
        Player.ResetLastCollected(); // reset the collected item ID
        Steps.Add(step);
    }

    void Replay() {
        if (Recording) {
            Recording = false;
            TickCount = 0;
        }
        if (TickCount >= Steps.Count) return;
        Step step = Steps[TickCount];
        transform.position = new Vector3(step.x, step.y);
        //Debug.Log("Replayed position for " + DebugCount + ": " + step.x + ", " + step.y);
        if (step.attack) OpponentAttack.TriggerAttack();
        if (step.collect != -1) Collect(step.collect); 
    }

    public void Reset() {
        // TODO cleanup
        Steps.Clear();
        TickCount = 0;
        GetComponent<HealthManager>().Reset();
    }

    void Collect(int id) {
        Score++;
        Collectible c = Manager.GetStageObject(id);
        if (c == null) Debug.LogError("No object found with ID " + id);
        c.Collect();
        GetComponent<HealthManager>().CheckIfWinner();
    }
}
