using UnityEngine;
using UnityStandardAssets._2D;
using System.Collections.Generic;

public class Capture : MonoBehaviour {
    public bool Recording = false;
    public bool Replaying = false;

    public bool DebugOn;
    public string DebugSteps;

    private Rigidbody2D Body;
    private PlatformerCharacter2D Player;
    private Platformer2DUserControl PlayerControl;
    private Vector2 StartPosition;
    private Attack OpponentAttack;

    [System.NonSerialized]
    public List<Vector3> Steps;
    [System.NonSerialized]
    public int TickCount = 0;

    void Awake() {
        Steps = new List<Vector3>();
        Body = GetComponent<Rigidbody2D>();
        Player = GetComponent<PlatformerCharacter2D>();
        PlayerControl = GetComponent<Platformer2DUserControl>();
        StartPosition = Body.position;
        OpponentAttack = GetComponent<Attack>();
    }

    void Start() {
        if (DebugOn) {
            ReadFromString(DebugSteps);
            Replaying = true;
        }
    }

    void FixedUpdate() {
        if (Recording || Replaying) TickCount++;
        if (Recording && PlayerControl) Record();
        if (Replaying) Replay();
    }

    void Record() {
        float attack = PlayerControl.GetAttack() ? 1 : 0;
        Vector3 position = new Vector3(Body.position.x, Body.position.y, attack);
        Steps.Add(position);
    }

    void Replay() {
        if (Recording) {
            ToString();
            TickCount = 0;
            Recording = false;
            ToString();
        }
        if (TickCount >= Steps.Count) return;
        Body.position = Steps[TickCount];
        if (Steps[TickCount].z != 0) OpponentAttack.TriggerAttack();

    }

    public override string ToString() {
        string result = "";

        for (int i = 0; i < Steps.Count; i++)
            result += VectorToString(Steps[i]) + "|";

        Debug.Log("####### ToString: " + result);
        return result;
    }

    private static string VectorToString(Vector3 vector) {
        string result = "" + vector.x + "," + vector.y;
        if (vector.z != 0) result += "," + vector.z;
        //Debug.Log("####### VectorToString: " + result);
        return result;
    }

    private Vector3 StringToVector(string s) {
        //Debug.Log("####### StringToVector: " + s);
        if (s.Length == 0) return Vector3.zero; // TODO deal with this

        Vector3 v = new Vector3();
        string[] values = s.Split(',');
        v.x = float.Parse(values[0]);
        v.y = float.Parse(values[1]);
        if (values.Length > 2)
            v.z = float.Parse(values[2]);
        else
            v.z = 0;
        return v;
    }

    public void ReadFromString(string data) {
        Debug.Log("####### ReadFromString: " + data);
        Steps.Clear();

        string[] steps = data.Split('|');

        for (int i = 0; i < steps.Length - 1; i++) {
            Vector3 position = StringToVector(steps[i]);
            Steps.Add(position);
        }
    }

    public void Reset() {
        Steps.Clear();
        Body.position = StartPosition;
        TickCount = 0;
    }
}
