using UnityEngine;
using System.Collections;

public class PointTowards : MonoBehaviour {
    public Transform Target;
    public Transform Origin;
    SpriteRenderer TargetRenderer;
    SpriteRenderer SelfRenderer;

    void Awake() {
        SelfRenderer = GetComponentInChildren<SpriteRenderer>();
        TargetRenderer = Target.GetComponent<SpriteRenderer>();
        SelfRenderer.color = TargetRenderer.color;
    }
    
    void Update() {
        if (TargetRenderer.isVisible) {
            SelfRenderer.enabled = false;
            return;
        }
        SelfRenderer.enabled = true;
        transform.position = Origin.position;
        transform.LookAt(Target);
    }
}
