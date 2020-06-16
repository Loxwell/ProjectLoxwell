﻿using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using MovementController = Platformer.Mechanics.PlayerMovementController;
using EJumpState = Platformer.Mechanics.PlayerMovementController.EJumpState;
using UnityEditor;
using UnityEditorInternal;

namespace LSG
{
    public class PlayerAnimator : MonoBehaviour
    {
        enum EState
        {
            ERROR = -1, DEFAULT = 0, JUMP_BEGINS = 1, FALL = 2
        }

        Animator Animator {
            get
            {
                if (!m_animator)
                    m_animator = GetComponent<Animator>();
                return m_animator;
            }
        }
        

        EState AnimatorState
        {
            set
            { Animator.SetInteger(m_hashState, (int)value); }

            get
            {
                if(m_animator)
                    return (EState) m_animator.GetInteger(m_hashState);
                return (EState) (-1);
            }
        }

        void RunningSpeed (float value) => m_animator.SetFloat(0, value);

        MovementController m_controller;
        Animator m_animator;
        int m_hashSpeed, m_hashState;

        private void Awake()
        {
            m_controller = GetComponent<MovementController>();
          

            m_hashState = Animator.StringToHash("State");
            m_hashSpeed = Animator.StringToHash("RunningSpeed");

            m_controller.OnGrounded += OnGrounded;
            m_controller.OnLanded += OnGrounded;
            m_controller.OnPrepareToJump+= OnJumping;
            m_controller.OnFlight += OnFall;
        }

        private void OnDestroy()
        {
            m_controller.OnGrounded -= OnGrounded;
            m_controller.OnLanded -= OnGrounded;
            m_controller.OnPrepareToJump -= OnJumping;
            m_controller.OnFlight -= OnFall;
        }


        private void Update()
        {
            Animator.SetFloat(m_hashSpeed, Mathf.Abs( m_controller.Velocity.x ));
        }

        void OnJumping()
        {
            AnimatorState = EState.JUMP_BEGINS;
        }

        void OnGrounded()
        {
            AnimatorState = EState.DEFAULT;
        }

        void OnFall()
        {
            AnimatorState = EState.FALL;
        }
    }
}

/*    void AdditiveAnimFix()
    {
        float skiptime;
        float timeMult = 2f;
        a = anim.GetCurrentAnimatorStateInfo(1);
        b = anim.GetCurrentAnimatorClipInfo(1);
        if (b.Length > 0)
        {
            c = b[0].clip;
 
            float normalizedSkipTime = (1f / c.frameRate) * timeMult;
            normalizedSkipTime /= c.length;
            if (a.normalizedTime % 1 < normalizedSkipTime)
            {
                //print("FirstFrame =" + normalizedSkipTime);
                anim.Play(a.shortNameHash, 1, normalizedSkipTime);
                anim.Update(Time.deltaTime);
            }
        }
        previousVelocity = body.velocity;
       
    }*/
