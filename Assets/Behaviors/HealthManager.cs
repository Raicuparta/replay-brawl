using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {

    private int Health = 100;
    private bool Dead;
    [SerializeField]
    private int InitialHealth = 100;
    [SerializeField]
    private int HealthDecrement = 10; // Amount of health to remove when receiving an attack

    public void Hit() {
        if (Dead) return;
        Health -= HealthDecrement;
        Debug.Log("Registered Hit: " + Health);
        if (Health <= 0) Death();
    }

    private void Death() {
        Debug.Log("Someone died");
    }
}