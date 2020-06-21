using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSG.LWBehaviorTree;

using MovementController = Platformer.Mechanics.PlayerMovementController;
using EJumpState = Platformer.Mechanics.PlayerMovementController.EJumpState;
using static LSG.Utilities.BitField;

namespace LSG
{
    public partial class PlayerAniController : MonoBehaviour, IBlackboard
    {
        const string INPUT_VERTICAL = "Vertical";

        internal enum EState
        {
            ERROR = -1, IDLE = 0, JUMP_BEGINS = 1, FALL = 2 , CROUCH = 6, JUMP_CLIMB = 10,
            ATTACK = 20
         }

        internal Animator HeroAnimator {
            get
            {
                if (!m_animator)
                    m_animator = GetComponent<Animator>();
                return m_animator;
            }
        }

        internal EState CurrentState
        {
            set
            { HeroAnimator.SetInteger(m_hashState, (int)value); }

            get
            {
                if(HeroAnimator)
                    return (EState)HeroAnimator.GetInteger(m_hashState);
                return (EState) (-1);
            }
        }

        void RunningSpeed (float value) => HeroAnimator.SetFloat(m_hashSpeed, value);

        MovementController m_controller;
        Animator m_animator;
        HeroBT m_bt;

        int m_hashSpeed, m_hashState;
        uint m_state;   

        private void Awake()
        {
            m_controller = GetComponent<MovementController>();
          
            m_hashState = Animator.StringToHash("State");
            m_hashSpeed = Animator.StringToHash("RunningSpeed");

            m_controller.OnGrounded += OnGrounded;
            m_controller.OnLanded += OnGrounded;
            m_controller.OnPrepareToJump+= OnJumping;
            m_controller.OnFlight += OnFall;

            m_bt = new HeroBT();
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
            RunningSpeed(Mathf.Abs( m_controller.Velocity.x ));
            m_bt.Update(this);
        }

        void OnJumping()
        {
            CurrentState = EState.JUMP_BEGINS;
        }

        void OnGrounded()
        {
            CurrentState = EState.IDLE;
        }

        void OnFall()
        {
            CurrentState = EState.FALL;
        }
    }
}


#region
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
#endregion