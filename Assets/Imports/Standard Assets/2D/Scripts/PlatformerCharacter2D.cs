using System;
using UnityEngine;

namespace UnityStandardAssets._2D {
    public class PlatformerCharacter2D : MonoBehaviour {
        [SerializeField]
        private float MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField]
        private float JumpForce = 400f;                  // Amount of force added when the player jumps.
        [SerializeField]
        private bool AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField]
        private LayerMask WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField]
        private int AttackDuration = 10;                          // How many ticks the attack animation lasts
        private int CurrentAttackTime = 0;

        private Transform GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool Grounded;            // Whether or not the player is grounded.
        private bool Attacking;
        private Transform CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Rigidbody2D Rigidbody2D;

        private void Awake() {
            // Setting up references.
            GroundCheck = transform.Find("GroundCheck");
            CeilingCheck = transform.Find("CeilingCheck");
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate() {
            Grounded = false;
            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, k_GroundedRadius, WhatIsGround);
            for (int i = 0; i < colliders.Length; i++) {
                if (colliders[i].gameObject != gameObject)
                    Grounded = true;
            }

            if (Attacking) Attack();
        }

        public void Attack() {
            CurrentAttackTime++;
            if (CurrentAttackTime > AttackDuration) {
                Attacking = false;
                CurrentAttackTime = 0;
                GetComponent<Animator>().SetBool("Attack", false);
            }
        }

        public void Move(float move, bool attack, bool jump) {
            //only control the player if grounded or airControl is turned on
            if (Grounded || AirControl) {
                // Move the character
                Rigidbody2D.velocity = new Vector2(move * MaxSpeed, Rigidbody2D.velocity.y);
            }

            // If the player should jump...
            if (Grounded && jump) {
                // Add a vertical force to the player.
                Grounded = false;
                Rigidbody2D.AddForce(new Vector2(0f, JumpForce));
            }

            // If the player should attack...
            if (attack) {
                Attacking = true;
                GetComponent<Animator>().SetBool("Attack", true);
            }
        }
    }
}
