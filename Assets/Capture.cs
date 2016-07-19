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
            return VectorToString(Position) + "V" + VectorToString(Velocity);
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
                TickCount = 0;
                Recording = false;
                GetComponent<Platformer2DUserControl>().enabled = false;
                GetComponent<PlatformerCharacter2D>().enabled = false;
                //GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
            if (TickCount >= Steps.Count) return;
            Body.velocity = ((Step)Steps[TickCount]).Velocity;
            Body.position = ((Step)Steps[TickCount]).Position;
        }
    }

    public override string ToString() {
        string result = "";

        for (int i = 0; i < Steps.Count; i++)
            result += Steps[i] + "|";

        return result;
    }

    private static string VectorToString(Vector3 vector) {
        return "" + vector.x + "," + vector.y + "," + vector.z;
    }

    private Vector3 StringToVector(string s) {
        if (s.Length == 0) return Vector3.zero; // TODO deal with this

        Vector3 v = new Vector3();
        string[] values = s.Split(',');
        v.x = float.Parse(values[0]);
        v.y = float.Parse(values[1]);
        v.z = float.Parse(values[2]);
        return v;
    }

    public void ReadFromString(string data) {
        Steps.Clear();

        string[] steps = data.Split('|');

        for (int i = 0; i < steps.Length - 1; i++) {
            string[] vectors = steps[i].Split('V');
            Vector3 position = StringToVector(vectors[0]);
            Vector3 velocity = StringToVector(vectors[1]);

            Steps.Add(new Step(position, velocity));
        }
    }
}
