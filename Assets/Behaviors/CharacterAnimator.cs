using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour {
    Animator Anim;
    Capture Cap;
    private bool FacingRight = true;  // For determining which way the player is currently facing.

    void Start () {
        Anim = GetComponent<Animator>();
        Cap = GetComponent<Capture>();
	}
	
	void FixedUpdate () {
        SetDirection();
    }

    private void SetDirection() {
        // Wait until there's enough info to calculate the direction 
        if (Cap.TickCount < 2) return;

        // Compare the most recent position and the one before that to find the direction
        float x0 = Cap.Steps[Cap.TickCount - 1].x;
        float x1 = Cap.Steps[Cap.TickCount - 2].x;
        float velocity = x1 - x0;

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
}
