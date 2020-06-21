
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

        public enum EJumpState : byte
        {
            GROUNDED = 0, PREPARE_TO_JUMP = 1, JUMPING = 2, IN_FLIGHT = 3, LANDED = 4, FALL = 5
        }

        public EJumpState CurrentJumpState
        {
            get { return m_jumpState; }
            set
            {
                if(value != m_jumpState)
                {
                    switch (value)
                    {
                        case EJumpState.GROUNDED:
                            OnGrounded?.Invoke();
                            break;
                        case EJumpState.IN_FLIGHT:
                            Schedule<PlayerJumped>().player = this;
                            OnFlight?.Invoke();
                            break;
                        case EJumpState.JUMPING:
                            OnJumping?.Invoke();
                            break;
                        case EJumpState.LANDED:
                            Schedule<PlayerLaneded>().player = this;
                            OnLanded?.Invoke();
                            break;
                        case EJumpState.PREPARE_TO_JUMP:
                            OnPrepareToJump?.Invoke();
                            Bounce(m_jumpTakeOffSpeed * model.jumpModifier);
                            break;
                        case EJumpState.FALL:
                            Schedule<PlayerStopJump>().player = this;
                            if (Velocity.y > 0)
                                Bounce(Velocity.y * model.jumpDeceleration);
                            break;
                    }

                    m_jumpState = value;
                }
            }
        }

        public event System.Action OnGrounded;
        public event System.Action OnFlight;
        public event System.Action OnJumping;
        public event System.Action OnLanded;
        public event System.Action OnPrepareToJump;

        public bool controlEnabled;

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
        EJumpState m_jumpState;
        Vector2 m_move, m_weightedVelocity;

        uint m_state;

        protected override void Awake()
        {
            base.Awake();
            m_renderer = GetComponentInChildren<SpriteRenderer>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_jumpState = EJumpState.GROUNDED;

#if UNITY_EDITOR
            controlEnabled = true;
#endif
        }

        protected override void Update()
        {
            if (controlEnabled)
                m_move.x = Input.GetAxis(INPUT_HORIZONTAL);
            else
                m_move.x = 0;

            UpdateJumpState();
            base.Update();
        }

        public void AdditiveVelocity(Vector2 weightedVelocity)
        {
            this.m_weightedVelocity = weightedVelocity;
        }

        protected override void ComputeVelocity()
        {
            if (m_move.x > 0.01f)
                m_renderer.flipX = false;
            else if (m_move.x < -0.01f)
                m_renderer.flipX = true;

            targetVelocity = m_move * m_maxSpeed + m_weightedVelocity;
            m_weightedVelocity = Vector2.zero;
        }

        void UpdateJumpState()
        {
            switch(CurrentJumpState)
            {
                case EJumpState.PREPARE_TO_JUMP:
                    CurrentJumpState = EJumpState.JUMPING;
                    break;
                case EJumpState.JUMPING:
                    if (!IsGrounded)
                        CurrentJumpState = EJumpState.IN_FLIGHT;
                    break;
                case EJumpState.IN_FLIGHT:
                    if(IsGrounded)
                        CurrentJumpState = EJumpState.GROUNDED;
                    break;
                case EJumpState.LANDED:
                    CurrentJumpState = EJumpState.GROUNDED;
                    break;
                case EJumpState.GROUNDED:
                    if (Input.GetButtonDown(INPUT_JUMP))
                        CurrentJumpState = EJumpState.PREPARE_TO_JUMP;
                    break;
                case EJumpState.FALL:
                    if (IsGrounded)
                        CurrentJumpState = EJumpState.GROUNDED;
                    break;
            }

            if(CurrentJumpState != EJumpState.GROUNDED && Input.GetButtonUp(INPUT_JUMP))
                CurrentJumpState = EJumpState.FALL;
        }
    }
}
