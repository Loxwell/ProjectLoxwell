using Platformer.Module;
using System.Collections.Generic;
using UnityEngine;
using Platformer;
using Platformer.Mechanics.AI.StateMachine;
using Platformer.Utils;
using System.Runtime.InteropServices.WindowsRuntime;

#if UNITY_EDITOR
using UnityEditor;
#endif

using MovementController = Platformer.Core.MovementController;

namespace Platformer.Mechanics.AI
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Collider2D))]
    public class ThingBehaviour : MonoBehaviour
    {
        static Collider2D[] s_ColliderCache = new Collider2D[16];

        public Vector3 moveVector { get { return m_MoveVector; } }
        [HideInInspector]
        public Transform target;

        [Tooltip("If the sprite face left on the spritesheet, enable this. Otherwise, leave disabled")]
        public bool spriteFaceLeft = false;

        [Header("Movement")]
        public float speed;
        public float gravity = 10f;

        [Header("References")]
        [Tooltip("If the enemy will be using ranged attack, set a prefab of the projectile it should use")]
        public Mechanics.Projectile projectilePrefab;

        [Range(0.0f, 360.0f), Header("Scanning settings"),Tooltip("The angle of the forward of the view cone. 0 is forward of the sprite, 90 is up, 180 behind etc.")]
        public float viewDirection = 0.0f;
        [Range(0.0f, 360.0f)]
        public float viewFov;
        [Range(0.0f, 360.0f)]
        public float viewDistance;
        [Tooltip("Time in seconds without the target in the view cone before the target is considered lost from sight")]
        public float timeBeforeTargetLost = 3.0f;

        [Header("Melee Attack Data")]
        [EnemyMeleeRangeCheck]
        public float meleeRange = 3.0f;

        // 캐릭터 플레이어와 공통
        public Damager meleeDamager;
        // 전용
        public Damager contactDamager;

        [Tooltip("if true, the enemy will jump/dash forward when it melee attack")]
        public bool attackDash;

        [Tooltip("The force used by the dash")]
        public Vector2 attackForce;

        [Header("Range Attack Data")]
        [Tooltip("From where the projectile are spawned")]
        public Transform shootingOrigin;

        [Header("Misc")] // miscellaneous 이것저것 다양한
        [Tooltip("Time in seconds during which the enemy flicker after being hit")]
        public float flickeringDuration;

        /// <summary>
        /// FIxed Update Event
        /// </summary>
        public event System.Action OnFIxedUpdate;
        /// <summary>
        /// 근거리 공격 시점
        /// </summary>
        public event System.Action OnAttackingBegins;
        /// <summary>
        /// 원거리 공격 시작 시점
        /// </summary>
        public event System.Action OnShootngBegins;
        
        /// <summary>
        /// 원거리 공격 시점
        /// </summary>
        public event System.Action OnShoot;
        /// <summary>
        /// 타겟 놓침
        /// </summary>
        public event System.Action OnForgetTarget;
        /// <summary>
        /// 피격
        /// </summary>
        public event System.Action OnTakeDamaged;
        /// <summary>
        /// 걷는 시점
        /// </summary>
        public event System.Action OnFootStep;
        /// <summary>
        /// 대상 확인
        /// </summary>
        public event System.Action OnSpottedTarget;
        /// <summary>
        /// 죽는 시점
        /// </summary>
        public event System.Action OnDie;
             
        protected SpriteRenderer m_SpriteRenderer;
        // class UnityEngine.CharacterController 이 존재함
        protected MovementController m_movementController;
        protected Collider2D m_Collider;
        protected Animator m_Animator;
        protected Coroutine m_FlickeringCoroutine = null;

        protected RaycastHit2D[] m_RaycastHitCache = new RaycastHit2D[8];
        protected Bounds m_LocalBounds;
        protected Color m_OriginalColor;
        protected ContactFilter2D m_Filter;
        //as we flip the sprite instead of rotating/scaling the object, this give the forward vector according to the sprite orientation
        protected Vector2 m_SpriteForward;
        protected Vector3 m_MoveVector;
        protected Vector3 m_LocalDamagerPosition;
        protected Vector3 m_TargetShootPosition;
        
        protected float m_TimeSinceLastTargetView;
        protected float m_FireTimer = 0.0f;

        protected bool m_Dead = false;

        private void Awake()
        {
            m_movementController = GetComponent<MovementController>();
            m_Collider = GetComponent<Collider2D>();
            m_Animator = GetComponent<Animator>();
            m_SpriteRenderer = GetComponent<SpriteRenderer>();

            m_OriginalColor = m_SpriteRenderer.color;

            //if (projectilePrefab != null)
            //    m_BulletPool = BulletPool.GetObjectPool(projectilePrefab.gameObject, 8);

            m_SpriteForward = spriteFaceLeft ? Vector2.left : Vector2.right;
            if (m_SpriteRenderer.flipX) m_SpriteForward = -m_SpriteForward;

            EndAttack();
        }

        private void OnEnable()
        {
            EndAttack();

            m_Dead = false;
            m_Collider.enabled = true;
        }

        private void OnDestroy()
        {
            OnAttackingBegins = null;
            OnShootngBegins = null;
            OnForgetTarget = null;
            OnTakeDamaged = null;
            OnFootStep = null;
        }

        private void Start()
        {
            SceneLinkedSMB<ThingBehaviour>.Initialise(m_Animator, this);

            m_LocalBounds = new Bounds();
            int count = m_movementController.Rigidbody2D.GetAttachedColliders(s_ColliderCache);
            
            for (int i = 0; i < count; ++i)
                m_LocalBounds.Encapsulate(transform.InverseTransformBounds(s_ColliderCache[i].bounds));

            m_Filter = new ContactFilter2D();
            m_Filter.layerMask = m_movementController.groundedLayerMask;
            m_Filter.useLayerMask = true;
            m_Filter.useTriggers = false;

            if (meleeDamager)
                m_LocalDamagerPosition = meleeDamager.transform.localPosition;
        }

        void FixedUpdate()
        {
            if (m_Dead) return;

            float deltaTime = Time.deltaTime;
            
            m_MoveVector.y = Mathf.Max(m_MoveVector.y - gravity * deltaTime, -gravity);
            m_movementController.Move(m_MoveVector * deltaTime);
            m_movementController.CheckCapsuleEndCollisions(deltaTime);
            UpdateTimers(deltaTime);

            OnFIxedUpdate?.Invoke();
        }

        //Call every frame when the enemy is in pursuit to check for range & Trigger the attack if in range
        public bool CheckMeleeAttack()
        {
            //we lost the target, shouldn't shoot
            if (!target)
                return false;

            if ((target.position - transform.position).sqrMagnitude < (meleeRange * meleeRange))
            {
                //meleeAttackAudio.PlayRandomSound();
                OnAttackingBegins?.Invoke();
                return true;
            }

            return false;
        }

        // 정면 장애물 확인
        // we circle cast with a size sligly small than the collider height. That avoid to collide with very small bump on the ground
        /// <summary>
        /// 
        /// </summary>
        /// <param name="forwardDistance"></param>
        /// <returns>true : 장애물 존재 fasle : 장애물 없음</returns>
        public bool CheckForObstacleForward(float forwardDistance)
        {
#if UNITY_EDITOR
            Debug.DrawRay(m_Collider.bounds.center, m_SpriteForward * forwardDistance);
#endif
            return Physics2D.CircleCast(m_Collider.bounds.center, m_Collider.bounds.extents.y - 0.2f, m_SpriteForward, forwardDistance, m_Filter.layerMask.value);
        }

        // 아래 방향 절벽 확인
        public bool CheckForObstacleDown(float forwardDistance)
        {
            // 가는 방향 살짝 앞 위치
            Vector3 castingPosition = (Vector2)(transform.position + m_LocalBounds.center) + m_SpriteForward * m_LocalBounds.extents.x;
            return !Physics2D.CircleCast(castingPosition, 0.1f, Vector2.down, m_LocalBounds.extents.y + 0.2f, m_movementController.groundedLayerMask.value);
        }

        public bool CheckForObstacle(float forwardDistance)
        {
            if (CheckForObstacleForward(forwardDistance))
                return true;
            
            // 가는 방향 살짝 앞 위치
            Vector3 castingPosition = (Vector2)(transform.position + m_LocalBounds.center) + m_SpriteForward * m_LocalBounds.extents.x;

#if UNITY_EDITOR
            Debug.DrawLine(castingPosition, castingPosition + Vector3.down * (m_LocalBounds.extents.y + 0.2f));
#endif
            if (CheckForObstacleDown(forwardDistance))
            {
                return true;
            }

            return false;
        }

        //This is call each update if the enemy is in a attack/shooting state, but the timer will early exit if too early to shoot.
        public void CheckShootingTimer()
        {
            if (m_FireTimer > 0.0f)
            {
                return;
            }

            //we lost the target, shouldn't shoot
            if (target == null)
                return;
            //shootingAudio.PlayRandomSound();
            OnShootngBegins?.Invoke();

            m_FireTimer = 1.0f;
        }

        public void CheckTargetStillVisible()
        {
            if (target == null)
                return;

            Vector3 toTarget = target.position - transform.position;

            if (toTarget.sqrMagnitude < viewDistance * viewDistance)
            {
                Vector3 testForward = Quaternion.Euler(0, 0, spriteFaceLeft ? -viewDirection : viewDirection) * m_SpriteForward;
                if (m_SpriteRenderer.flipX) testForward.x = -testForward.x;

                float angle = Vector3.Angle(testForward, toTarget);

                if (angle <= viewFov)
                {
                    RefreshSerchingTargetTime();
                }
            }

            if (m_TimeSinceLastTargetView <= 0.0f)
            {
                ForgetTarget();
            }
        }

        /// <summary>
        /// we reset the timer if the target is at viewing distance.
        /// </summary>
        public void RefreshSerchingTargetTime()
        {
            m_TimeSinceLastTargetView = timeBeforeTargetLost;
        }

        public void Die(Damager damager, Damageable damageable)
        {
            Vector2 throwVector = new Vector2(0, 2.0f);
            Vector2 damagerToThis = damager.transform.position - transform.position;

            throwVector.x = Mathf.Sign(damagerToThis.x) * -4.0f;
            SetMoveVector(throwVector);
            
            m_Dead = true;
            m_Collider.enabled = false;

            //dieAudio.PlayRandomSound();
            //CameraShaker.Shake(0.15f, 0.3f);
            OnDie.Invoke();
        }

        public void DisableDamage()
        {
            if (meleeDamager != null)
                meleeDamager.DisableDamage();
            if (contactDamager != null)
                contactDamager.DisableDamage();
        }

        public void EndAttack()
        {
            if (meleeDamager != null)
            {
                // 원본은 별도의 GameObject에 Damager 컴포넌트 할당
                meleeDamager.enabled = false;
                meleeDamager.DisableDamage();
            }
        }

        public void ForgetTarget()
        {
            OnForgetTarget?.Invoke();
            target = null;
        }

        /// <summary>
        /// OnTakeDamage Event에 할당
        /// </summary>
        /// <param name="damager"></param>
        /// <param name="damageable"></param>
        public void Hit(Damager damager, Damageable damageable)
        {
            if (damageable.CurrentHealth <= 0)
                return;
            
            //CameraShaker.Shake(0.15f, 0.3f);
            OnTakeDamaged?.Invoke();

            Vector2 damagerToThis = damager.transform.position - transform.position;
            m_MoveVector = new Vector2(Mathf.Sign(damagerToThis.x) * -2.0f, 3.0f); // 던져지는 방향

            if (m_FlickeringCoroutine != null)
            {
                StopCoroutine(m_FlickeringCoroutine);
                m_SpriteRenderer.color = m_OriginalColor;
            }

            m_FlickeringCoroutine = StartCoroutine(FlickerProcess(damageable));
        }

        public void OrientToTarget()
        {
            if (target == null)
                return;

            Vector3 toTarget = target.position - transform.position;

            if (Vector2.Dot(toTarget, m_SpriteForward) < 0)
            {
                SetFacingData(Mathf.RoundToInt(-m_SpriteForward.x));
            }
        }

        public void PlayFootStep()
        {
            OnFootStep?.Invoke();
            //footStepAudio.PlayRandomSound();
        }

        //This is used in case where there is a delay between deciding to shoot and shoot (e.g. Spitter that have an animation before spitting)
        //so we shoot where the target was when the animation started, not where it is when the actual shooting happen
        public void RememberTargetPos(Transform target)
        {
            if (target == null)
                return;

            m_TargetShootPosition = target.position;
        }


        public void SetFacingData(int facing)
        {
            if (facing == -1)
            {
                m_SpriteRenderer.flipX = !spriteFaceLeft;
                m_SpriteForward = spriteFaceLeft ? Vector2.right : Vector2.left;
            }
            else if (facing == 1)
            {
                m_SpriteRenderer.flipX = spriteFaceLeft;
                m_SpriteForward = spriteFaceLeft ? Vector2.left : Vector2.right;
            }
        }

        public void SetHorizontalSpeed(float horizontalSpeed)
        {
            m_MoveVector.x = horizontalSpeed * m_SpriteForward.x;
        }

        public void SetMoveVector(Vector2 newMoveVector)
        {
            m_MoveVector = newMoveVector;
        }

        public void ScanForPlayer(Transform target)
        {
            //If the player don't have control, they can't react, so do not pursue them

            //if (!PlayerInput.Instance.HaveControl)
            //    return;

            if (!target)
                return;

            Vector3 dir = target.transform.position - transform.position;

            if (dir.sqrMagnitude > viewDistance * viewDistance)
            {
                return;
            }

            Vector3 testForward = Quaternion.Euler(0, 0, spriteFaceLeft ? Mathf.Sign(m_SpriteForward.x) * -viewDirection : Mathf.Sign(m_SpriteForward.x) * viewDirection) * m_SpriteForward;

            float angle = Vector3.Angle(testForward, dir.normalized);

            if (angle > viewFov * 0.5f)
            {
                return;
            }
            this.target = target;
            m_TimeSinceLastTargetView = timeBeforeTargetLost;
            OnSpottedTarget?.Invoke();
        }

        /// <summary>
        /// Animation Clip 내 이벤트로 설정 됨
        /// </summary>
        public void Shooting()
        {
            Vector2 shootPosition = shootingOrigin.transform.localPosition;

            //if we are flipped compared to normal, we need to localy flip the shootposition too
            if ((spriteFaceLeft && m_SpriteForward.x > 0) || (!spriteFaceLeft && m_SpriteForward.x > 0))
                shootPosition.x *= -1;

            //shootingAudio.PlayRandomSound();
            //Projectile projectile = m_BulletPool.Pop(shootingOrigin.TransformPoint(shootPosition));
            //projectile.SetProjectileVeloicty(m_TargetShootPosition, shootingOrigin.transform.position);
            OnShoot?.Invoke();
        }

        //This is called when the damager get enabled (so the enemy can damage the player). 
        // Likely be called by the animation throught animation event (see the attack animation of the Chomper)
        public void StartAttack()
        {
            if(meleeDamager)
            {
                if (meleeDamager.transform != transform)
                {
                    if (m_SpriteRenderer.flipX)
                        meleeDamager.transform.localPosition = Vector3.Scale(m_LocalDamagerPosition, new Vector3(-1, 1, 1));
                    else
                        meleeDamager.transform.localPosition = m_LocalDamagerPosition;
                }

                meleeDamager.EnableDamage();
                meleeDamager.enabled = true;
            }
           
            if (attackDash)
                m_MoveVector = new Vector2(m_SpriteForward.x * attackForce.x, attackForce.y);
        }

        public void UpdateFacing()
        {
            bool faceLeft = m_MoveVector.x < 0f;
            bool faceRight = m_MoveVector.x > 0f;

            if (faceLeft)
            {
                SetFacingData(-1);
            }
            else if (faceRight)
            {
                SetFacingData(1);
            }
        }

        public void EnableContactDamager()
        {
            if (contactDamager)
                contactDamager.EnableDamage();
        }

        public void DisableContactDamager()
        {
            if (contactDamager)
                contactDamager.DisableDamage();
        }
   
        protected System.Collections.IEnumerator FlickerProcess(Damageable damageable)
        {
            float timer = 0f;
            float sinceLastChange = 0.0f;

            Color transparent = m_OriginalColor;
            transparent.a = 0.2f;
            int state = 1;

            m_SpriteRenderer.color = transparent;

            while (timer < damageable.invulnerabilityDuration)
            {
                yield return null;
                timer += Time.deltaTime;
                sinceLastChange += Time.deltaTime;
                if (sinceLastChange > flickeringDuration)
                {
                    sinceLastChange -= flickeringDuration;
                    state = 1 - state;
                    m_SpriteRenderer.color = state == 1 ? transparent : m_OriginalColor;
                }
            }

            m_SpriteRenderer.color = m_OriginalColor;
        }

        void UpdateTimers(float deltaTime)
        {
            if (m_TimeSinceLastTargetView > 0.0f)
                m_TimeSinceLastTargetView -= deltaTime;

            if (m_FireTimer > 0.0f)
                m_FireTimer -= deltaTime;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            //draw the cone of view
            Vector3 forward = spriteFaceLeft ? Vector2.left : Vector2.right;
            forward = Quaternion.Euler(0, 0, spriteFaceLeft ? -viewDirection : viewDirection) * forward;

            if (GetComponent<SpriteRenderer>().flipX) forward.x = -forward.x;

            Vector3 endpoint = transform.position + (Quaternion.Euler(0, 0, viewFov * 0.5f) * forward);

            Handles.color = new Color(0, 1.0f, 0, 0.2f);
            Handles.DrawSolidArc(transform.position, -Vector3.forward, (endpoint - transform.position).normalized, viewFov, viewDistance);

            //Draw attack range
            Handles.color = new Color(1.0f, 0, 0, 0.1f);
            Handles.DrawSolidDisc(transform.position, Vector3.back, meleeRange);
        }
#endif
    }

    // bit hackish, to avoid to have to redefine the whole inspector, we use an attirbute and associated property drawer to 
    // display a warning above the melee range when it get over the view distance.
    public class EnemyMeleeRangeCheckAttribute : PropertyAttribute
    {

    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(EnemyMeleeRangeCheckAttribute))]
    public class EnemyMeleeRangePropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty viewRangeProp = property.serializedObject.FindProperty("viewDistance");
            if (viewRangeProp.floatValue < property.floatValue)
            {
                Rect pos = position;
                pos.height = 30;
                EditorGUI.HelpBox(pos, "Melee range is bigger than View distance. Note enemies only attack if target is in their view range first", MessageType.Warning);
                position.y += 30;
            }

            EditorGUI.PropertyField(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty viewRangeProp = property.serializedObject.FindProperty("viewDistance");
            if (viewRangeProp.floatValue < property.floatValue)
            {
                return base.GetPropertyHeight(property, label) + 30;
            }
            else
                return base.GetPropertyHeight(property, label);
        }
    }
#endif
}


