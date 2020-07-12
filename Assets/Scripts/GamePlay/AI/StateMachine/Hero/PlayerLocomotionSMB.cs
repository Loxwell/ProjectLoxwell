
using UnityEngine;
using Platformer.Mechanics.AI.StateMachine;
using Platformer.Mechanics;

namespace InGame.StateMachine.Player
{
    public class PlayerLocomotionSMB : SceneLinkedSMB<PlayerCharacter>
    {
        float preTime = 0;

        PlayerLocomotionSMB()
        {
            preTime = 0;
        }

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.TeleportToColliderBottom();
            preTime = Time.time;
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float deltaTIme = Time.deltaTime;// Time.time - preTime;
            preTime = Time.time;
            m_MonoBehaviour.UpdateFacing();
            m_MonoBehaviour.GroundedHorizontalMovement(true, deltaTIme);
            m_MonoBehaviour.GroundedVerticalMovement(deltaTIme);
            m_MonoBehaviour.CheckForCrouching();
            m_MonoBehaviour.CheckForGrounded();
            
            if (m_MonoBehaviour.CheckForJumpInput())
                m_MonoBehaviour.SetVerticalMovement(m_MonoBehaviour.jumpSpeed);
            else if (m_MonoBehaviour.CheckForAttackInput())
                m_MonoBehaviour.UpdateAttacking(m_MonoBehaviour.aniStandAttack);
        }
    }
}