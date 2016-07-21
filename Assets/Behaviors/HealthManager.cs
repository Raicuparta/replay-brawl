using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {

    private int Health = 100;
    private const int InitialHealth = 100;
    [SerializeField]
    private int HealthDecrement = 10; // Amount of health to remove when receiving an attack

    public void Hit() {
        Health -= HealthDecrement;
        Debug.Log("Registered Hit: " + Health);
    }
}
