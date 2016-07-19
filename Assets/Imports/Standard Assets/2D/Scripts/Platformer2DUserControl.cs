using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;
        private bool m_Attack;


        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
        }


        private void Update()
        {                
            // Read button inputs in Update so presses aren't missed.
            if (!m_Jump)
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");

            if (!m_Attack)
                m_Attack = CrossPlatformInputManager.GetButtonDown("Fire1");
        }


        private void FixedUpdate()
        {
            // Read the inputs.

            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            m_Character.Move(h, m_Attack, m_Jump);
            m_Jump = false;
            m_Attack = false;
        }
    }
}
