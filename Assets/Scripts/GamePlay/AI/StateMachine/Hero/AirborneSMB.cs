using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mechanics.AI.StateMachine;
using Platformer.Player;

namespace GamePlay.StateMachine
{
    public class AirborneSMB : SceneLinkedSMB<PlayerCharacter>
    {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float deltaTime = Time.deltaTime;//Time.time - preTime;

            m_MonoBehaviour.UpdateFacing();
            m_MonoBehaviour.UpdateJump(deltaTime);
            m_MonoBehaviour.AirborneHorizontalMovement(deltaTime);
            m_MonoBehaviour.AirborneVerticalMovement(deltaTime);
            m_MonoBehaviour.CheckForGrounded();
            if (m_MonoBehaviour.CheckForAttackInput())
                m_MonoBehaviour.Attack(m_MonoBehaviour.aniJumpingAttack);
            m_MonoBehaviour.CheckForCrouching();
        }
    }
}

