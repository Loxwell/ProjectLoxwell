using UnityEngine;

using Platformer.Mechanics.AI.StateMachine;
using Platformer.Mechanics.AI;

namespace InGame.StateMachine.Player
{
    public class PlayerCrouchSMB : SceneLinkedSMB<PlayerCharacter>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.TeleportToColliderBottom();
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float deltaTime = Time.deltaTime;//Time.time - m_preTime;

            m_MonoBehaviour.UpdateFacing();
            m_MonoBehaviour.CheckForCrouching();
            m_MonoBehaviour.CheckForGrounded();

            if (m_MonoBehaviour.CheckForFallInput())
                m_MonoBehaviour.MakePlatformFallthrough();
            else if (m_MonoBehaviour.CheckForAttackInput())
                m_MonoBehaviour.Attack(m_MonoBehaviour.aniCrouchingAttack);

            m_MonoBehaviour.GroundedVerticalMovement(deltaTime);
            m_MonoBehaviour.GroundedHorizontalMovement(false, deltaTime);
        }
    }
}