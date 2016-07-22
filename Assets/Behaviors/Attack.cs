using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
    [SerializeField]
    private int AttackDuration = 10;                          // How many ticks the attack animation lasts
    private int CurrentAttackTime = 0;
    private bool AttackAnimation;
    private bool Attacking;

    private void FixedUpdate() {
        if (AttackAnimation) UpdateAttack();
    }

    void OnTriggerStay2D(Collider2D collider) {
        // Register hit
        if (Attacking && collider.tag == "Attackable") {
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
        AttackAnimation = false;
        Attacking = false;
        CurrentAttackTime = 0;
        GetComponent<Animator>().SetBool("Attack", false);
    }

    public void TriggerAttack() {
        if (!AttackAnimation) {
            AttackAnimation = true;
            Attacking = true;
            GetComponent<Animator>().SetBool("Attack", true);
        }
    }
}
