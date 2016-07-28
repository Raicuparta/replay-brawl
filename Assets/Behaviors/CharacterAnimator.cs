using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour {
    Animator Anim;
    Capture Cap;
    private bool FacingRight = true;

    void Start() {
        Anim = GetComponent<Animator>();
        Cap = GetComponent<Capture>();
    }

    void FixedUpdate() {
        Movement();
    }

    private void Movement() {
        // Wait until there's enough info to calculate the direction 
        if (Cap.TickCount < 2 || Cap.Steps.Count < Cap.TickCount) return;
        HorizontalMovement();
        VerticalMovement();
    }

    private void HorizontalMovement() {
        // Compare the most recent position and the one before that to find the direction
        float x0 = Cap.Steps[Cap.TickCount - 1].x;
        float x1 = Cap.Steps[Cap.TickCount - 2].x;
        float velocity = x1 - x0;
        Anim.SetFloat("Speed", Mathf.Abs(velocity)); // Animator uses this to pick animation

        // Check if flipping is necessary
        if (velocity > 0 && FacingRight || velocity < 0 && !FacingRight) {
            // Switch the way the player is labelled as facing.
            FacingRight = !FacingRight;

            // Actually flip the sprite
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    private void VerticalMovement() {
        // Compare the most recent position and the one before that to find the direction
        float y0 = Cap.Steps[Cap.TickCount - 1].y;
        float y1 = Cap.Steps[Cap.TickCount - 2].y;

        float vSpeed = Mathf.Round((y0 - y1) * 100); // Spaghetti number six
        Anim.SetFloat("vSpeed", vSpeed); // Animator uses this to pick animation

        Anim.SetBool("Ground", vSpeed == 0);
    }
}