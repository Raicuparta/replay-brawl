using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
    public int AttackDuration = 10;                          // How many ticks the attack animation lasts
    int CurrentAttackTime = 0;
    bool AttackAnimation;
    bool Attacking;
    Rigidbody2D Body;
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
        if (Bullet == null && Attacking && collider.tag == "Attackable") {
            ConnectAttack(collider);
        }
    }

    void ConnectAttack(Collider2D collider) {
        Attacking = false;
        collider.GetComponentInParent<HealthManager>().Hit();
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

    void ShootBullet() {
        Rigidbody2D bullet = GameObject.Instantiate<Rigidbody2D>(Bullet);
        bullet.position = transform.position;
        bullet.AddForce(Vector2.right * Body.transform.lossyScale.x * 20, ForceMode2D.Impulse);
    }
}
