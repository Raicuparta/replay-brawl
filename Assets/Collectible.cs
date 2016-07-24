using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {
	// Called when this object is collected
	public void Collect() {
        Destroy(gameObject);
    }
}
