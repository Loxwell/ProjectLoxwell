using LSG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TileBase = UnityEngine.Tilemaps.TileBase;
using AnimatorHelper = LSG.Utilities.AnimatorHelper;
using PhysicsHelper = Platformer.Mechanics.Helper.PhysicsHelper;
using FallthroughReseter = Platformer.Mechanics.FallthroughReseter;
using Damager = Platformer.Module.Damager;
using Damageable = Platformer.Module.Damageable;
using MovementController = Platformer.Core.MovementController;
using Platformer.Mechanics.AI.StateMachine;

namespace Platformer.Mechanics.AI
{
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(Animator))]
    public class PlayerCharacter : MonoBehaviour
    {
        protected const float MIN_HURT_JUMP_ANGLE = 0.001f;
        protected const float MAX_HURT_JUMP_ANGLE = 89.999f;

        protected readonly int HASH_PARAM_ATTACK = Animator.StringToHash("Attack");
        protected readonly int HASH_PARAM_GROUNDED = Animator.StringToHash("Grounded");
        protected readonly int HASH_PARAM_CROUCHING = Animator.StringToHash("Crouching");
        protected readonly int HASH_PARAM_MOVING_SPEED = Animator.StringToHash("RunningSpeed");
        protected readonly int HASH_PARAM_JUMP_VELOCITY = Animator.StringToHash("JumpVelocity");
        /// <summary>
        /// This is to help the character stick to vertically moving platforms.
        /// </summary>
        protected const float GROUNDED_STICKING_VELOCITY_MULTIPLIER = 3f;

        public static PlayerCharacter PlayerInstance { get { return s_PlayerInstance; } }
        protected static PlayerCharacter s_PlayerInstance;

        [SerializeField]
        public SpriteRenderer spriteRenderer;
        [SerializeField]
        public Transform cameraFollowTarget;
        [SerializeField]
        public Damager meleeDamager;
        [SerializeField]
        public Damageable damageable;
        [SerializeField]
        public PlayerInputManager inputManager;

        [SerializeField]
        public float maxSpeed = 10f;
        [SerializeField]
        public float groundAcceleration = 100f;
        [SerializeField]
        public float groundDeceleration = 100f;

        [Range(0f, 1f), SerializeField]
        public float airborneAccelProportion;
        [Range(0f, 1f), SerializeField]
        public float airborneDecelProportion;

        #region JUMP
        [SerializeField]
        public float gravity = 50f;
        [SerializeField, Header("중력 감쇄량"),  Range(1e-7f, 1f)]
        public float gravityReduction = 1;
        [SerializeField]
        public float jumpSpeed = 20f;
        [SerializeField]
        public float jumpAbortSpeedReduction = 100f;
        [Range(MIN_HURT_JUMP_ANGLE, MAX_HURT_JUMP_ANGLE), SerializeField]
        public float hurtJumpAngle = 45f;
        #endregion

        public float flickeringDuration = 0.1f;

        #region EventTrigger
        public System.Action onHurt;
        public System.Action onBeginAttack;
        public System.Action onAttack;
        public System.Action onEndAttack;

        #endregion

        #region CAMERA
        [SerializeField]
        public float cameraHorizontalFacingOffset;
        [SerializeField]
        public float cameraHorizontalSpeedOffset;
        [SerializeField]
        public float cameraVerticalInputOffset;
        [SerializeField]
        public float maxHorizontalDeltaDampTime;
        [SerializeField]
        public float maxVerticalDeltaDampTime;
        [SerializeField]
        public float verticalCameraOffsetDelay;
        #endregion

        [SerializeField]
        public bool spriteOriginallyFacesLeft = false;

        [SerializeField]
        public AnimationClip aniStandAttack;
        [SerializeField]
        public AnimationClip aniCrouchingAttack;
        [SerializeField]
        public AnimationClip aniJumpingAttack;

        protected AnimatorHelper m_animator;
        // UnityEngine.CharacterController 이 존재함
        protected MovementController m_CharacterController;
        protected Collider2D m_collider;
        protected Transform m_Transform;
        protected TileBase m_CurrentSurface;

        /// <summary>
        /// /used in non alloc version of physic function
        /// </summary>
        protected ContactPoint2D[] m_ContactsBuffer = new ContactPoint2D[16];
        protected Vector2 m_MoveVector;
        protected Vector2 m_StartingPosition = Vector2.zero;

        protected float m_TanHurtJumpAngle;
        protected float m_CamFollowHorizontalSpeed;
        protected float m_CamFollowVerticalSpeed;
        protected float m_VerticalCameraOffsetTimer;

        protected bool m_StartingFacingLeft = false;
        protected bool m_InPause = false;

        void Awake()
        {
            s_PlayerInstance = this;

            m_CharacterController = GetComponent<MovementController>();
            m_animator = new AnimatorHelper(GetComponent<Animator>());
            m_collider = GetComponent<CapsuleCollider2D>();
            m_Transform = transform;

            if(!inputManager)
                inputManager = GetComponent<PlayerInputManager>();
        }

        private void OnEnable()
        {
            m_MoveVector = Vector2.up * GROUNDED_STICKING_VELOCITY_MULTIPLIER * -1;
            m_animator.Animator.SetBool(HASH_PARAM_GROUNDED, true);
        }

        void Start()
        {
            hurtJumpAngle = Mathf.Clamp(hurtJumpAngle, MIN_HURT_JUMP_ANGLE, MAX_HURT_JUMP_ANGLE);
            m_TanHurtJumpAngle = Mathf.Tan(Mathf.Deg2Rad * hurtJumpAngle);
           
            if (!Mathf.Approximately(maxHorizontalDeltaDampTime, 0f))
            {
                float maxHorizontalDelta = maxSpeed * cameraHorizontalSpeedOffset + cameraHorizontalFacingOffset;
                m_CamFollowHorizontalSpeed = maxHorizontalDelta / maxHorizontalDeltaDampTime;
            }

            if (!Mathf.Approximately(maxVerticalDeltaDampTime, 0f))
            {
                float maxVerticalDelta = cameraVerticalInputOffset;
                m_CamFollowVerticalSpeed = maxVerticalDelta / maxVerticalDeltaDampTime;
            }

            SceneLinkedSMB<PlayerCharacter>.Initialise(m_animator.Animator, this);
            m_MoveVector = Vector2.up * GROUNDED_STICKING_VELOCITY_MULTIPLIER * -1;
            m_StartingPosition = transform.position;
            m_StartingFacingLeft = GetFacing() < 0.0f;
        }

        private void FixedUpdate()
        {
            m_CharacterController.Move(m_MoveVector * Time.deltaTime);
            m_animator.Animator.SetFloat(HASH_PARAM_MOVING_SPEED, Mathf.Abs( m_MoveVector.x) );
            m_animator.Animator.SetFloat(HASH_PARAM_JUMP_VELOCITY, m_MoveVector.y);
        }

        /// <summary>
        /// called mostly by StateMachineBehaviours in the character's Animator Controller but also by Events.
        /// </summary>
        /// <param name="newMoveVector"></param>
        public void SetMoveVector(Vector2 newMoveVector) => m_MoveVector = newMoveVector;

        public void SetHorizontalMovement(float newHorizontalMovement) => m_MoveVector.x = newHorizontalMovement;

        public void SetVerticalMovement(float newVerticalMovement) => m_MoveVector.y = newVerticalMovement;

        public void IncrementMovement(Vector2 additionalMovement) => m_MoveVector += additionalMovement;

        public void IncrementHorizontalMovement(float additionalHorizontalMovement) => m_MoveVector.x += additionalHorizontalMovement;

        public void IncrementVerticalMovement(float additionalVerticalMovement) => m_MoveVector.y += additionalVerticalMovement;

        public void GroundedVerticalMovement(float deltaTime)
        {
            UpdateGravity(deltaTime); //m_MoveVector.y -= gravity * deltaTime * gravityReduction;

            if (m_MoveVector.y < -gravity * deltaTime * GROUNDED_STICKING_VELOCITY_MULTIPLIER)
            {
                m_MoveVector.y = -gravity * deltaTime * GROUNDED_STICKING_VELOCITY_MULTIPLIER;
            }
        }
        
        public void TeleportToColliderBottom()
        {
            Vector2 colliderBottom = m_CharacterController.Rigidbody2D.position + m_collider.offset 
                + Vector2.down * m_collider.bounds.extents.y;
            m_CharacterController.Teleport(colliderBottom);
        }

        public void OnHurt(Damager damager, Damageable damageable)
        {
            //if the player don't have control, we shouldn't be able to be hurt as this wouldn't be fair
            //if (!PlayerInput.Instance.HaveControl)
            //    return;

            UpdateFacing(damageable.GetDamageDirection().x > 0f);
            damageable.EnableInvulnerability();



            //we only force respawn if helath > 0, otherwise both forceRespawn & Death trigger are set in the animator, messing with each other.
            if (damageable.CurrentHealth > 0 && damager.forceRespawn)
            {
                // Respawn
                // m_Animator.SetTrigger(m_HashForcedRespawnPara);
            }
            
            //m_Animator.SetTrigger(m_HashHurtPara);
            //m_Animator.SetBool(m_HashGroundedPara, false);
            //hurtAudioPlayer.PlayRandomSound();
            onHurt?.Invoke();


            //if the health is < 0, mean die callback will take care of respawn
            if (damager.forceRespawn && damageable.CurrentHealth > 0)
            {
                //StartCoroutine(DieRespawnCoroutine(false, true));
            }
        }

        public void OnDie()
        {
            gameObject.SetActive(false);
        }

        public Vector2 GetMoveVector() => m_MoveVector;

        /// <summary>
        /// 애니메이터 상태 설정 확인 해볼것
        /// </summary>
        /// <returns></returns>
        public bool IsFalling() => m_MoveVector.y< 0f && !m_animator.Animator.GetBool(HASH_PARAM_GROUNDED);

        public float GetFacing() => spriteRenderer.flipX != spriteOriginallyFacesLeft? -1f : 1f;

        public void UpdateFacing()
        {
            bool faceLeft = inputManager.keyLeft.IsKeyDown || inputManager.keyLeft.IsPressing;
            bool faceRight = inputManager.keyRight.IsKeyDown || inputManager.keyRight.IsPressing ;

            if (faceLeft)
            {
                spriteRenderer.flipX = !spriteOriginallyFacesLeft;
            }
            else if (faceRight)
            {
                spriteRenderer.flipX = spriteOriginallyFacesLeft;
            }
        }

        public void UpdateFacing(bool faceLeft)
        {
            if (faceLeft)
            {
                spriteRenderer.flipX = !spriteOriginallyFacesLeft;
                //m_CurrentBulletSpawnPoint = facingLeftBulletSpawnPoint;
            }
            else
            {
                spriteRenderer.flipX = spriteOriginallyFacesLeft;
                //m_CurrentBulletSpawnPoint = facingRightBulletSpawnPoint;
            }
        }

        #region ATTACK
        public bool CheckForAttackInput()
        {
            return inputManager.keyAction1.IsKeyDown;
        }

        public void Attack(AnimationClip attackClip)
        {
            // Ani_HeroAttack - Animator에 등록 되어 있는 애니메이션 클립명
            // 클립 이름을 Key로 사용
            m_animator.ChangeClip("Ani_HeroAttack", attackClip);
            m_animator.Animator.SetTrigger(HASH_PARAM_ATTACK);
        }

        public void EnableMeleeAttack()
        {
            meleeDamager.EnableDamage();
            meleeDamager.disableDamageAfterHit = true;
        }

        public void DisableMeleeAttack()
        {
            meleeDamager.DisableDamage();
        }
        #endregion

        public void GroundedHorizontalMovement(bool useInput, float deltaTime, float speedScale = 1f)
        {
            int dir = inputManager.HorizontaAxis;
            float desiredSpeed = useInput ? dir * maxSpeed * speedScale : 0f;
            float acceleration = useInput && (dir != 0) ? groundAcceleration : groundDeceleration;
            m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * deltaTime);
        }

        public void CheckForCrouching()
        {
            m_animator.Animator.SetBool(HASH_PARAM_CROUCHING, inputManager.keyDown.IsPressing);
        }

        public bool CheckForGrounded()
        {
            bool grounded = m_CharacterController.IsGrounded;

            if (grounded)
            {
                FindCurrentSurface();

                if (!m_animator.Animator.GetBool(HASH_PARAM_GROUNDED) && m_MoveVector.y < -1.0f)
                {
                    // only play the landing sound if falling "fast" enough (avoid small bump playing the landing sound)
                    // 착지음
                    //landingAudioPlayer.PlayRandomSound(m_CurrentSurface);
                }
            }
            else
                m_CurrentSurface = null;

            m_animator.Animator.SetBool(HASH_PARAM_GROUNDED, grounded);

            return grounded;
        }

        public void FindCurrentSurface()
        {
            Collider2D groundCollider = m_CharacterController.GroundColliders[0];

            if (groundCollider == null)
                groundCollider = m_CharacterController.GroundColliders[1];

            if (groundCollider == null)
                return;

            TileBase b = PhysicsHelper.FindTileForOverride(groundCollider, transform.position, Vector2.down);
            if (b != null)
                m_CurrentSurface = b;
        }

        public void UpdateJump(float deltaTime)
        {
            if (!CheckForJumpInput() && m_MoveVector.y > 0.0f)
            {
                m_MoveVector.y -= jumpAbortSpeedReduction * deltaTime;
            }
        }

        public void AirborneHorizontalMovement(float deltaTime)
        {
            float desiredSpeed = inputManager.HorizontaAxis * maxSpeed;

            float acceleration;

            if (desiredSpeed != 0) //PlayerInput.Instance.Horizontal.ReceivingInput
                acceleration = groundAcceleration * airborneAccelProportion;
            else
                acceleration = groundDeceleration * airborneDecelProportion;

            m_MoveVector.x = Mathf.MoveTowards(m_MoveVector.x, desiredSpeed, acceleration * deltaTime);
        }

        public void AirborneVerticalMovement(float deltaTime)
        {
            if (Mathf.Approximately(m_MoveVector.y, 0f) || m_CharacterController.IsCeilinged && m_MoveVector.y > 0f)
                m_MoveVector.y = 0f;
            UpdateGravity(deltaTime); //m_MoveVector.y -= gravity * deltaTime;
        }

        public bool CheckForJumpInput() => inputManager.keyJump.IsKeyDown;

        public bool CheckForFallInput() => inputManager.VerticalAxis < -float.Epsilon && CheckForJumpInput();

        // Flatform 아래로 뛰는 함수
        public bool MakePlatformFallthrough()
        {
            int colliderCount = 0;
            int fallthroughColliderCount = 0;

            for (int i = 0, len = m_CharacterController.GroundColliders.Length; i < len; i++)
            {
                Collider2D col = m_CharacterController.GroundColliders[i];
                if (col == null)
                    continue;

                ++colliderCount;

                if (PhysicsHelper.ColliderHasPlatformEffector(col))
                    ++fallthroughColliderCount;
            }

            if (fallthroughColliderCount == colliderCount)
            {
                for (int i = 0; i < m_CharacterController.GroundColliders.Length; i++)
                {
                    Collider2D col = m_CharacterController.GroundColliders[i];
                    if (col == null)
                        continue;

                    PlatformEffector2D effector;
                    PhysicsHelper.TryGetPlatformEffector(col, out effector);
                    FallthroughReseter reseter = effector.gameObject.AddComponent<FallthroughReseter>();
                    reseter.StartFall(effector);

                    //set invincible for half a second when falling through a platform, as it will make the player "standup"
                    StartCoroutine(FallThroughtInvincibility());
                }
            }

            return fallthroughColliderCount == colliderCount;
        }

        IEnumerator FallThroughtInvincibility()
        {
            damageable.EnableInvulnerability(true);
            yield return new WaitForSeconds(0.5f);
            damageable.DisableInvulnerability();
        }


        void UpdateGravity(float deltaTime)
        {
            m_MoveVector.y -=( gravity * deltaTime * gravityReduction );
        }
    }
}
