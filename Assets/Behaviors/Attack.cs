using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
    [SerializeField]
    int AttackDuration = 10;                          // How many ticks the attack animation lasts
    int CurrentAttackTime = 0;
    bool AttackAnimation;
    bool Attacking;
    Rigidbody2D Body;
    ParticleSystem Particles;

    [SerializeField]
    float AttackPauseTime = 0.1f;  // How long to freeze the game for when attack connects
    [SerializeField]
    float ShakeAplitude = 0.1f;    // How much to shake the screen when an attack connects
    [SerializeField]
    float ShakeTime = 0.2f;
    public Camera MainCamera;

    private void Awake() {
        Particles = GetComponent<ParticleSystem>();
        Body = GetComponentInParent<Rigidbody2D>();
    }

    private void Update() {
        if (AttackAnimation) {
            UpdateAttack();
        }
        if (Body && Body.IsSleeping())
            Body.WakeUp();
    }

    void OnTriggerStay2D(Collider2D collider) {
        //Debug.Log("Detect Atack1 " + collider.tag);
        // Register hit
        if (Attacking && collider.tag == "Attackable") {
            ConnectAttack(collider);
        }
    }

    void ConnectAttack(Collider2D collider) {
        Debug.Log("Detect Atack " + collider.name);
        Attacking = false;
        collider.GetComponentInParent<HealthManager>().Hit();
        Particles.Play();
        StartCoroutine(PauseFor(AttackPauseTime));
        ShakeScreen();
    }

    public void UpdateAttack() {
        CurrentAttackTime++;
        if (CurrentAttackTime > AttackDuration)
            EndAttack();
    }

    public void EndAttack() {
        Debug.Log("EndAttack");
        AttackAnimation = false;
        Attacking = false;
        CurrentAttackTime = 0;
        GetComponentInParent<Animator>().SetBool("Attack", false);
    }

    public void TriggerAttack() {
        if (!AttackAnimation) {
        Debug.Log("TriggerAttack");
            AttackAnimation = true;
            Attacking = true;
            GetComponentInParent<Animator>().SetBool("Attack", true);
        }
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
