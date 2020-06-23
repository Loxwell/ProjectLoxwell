
using UnityEngine;

using Platformer.Model;
using ScheduleSystem.Core;
using Platformer.GamePlay;

using static ScheduleSystem.Core.Simulation;
using static LSG.Utilities.BitField;

namespace Platformer.Mechanics
{

    public class PlayerMovementController : KinematicObject
    {
        const string INPUT_HORIZONTAL = "Horizontal";
        const string INPUT_JUMP = "Jump";
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public enum EActionState : byte
        {
            GROUNDED = 0, PREPARE_TO_JUMP = 1, JUMPING = 2, IN_FLIGHT = 3, LANDED = 4, FALL = 5, FREEZE = 6, CONTROL_ENABLED = 7
        }

        public EActionState CurrentJumpState
        {
            get { return m_jumpState; }
            set
            {
                if(value != m_jumpState)
                {
                    switch (value)
                    {
                        case EActionState.GROUNDED:
                            OnGrounded?.Invoke();
                            break;
                        case EActionState.IN_FLIGHT:
                            Schedule<PlayerJumped>().player = this;
                            OnFlight?.Invoke();
                            break;
                        case EActionState.JUMPING:
                            OnJumping?.Invoke();
                            break;
                        case EActionState.LANDED:
                            Schedule<PlayerLaneded>().player = this;
                            OnLanded?.Invoke();
                            break;
                        case EActionState.PREPARE_TO_JUMP:
                            OnPrepareToJump?.Invoke();
                            Bounce(m_jumpTakeOffSpeed * model.jumpModifier);
                            break;
                        case EActionState.FALL:
                            Schedule<PlayerStopJump>().player = this;
                            if (Velocity.y > 0)
                                Bounce(Velocity.y * model.jumpDeceleration);
                            break;
                    }

                    m_jumpState = value;
                }
            }
        }


        public bool ForwardTo
        {
            get {
                return !m_renderer.flipX;
            }
        }

        /// <summary>
        /// true 이동 및 방향 전환 금지
        /// false 이동 및 방향 전환 가능
        /// </summary>
        public bool Freezing
        {
            get
            {
                return IsMarkedFlag(m_state, (int)EActionState.FREEZE);
            }

            set
            {
                ControlEnabled = !value;

                if (value)
                    SetFlag(ref m_state, (int)EActionState.FREEZE);
                else
                    ReleaseFlag(ref m_state, (int)EActionState.FREEZE);

            }
        }

        /// <summary>
        /// true 이동 및 방향 전환 가능
        /// false 이동 금지 및 방향은 전환 가능
        /// </summary>
        public bool ControlEnabled
        {
            get
            {
                return IsMarkedFlag(m_state, (int)EActionState.CONTROL_ENABLED) && !Freezing;
            }

            set
            {
                if (value)
                    SetFlag(ref m_state, (int)EActionState.CONTROL_ENABLED);
                else
                    ReleaseFlag(ref m_state, (int)EActionState.CONTROL_ENABLED);
            }
        }


        public event System.Action OnGrounded;
        public event System.Action OnFlight;
        public event System.Action OnJumping;
        public event System.Action OnLanded;
        public event System.Action OnPrepareToJump;


        /// <summary>
        /// Max horizontal speed of the player
        /// </summary>
        [SerializeField, Header("Max horizontal speed"), Range(1e-5f, 20f)]
        float m_maxSpeed = 7;

        /// <summary>
        /// Initial jump velocity at the start of a jump
        /// </summary>
        [SerializeField, Header("Initial Jump velocity"), Range(1e-5f, 20f)]
        float m_jumpTakeOffSpeed = 7;

        SpriteRenderer m_renderer;
        EActionState m_jumpState;
        Vector2 m_move;

        int m_state;
        float m_faceTo;

        protected override void Awake()
        {
            base.Awake();
            ClearFlags(ref m_state);
            m_renderer = GetComponentInChildren<SpriteRenderer>();
            Freezing = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_jumpState = EActionState.GROUNDED;
            m_faceTo = 0;

#if UNITY_EDITOR
            Freezing = false;
#endif
        }

        protected override void Update()
        {
            m_move.x = Input.GetAxis(INPUT_HORIZONTAL);

            if (!Freezing)
                m_faceTo = m_move.x;
            else
                m_faceTo = 0;

            if (!ControlEnabled)
            { m_move.x = 0; }
            
            UpdateJumpState();
            base.Update();
        }

        protected override void ComputeVelocity()
        {
            if (m_faceTo > 0.01f)
                m_renderer.flipX = false;
            else if (m_faceTo < -0.01f)
                m_renderer.flipX = true;

            targetVelocity = m_move * m_maxSpeed;
        }

        void UpdateJumpState()
        {
            switch(CurrentJumpState)
            {
                case EActionState.PREPARE_TO_JUMP:
                    CurrentJumpState = EActionState.JUMPING;
                    break;
                case EActionState.JUMPING:
                    if (!IsGrounded)
                        CurrentJumpState = EActionState.IN_FLIGHT;
                    break;
                case EActionState.IN_FLIGHT:
                    if(IsGrounded)
                        CurrentJumpState = EActionState.GROUNDED;
                    break;
                case EActionState.LANDED:
                    CurrentJumpState = EActionState.GROUNDED;
                    break;
                case EActionState.GROUNDED:
                    if (Input.GetButtonDown(INPUT_JUMP))
                        CurrentJumpState = EActionState.PREPARE_TO_JUMP;
                    break;
                case EActionState.FALL:
                    if (IsGrounded)
                        CurrentJumpState = EActionState.GROUNDED;
                    break;
            }

            if(CurrentJumpState != EActionState.GROUNDED && Input.GetButtonUp(INPUT_JUMP))
                CurrentJumpState = EActionState.FALL;
        }
    }
}
