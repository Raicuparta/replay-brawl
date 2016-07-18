using UnityEngine;
using UnityStandardAssets._2D;
using System.Collections.Generic;

public class Capture : MonoBehaviour {
    public bool Recording = false;
    public bool Replaying = false;

    int TickCount = 0;

    Rigidbody2D Body;

    List<Step> Steps;

    [System.Serializable]
    public class Step {
        public Vector3 Position;
        public Vector3 Velocity;

        public Step(Vector3 position, Vector3 velocity) {
            Position = position;
            Velocity = velocity;
        }

        public override string ToString() {
            return "P" + PrintVector(Position) + "V" + PrintVector(Velocity);
        }
    }

    // Use this for initialization
    void Start() {
        Steps = new List<Step>();
        Body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (Recording || Replaying) TickCount++;

        if (Recording) {
            Steps.Add(new Step(Body.position, Body.velocity));
        }

        if (Replaying) {
            if (Recording) {
                Debug.Log(PrintSteps(Steps));
                TickCount = 0;
                Recording = false;
                GetComponent<Platformer2DUserControl>().enabled = false;
                GetComponent<PlatformerCharacter2D>().enabled = false;
                //GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
            Body.velocity = ((Step) Steps[TickCount]).Velocity;
            Body.position = ((Step)Steps[TickCount]).Position;
        }
    }

    static string PrintSteps (List<Step> steps) {
        string result = "";

        for (int i = 0; i < steps.Count; i++)
            result += steps[i] + "|";

        return result;
    }

    static string PrintVector (Vector3 vector) {
        return "(" + vector.x + "," + vector.y + "," + vector.z + ")";
    }
}
