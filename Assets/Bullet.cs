using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag != "Attackable") return;
        collider.GetComponent<HealthManager>().Hit();
        Destroy(gameObject);
    }
}
