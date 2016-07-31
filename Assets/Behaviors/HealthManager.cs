using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {
    int Health;
    public int InitialHealth = 100;
    public int HealthDecrement = 10; // Amount of health to remove when receiving an attack
    Transform HealthBar;
    float InitialHealthScaleX;
    CharacterState State = CharacterState.Normal;
    Animator Anim;
    ParticleSystem Particles;
    public ParticleSystem OriginalParticles;
    public float AttackPauseTime = 0.1f;  // How long to freeze the game for when attack connects
    public float ShakeAplitude = 0.1f;    // How much to shake the screen when an attack connects
    public float ShakeTime = 0.2f;
    public Camera MainCamera;

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

    public void Reset() {
        Start();
        State = CharacterState.Normal;
    }

    public void Hit() {
        if (!IsNormal()) return;
        Debug.Log("Registered Hit: " + Health);
        Anim.SetTrigger("Damage");
        PlayParticles();
        StartCoroutine(PauseFor(AttackPauseTime));
        ShakeScreen();
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
        // TODO do this more efficiently
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

    void PlayParticles() {
        if (Particles == null)
            Particles = GameObject.Instantiate<ParticleSystem>(OriginalParticles);
        Particles.transform.position = transform.position;
        Particles.Play();
    }
    private IEnumerator PauseFor(float time) {
        Time.timeScale = 0f;
        float pauseEndTime = Time.realtimeSinceStartup + time;
        while (Time.realtimeSinceStartup < pauseEndTime) {
            yield return 0;
        }
        Time.timeScale = 1;
    }

    void ShakeScreen() {
        InvokeRepeating("StartShaking", 0, .01f);
        Invoke("StopShaking", ShakeTime);
    }

    void StartShaking() {
        if (ShakeAplitude > 0) {
            float shakeX = Random.value * ShakeAplitude * 2 - ShakeAplitude;
            float shakeY = Random.value * ShakeAplitude * 2 - ShakeAplitude;
            Vector3 pp = MainCamera.transform.position;
            pp.x += shakeX;
            pp.y += shakeY;
            MainCamera.transform.position = pp;
        }
    }

    void StopShaking() {
        CancelInvoke("StartShaking");
    }

}