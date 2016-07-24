using System;
using UnityEngine;

public class PlatformerCharacter2D : MonoBehaviour {
    [SerializeField]
    float MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
    [SerializeField]
    float JumpForce = 400f;                  // Amount of force added when the player jumps.
    [SerializeField]
    bool AirControl = false;                 // Whether or not a player can steer while jumping;
    [SerializeField]
    LayerMask WhatIsGround;                  // A mask determining what is ground to the character
    Transform GroundCheck;    // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    bool Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
    Rigidbody2D Body;
    Attack PlayerAttack;
    Animator Anim;
    bool FacingRight = true;
    int Score = 0;

    void Awake() {
        // Setting up references.
        GroundCheck = transform.Find("GroundCheck");
        Body = GetComponent<Rigidbody2D>();
        PlayerAttack = GetComponent<Attack>();
        Anim = GetComponent<Animator>();
    }

    void FixedUpdate() {
        Grounded = false;
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, k_GroundedRadius, WhatIsGround);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject != gameObject)
                Grounded = true;
        }
        Anim.SetBool("Ground", Grounded);
        Anim.SetFloat("vSpeed", Body.velocity.y);
    }

    public void Move(float move, bool attack, bool jump) {
        //only control the player if grounded or airControl is turned on
        if (Grounded || AirControl) {
            // Move the character
            Body.velocity = new Vector2(move * MaxSpeed, Body.velocity.y);
            Anim.SetFloat("Speed", Mathf.Abs(move));

            if (move > 0 && !FacingRight || move < 0 && FacingRight)
                Flip();
        }

        // If the player should jump...
        if (Grounded && jump) {
            // Add a vertical force to the player.
            Grounded = false;
            Body.AddForce(new Vector2(0f, JumpForce));
            Anim.SetBool("Ground", false);
        }

        // If the player should attack...
        if (attack) {
            PlayerAttack.TriggerAttack();
        }
    }

    void Flip() {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Collectible") {
            Collect();
            other.GetComponent<Collectible>().Collect();
        }
    }

    void Collect() {
        Score++;
        Debug.Log("Collected: " + Score);
    }
}
