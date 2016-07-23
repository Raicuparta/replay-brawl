using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {
    int Health;
    bool Dead;
    [SerializeField]
    int InitialHealth = 100;
    [SerializeField]
    int HealthDecrement = 10; // Amount of health to remove when receiving an attack
    ParticleSystem Particles;

    void Awake() {
        Particles = GetComponent<ParticleSystem>();
    }

    void Start() {
        Health = InitialHealth;
    }

    public void Hit() {
        if (Dead) return;
        Debug.Log("Registered Hit: " + Health);

        Particles.Play();

        // Decrease health
        Health -= HealthDecrement;

        // Kill if no leath left
        if (Health <= 0) Death();
    }

    private void Death() {
        Dead = true;
        Debug.Log("Someone died");
    }

    public bool IsDead() {
        return Dead;
    }
}