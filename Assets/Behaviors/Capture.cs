using UnityEngine;
using UnityStandardAssets._2D;
using System.Collections.Generic;

public class Capture : MonoBehaviour {
    public bool Recording = false;
    public bool Replaying = false;

    public bool DebugOn;
    public string DebugSteps;
    public static int DebugCount = 0;

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
    Vector2 PreviousPosition;
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
        StartPosition = Body.position;
        //Body.position = Vector2.zero;
        OpponentAttack = GetComponent<Attack>();
    }

    void Start() {

        // Create the first step
        // It marks the absolute starting position instead of a diff
        Step step = new Step();
        step.x = StartPosition.x;
        step.y = StartPosition.y;
        PreviousPosition = StartPosition;
        Steps.Add(step);
        DebugCount++;
    }

    void FixedUpdate() {
        if (Recording || Replaying) TickCount++;
        if (Recording && PlayerControl) Record();
        if (Replaying) Replay();
    }

    void Record() {
        Step step = new Step();
        step.x = Body.position.x - PreviousPosition.x;
        step.y = Body.position.y - PreviousPosition.y;
        //Debug.Log("Recorded position for " + DebugCount + ": " + Body.position.x + ", " + Body.position.y);
        step.attack = PlayerControl.GetAttack();
        step.collect = Player.GetLastCollected();
        Player.ResetLastCollected(); // reset the collected item ID
        PreviousPosition = Body.position;
        Steps.Add(step);
    }

    void Replay() {
        if (Recording) {
            ToString();
            TickCount = 0;
            Recording = false;
            ToString();
            Body.position = Vector2.zero;
        }
        if (TickCount >= Steps.Count) return;
        Step step = Steps[TickCount];
        Body.position += new Vector2(step.x, step.y);
        //Debug.Log("Replayed position for " + DebugCount + ": " + step.x + ", " + step.y);
        if (step.attack) OpponentAttack.TriggerAttack();
        if (step.collect != -1) Collect(step.collect); 
    }

    public void Reset() {
        Steps.Clear();
        Body.position = StartPosition;
        TickCount = 0;
    }

    void Collect(int id) {
        Score++;
        Collectible c = Manager.GetStageObject(id);
        if (c == null) Debug.LogError("No object found with ID " + id);
        c.Collect();
        GetComponent<HealthManager>().CheckIfWinner();
    }
}
