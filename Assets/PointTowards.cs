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

    // Update is called once per frame
    void Update() {
        if (TargetRenderer.isVisible) SelfRenderer.enabled = false;
        else SelfRenderer.enabled = true;
        transform.position = Origin.position;
        Quaternion rotation = Quaternion.LookRotation
             (Target.transform.position - transform.position, transform.TransformDirection(Vector3.up));
        transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
    }
}
