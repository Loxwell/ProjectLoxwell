using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSG.LWBehaviorTree;

using MovementController = Platformer.Mechanics.PlayerMovementController;
using EJumpState = Platformer.Mechanics.PlayerMovementController.EActionState;
using static LSG.Utilities.BitField;


namespace LSG
{
    public partial class PlayerAniController : MonoBehaviour, IBlackboard
    {
        public TextMesh debugText;
        public TextMesh heroState;

        public void Debug(string v)
        {
            debugText.text = v;
        }


        const string INPUT_VERTICAL = "Vertical";

        internal enum EState
        {
            ERROR = -1, IDLE = 0, JUMP_BEGINS = 1, FALL = 2 , CROUCH = 6, JUMP_CLIMB = 10,
            ATTACK = 20
         }

        internal Animator Animator {
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
            { Animator.SetInteger(m_hashState, (int)value); }

            get
            {
                if(Animator)
                    return (EState)Animator.GetInteger(m_hashState);
                return (EState) (-1);
            }
        }

        internal AnimatorStateInfo StateInfo
        {
            get
            {
                return Animator.GetCurrentAnimatorStateInfo(0);
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

        void RunningSpeed (float value) => Animator.SetFloat(m_hashSpeed, value);

#pragma warning disable
        [SerializeField]
        HeroBlackboard heroBB;

        private MovementController m_controller;
        private Animator m_animator;
        private HeroBT m_bt;

        int m_hashSpeed, m_hashState;
        uint m_inputstate;   

        private void Awake()
        {
            m_controller = GetComponent<MovementController>();
          
            m_hashState = Animator.StringToHash("State");
            m_hashSpeed = Animator.StringToHash("RunningSpeed");

            m_controller.OnGrounded += OnGrounded;
            m_controller.OnLanded += OnGrounded;
            m_controller.OnPrepareToJump+= OnJumping;
            m_controller.OnFlight += OnFall;

            heroBB.controller = this;
            m_bt = new HeroBT(heroBB);
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

            heroState.text = CurrentState.ToString();

            RunningSpeed(Mathf.Abs( m_controller.Velocity.x ));
            m_bt.Update();
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