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

    public ParticleSystem OriginalParticles;
    public float AttackPauseTime = 0.1f;  // How long to freeze the game for when attack connects
    public float ShakeAplitude = 0.1f;    // How much to shake the screen when an attack connects
    public float ShakeTime = 0.2f;
    public Camera MainCamera;
    public Rigidbody2D Bullet;

    private void Awake() {
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
        Attacking = false;
        collider.GetComponentInParent<HealthManager>().Hit();
        PlayParticles();
        StartCoroutine(PauseFor(AttackPauseTime));
        ShakeScreen();
    }

    void PlayParticles() {
        if (Particles == null)
            Particles = GameObject.Instantiate<ParticleSystem>(OriginalParticles);
        Particles.transform.position = transform.position;
        Particles.Play();
    }

    public void UpdateAttack() {
        CurrentAttackTime++;
        if (CurrentAttackTime > AttackDuration)
            EndAttack();
    }

    public void EndAttack() {
        AttackAnimation = false;
        Attacking = false;
        CurrentAttackTime = 0;
        GetComponentInParent<Animator>().SetBool("Attack", false);
    }

    public void TriggerAttack() {
        if (!AttackAnimation) {
            AttackAnimation = true;
            Attacking = true;
            GetComponentInParent<Animator>().SetBool("Attack", true);
            if (Bullet != null) ShootBullet();
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

    void ShootBullet() {
        Rigidbody2D bullet = GameObject.Instantiate<Rigidbody2D>(Bullet);
        bullet.position = transform.position;
        bullet.AddForce(Vector2.right * Body.transform.lossyScale.x * 20, ForceMode2D.Impulse);
    }
}
