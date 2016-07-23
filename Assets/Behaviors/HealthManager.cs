using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {
    int Health;
    bool Dead = false;
    [SerializeField]
    int InitialHealth = 100;
    [SerializeField]
    int HealthDecrement = 10; // Amount of health to remove when receiving an attack
    ParticleSystem Particles;
    Transform HealthBar;
    float InitialHealthScaleX;

    void Awake() {
        Particles = GetComponent<ParticleSystem>();
        HealthBar = transform.Find("Health");
    }

    void Start() {
        Health = InitialHealth;
        InitialHealthScaleX = HealthBar.localScale.x;
    }

    public void Hit() {
        if (Dead) return;
        Debug.Log("Registered Hit: " + Health);

        Particles.Play();

        // Decrease health
        Health -= HealthDecrement;
        UpdateHealthBar();

        // Kill if no leath left
        if (Health <= 0) Death();
    }

    void UpdateHealthBar() {
        Vector2 newScale = HealthBar.localScale;
        newScale.x = ((float)Health / InitialHealth) * InitialHealthScaleX;
        HealthBar.localScale = newScale;
    }

    void Death() {
        Dead = true;
        Debug.Log("Someone died");
    }

    public bool IsDead() {
        return Dead;
    }
}