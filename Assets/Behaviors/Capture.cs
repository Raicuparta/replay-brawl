using UnityEngine;
using UnityStandardAssets._2D;
using System.Collections.Generic;

public class Capture : MonoBehaviour {
    public bool Recording = false;
    public bool Replaying = false;

    public bool DebugOn;
    public string DebugSteps;

    Rigidbody2D Body;
    PlatformerCharacter2D Player;
    Platformer2DUserControl PlayerControl;
    Vector2 StartPosition;
    Attack OpponentAttack;
    int LastCollected = -1;
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
        StartPosition = Body.position;
        OpponentAttack = GetComponent<Attack>();
    }

    void FixedUpdate() {
        if (Recording || Replaying) TickCount++;
        if (Recording && PlayerControl) Record();
        if (Replaying) Replay();
    }

    void Record() {
        Step step = new Step();
        step.x = Body.position.x;
        step.y = Body.position.y;
        step.attack = PlayerControl.GetAttack();
        step.collect = LastCollected;
        LastCollected = -1; // reset the collected item ID
        Steps.Add(step);
    }

    void Replay() {
        if (Recording) {
            ToString();
            TickCount = 0;
            Recording = false;
            ToString();
        }
        if (TickCount >= Steps.Count) return;
        Step step = Steps[TickCount];
        Body.position = new Vector2(step.x, step.y);
        if (step.attack) OpponentAttack.TriggerAttack();
        if (step.collect != -1) Collect(step.collect); 
    }

    public void Reset() {
        Steps.Clear();
        Body.position = StartPosition;
        TickCount = 0;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Collectible") {
            Collectible collectible = other.GetComponent<Collectible>();
            Collect(collectible.GetId());
            collectible.Collect();
        }
    }

    void Collect(int id) {
        Score++;
        Debug.Log("Collected: " + Score);
        if (Recording) LastCollected = id;
        else if (Replaying) {
            LastCollected = Manager.GetStageObject(id).GetId();
        }
    }
}
