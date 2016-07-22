using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof (PlatformerCharacter2D))]
public class Platformer2DUserControl : MonoBehaviour
{
    [Range(0,1)]
    public float JumpThreshold = 0.5f; // How much you need to move the joystick up to trigger a jump
    private PlatformerCharacter2D Character;
    private bool Attack;
    private float PrevJoyV = 0;


    private void Awake()
    {
        Character = GetComponent<PlatformerCharacter2D>();
    }


    private void Update()
    {                
        // Read button inputs in Update so presses aren't missed.
        if (!Attack)
            Attack = CrossPlatformInputManager.GetButtonDown("Fire1");
    }


    private void FixedUpdate()
    {
        // Read the inputs.

        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        // Use the analog stick to jump
        bool j = v > JumpThreshold;
        bool jump = j && PrevJoyV <= JumpThreshold;
        PrevJoyV = v;

        // Pass all parameters to the character control script.
        Character.Move(h, Attack, jump);
        Attack = false;
    }

    public bool GetAttack() {
        return Attack;
    }
}
