using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {
    int Health;
    [SerializeField]
    int InitialHealth = 100;
    [SerializeField]
    int HealthDecrement = 10; // Amount of health to remove when receiving an attack
    Transform HealthBar;
    float InitialHealthScaleX;
    CharacterState State = CharacterState.Normal;
    Animator Anim;
    float DamageTime = 0;
    const float StartDamageTime = 0.2f;

    enum CharacterState {
        Normal,
        Winner,
        Dead
    }

    void Awake() {
        HealthBar = transform.Find("Health");
        Anim = GetComponent<Animator>();
    }

    void Start() {
        Health = InitialHealth;
        InitialHealthScaleX = HealthBar.localScale.x;
    }

    public void Hit() {
        if (!IsNormal()) return;
        Debug.Log("Registered Hit: " + Health);
        Anim.SetTrigger("Damage");

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

    public void Death() {
        State = CharacterState.Dead;
        Debug.Log("Someone died");
    }

    public void CheckIfWinner() {
        // Check if collected everything
        int count = GameObject.FindGameObjectsWithTag("Collectible").Length;
        Debug.LogWarning("Remaining obstacles: " + count);
        if (count <= 0) {
            Debug.LogWarning("Collected all");
            Victory();
        }
    }

    public void Victory() {
        State = CharacterState.Winner;
    }

    public bool IsDead() {
        return State == CharacterState.Dead;
    }

    public bool IsWinner() {
        return State == CharacterState.Winner;
    }

    public bool IsNormal() {
        return State == CharacterState.Normal;
    }
}