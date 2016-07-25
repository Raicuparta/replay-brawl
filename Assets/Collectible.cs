using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {
    public int GetId() {
        return int.Parse(name);
    }

	// Called when this object is collected
	public void Collect() {
        // remove tag so the object is no longer marked as collectible
        tag = "Untagged";
        // TODO do something else instead of immedeately destroying it?
        Destroy(gameObject);
    }
}
