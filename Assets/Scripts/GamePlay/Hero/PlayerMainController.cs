using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSG.LWBehaviorTree;
using System;
using MovementController = Platformer.Mechanics.PlayerMovementController;
using AnimatorHelper = LSG.Utilities.AnimatorHelper;

using static LSG.Utilities.BitField;

namespace LSG
{
    [System.Obsolete("사용안함")]
    public partial class PlayerMainController : MonoBehaviour, IBlackboard,
        IEquatable<int>, IEquatable<PlayerMainController>, IEquatable<string>
    {
        internal enum EState
        {
            ERROR = -1, IDLE = 0, JUMP_BEGINS = 1, FALL = 2, CROUCH = 6, JUMP_CLIMB = 10,
            ATTACK = 20
        }

        internal Animator Animator
        {
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
            {
                if (m_state != value)
                {
                    m_state = value;
                    Animator.SetInteger(m_hashState, m_cachedAniState = (int)value);
                }
            }

            get
            {
                //if(Animator)
                //    return (EState)Animator.GetInteger(m_hashState);
                // 현재 외부에서 확인 되는 상태
                return m_state;
            }
        }


        internal bool Freezing
        {
            set
            {
                m_controller.Freezing = value;
            }
        }

        internal bool ControlEnabled
        {
            get { return m_controller.ControlEnabled; }
            set
            {
                m_controller.ControlEnabled = value;
            }
        }

#pragma warning disable
        [SerializeField]
        PlayerBlackboard heroBB;

        private MovementController m_controller;
        private Animator m_animator;
        private AnimatorHelper m_attackMotion;
        private EState m_state;
        private int m_hashSpeed, m_hashState, m_cachedAniState;
        private uint m_inputstate;

        private void Awake()
        {
            m_controller = GetComponent<MovementController>();
            m_attackMotion = new AnimatorHelper(Animator);

            m_hashState = Animator.StringToHash("State");
            m_hashSpeed = Animator.StringToHash("RunningSpeed");

            m_controller.OnGrounded += OnGrounded;
            m_controller.OnLanded += OnGrounded;
            m_controller.OnPrepareToJump += OnJumping;
            m_controller.OnFlight += OnFall;
        }

        void OnEnable()
        {
            Initialize();
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
            Animator.SetFloat(m_hashSpeed, Mathf.Abs(m_controller.Velocity.x));
        }

        public void SetStateCrouch()
        {

        }

        public void SetStateIdle()
        {

        }

        public void Attack(AnimationClip attackClip)
        {
            m_attackMotion.ChangeClip("Ani_HeroAttack", attackClip);
            CurrentState = EState.ATTACK;
        }

        public bool Equals(string aniStateFullName)
        {
            return Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash(aniStateFullName);
        }

        public bool Equals(int aniHashFullPath)
        {
            return Animator.GetCurrentAnimatorStateInfo(0).fullPathHash == aniHashFullPath;
        }

        public bool Equals(PlayerMainController other)
        {
            return base.Equals(other);
        }

        public void Initialize()
        {
            //heroBB.isGrounded = false;
            m_state = EState.ERROR;
            m_cachedAniState = (int)EState.ERROR;
        }

        void OnJumping()
        {
            //m_state = EState.JUMP_BEGINS;
            CurrentState = EState.JUMP_BEGINS;
        }

        void OnGrounded()
        {
            //m_state = EState.IDLE;
            CurrentState = EState.IDLE;
        }

        void OnFall()
        {
            //m_state = EState.IDLE;
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



