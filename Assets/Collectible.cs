using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {
    public int GetId() {
        return int.Parse(name);
    }

	// Called when this object is collected
	public void Collect() {
        Destroy(gameObject);
    }
}
