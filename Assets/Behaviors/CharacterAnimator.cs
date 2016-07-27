using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour {
    Animator Anim;
    Capture Cap;
    private bool FacingRight = true;
    // Number of steps too look behind the current step
    // to determine movement direction
    static int VelMultiplier = 5;

    void Start () {
        Anim = GetComponent<Animator>();
        Cap = GetComponent<Capture>();
	}
	
	void FixedUpdate () {
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
        float x = Cap.Steps[Cap.TickCount - 1].x * VelMultiplier;
        Anim.SetFloat("Speed", Mathf.Abs(x)); // Animator uses this to pick animation

        // Check if flipping is necessary
        if (x < -0.01 && FacingRight || x > 0.01 && !FacingRight) {
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
        float y = Cap.Steps[Cap.TickCount - 1].y;

        float vSpeed = Mathf.Round((y)*100); // Spaghetti number six
        Anim.SetFloat("vSpeed", vSpeed); // Animator uses this to pick animation
        Anim.SetBool("Ground", vSpeed == 0);
    }
}
