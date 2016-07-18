using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class Capture : MonoBehaviour {
    public bool Recording = false;
    public bool Replaying = false;

    int TickCount = 0;

    Rigidbody2D Body;
    ArrayList Steps;

    public class Step {
        public Vector3 Position;

        public Step(Vector3 position) {
            Position = position;
        }
    }

    // Use this for initialization
    void Start() {
        Steps = new ArrayList();
        Body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (Recording || Replaying) TickCount++;

        if (Recording) {
            Steps.Add(new Step(Body.velocity));
        }

        if (Replaying) {
            if (Recording) {
                TickCount = 0;
                Recording = false;
                GetComponent<Platformer2DUserControl>().enabled = false;
                GetComponent<PlatformerCharacter2D>().enabled = false;
                //GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
            Body.velocity = ((Step) Steps[TickCount]).Position;
        }
    }
}
