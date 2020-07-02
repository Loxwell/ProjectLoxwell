using UnityEngine;

using Mechanics.AI.StateMachine;
using Platformer.Player;

namespace GamePlay.StateMachine
{
    public class LocomotionSMB : SceneLinkedSMB<PlayerCharacter>
    {
        float preTime = 0;

        LocomotionSMB()
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
                m_MonoBehaviour.Attack(m_MonoBehaviour.aniStandAttack);
        }
    }
}