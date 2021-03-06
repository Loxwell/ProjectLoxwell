﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Mechanics.AI.StateMachine;
using Platformer.Mechanics;

namespace InGame.StateMachine.Player
{
    public class PlayerAttackSMB : SceneLinkedSMB<PlayerCharacter>
    {
        //int m_HashAirborneMeleeAttackState = Animator.StringToHash("AirborneMeleeAttack");

        public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.SetHorizontalMovement(0);
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float deltaTime = Time.deltaTime;
            m_MonoBehaviour.UpdateFacing();
            m_MonoBehaviour.UpdateJump(deltaTime);

            if (!m_MonoBehaviour.CheckForGrounded())
            {
                m_MonoBehaviour.AirborneHorizontalMovement(deltaTime);
                m_MonoBehaviour.AirborneVerticalMovement(deltaTime);
            }else
            {
                m_MonoBehaviour.GroundedHorizontalMovement(false, deltaTime);// Time.time - m_preTime);
            }
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.DisableMeleeAttack();
        }
    }
}
