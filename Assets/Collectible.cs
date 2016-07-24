using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {
    int Id;

    void Start() {
        Id = GetInstanceID();
    }

    public int GetId() {
        return Id;
    }

	// Called when this object is collected
	public void Collect() {
        Destroy(gameObject);
    }
}
