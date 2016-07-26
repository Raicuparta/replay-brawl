using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
    [SerializeField]
    int AttackDuration = 10;                          // How many ticks the attack animation lasts
    int CurrentAttackTime = 0;
    bool AttackAnimation;
    bool Attacking;
    Rigidbody2D Body;

    private void Start() {
        Body = GetComponent<Rigidbody2D>();
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
            Debug.Log("Detect Atack " + collider.name);
            Attacking = false;
            collider.GetComponent<HealthManager>().Hit();
        }
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
        GetComponent<Animator>().SetBool("Attack", false);
    }

    public void TriggerAttack() {
        if (!AttackAnimation) {
        Debug.Log("TriggerAttack");
            AttackAnimation = true;
            Attacking = true;
            GetComponent<Animator>().SetBool("Attack", true);
        }
    }
}
