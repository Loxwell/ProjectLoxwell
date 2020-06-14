
using UnityEngine;

using Platformer.Model;
using ScheduleSystem.Core;
using Platformer.GamePlay;

using static ScheduleSystem.Core.Simulation;
using static LSG.Utilities.BitField;

namespace Platformer.Mechanics
{

    public class PlayerController : KinematicObject
    {
        const string INPUT_HORIZONTAL = "Horizontal";
        //const string INPUT_VERTICAL = "Vertical";

        enum EJumpState : byte
        {
            GROUNDED = 0, PREPARE_TO_JUMP, JUMPING, IN_FLIGHT, LANDED
        }

        public enum EState : byte
        {
            ENABLE_CONTROLLER, STOP_JUMP, JUMP
        }

        EJumpState JumpState
        {
            get { return m_jumpState; }
            set
            {
                if(value != m_jumpState)
                {

                    switch (m_jumpState)
                    {
                        case EJumpState.GROUNDED:
                            OnGrounded?.Invoke();
                            break;
                        case EJumpState.IN_FLIGHT:
                            OnFlight?.Invoke();
                            break;
                        case EJumpState.JUMPING:
                            Schedule<PlayerJumped>().player = this;
                            OnJumping?.Invoke();
                            break;
                        case EJumpState.LANDED:
                            Schedule<PlayerLaneded>().player = this;
                            OnLanded?.Invoke();
                            break;
                        case EJumpState.PREPARE_TO_JUMP:
                            OnPrepareToJump?.Invoke();
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

        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

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
        Vector2 m_move;

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
            MarkFlag(ref m_state, (int)EState.ENABLE_CONTROLLER);
        }

        protected override void Update()
        {
            if (IsMarkedFlag(m_state, (int)EState.ENABLE_CONTROLLER))
            {
                m_move.x = Input.GetAxis(INPUT_HORIZONTAL);
            }

            UpdateJumpState();
            base.Update();
        }

        public void AddState(EState additionState)
        {
            MarkFlag(ref m_state, (int)additionState);
        }

        public void ReleaseState(EState deletionState)
        {
            ReleaseFlag(ref m_state, (int)deletionState);
        }

        protected override void ComputeVelocity()
        {
            if (m_move.x > 0.01f)
                m_renderer.flipX = false;
            else if (m_move.x < -0.01f)
                m_renderer.flipX = true;

            targetVelocity = m_move * m_maxSpeed;
        }

        void UpdateJumpState()
        {
            switch(JumpState)
            {
                case EJumpState.PREPARE_TO_JUMP:
                    JumpState = EJumpState.JUMPING;
                    break;
                case EJumpState.JUMPING:
                    if (!IsMarkedFlag(m_state, (int)EJumpState.GROUNDED))
                        JumpState = EJumpState.IN_FLIGHT;
                    break;
                case EJumpState.IN_FLIGHT:
                    break;
                case EJumpState.LANDED:
                    break;
                case EJumpState.GROUNDED:
                    break;
            }
        }
    }
}
