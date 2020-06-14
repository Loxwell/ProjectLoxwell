
using UnityEngine;

namespace Platformer.Mechanics 
{
    /// <summary>
    /// Implements game physics for some in game entity.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D)), DisallowMultipleComponent]
    public class KinematicObject : MonoBehaviour
    {
        protected const float MIN_MOVE_DISTANCE = 0.001f;
        protected const float SHELL_RADIUS = 0.01f;

        /// <summary>
        /// The current velocity of the entity.
        /// </summary>
        public Vector2 velocity;

        /// <summary>
        /// Implements game physics for some in game entity.
        /// </summary>
        [SerializeField, Header("Implements game physics"), Range(0.01f, 1f)]
        float m_minGroundNormalY = .65f;

        /// <summary>
        /// A custom gravity coefficient applied to this entity.
        /// </summary>
        [SerializeField, Header("Custom Gravity coefficient"), Range(1e-7f, 2f)]
        float m_gravityModifier = 1f;

        public bool IsGrounded { get; private set; }

        protected Vector2 targetVelocity;
        protected Vector2 groundNormal;
        protected Rigidbody2D body;
        protected ContactFilter2D contactFilter;
        protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

        protected virtual void Awake()
        {
            if(!body)
                body = GetComponentInChildren<Rigidbody2D>();
        }

        protected virtual void OnEnable()
        {
            body.isKinematic = true;
        }

        protected virtual void OnDisable()
        {
            body.isKinematic = false;
        }

        protected virtual void Start()
        {
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            contactFilter.useLayerMask = true;
        }

        protected virtual void Update()
        {
            targetVelocity = Vector2.zero;
            ComputeVelocity();
        }

        protected virtual void FixedUpdate()
        {
            UpdateMovement(Time.fixedDeltaTime);
        }

        /// <summary>
        /// Bounce the object's vertical velocity.
        /// </summary>
        /// <param name="value"></param>
        public void Bounce(float value)
        {
            velocity.y = value;
        }

        /// <summary>
        /// Bounce the objects velocity in a direction.
        /// </summary>
        /// <param name="dir"></param>
        public void Bounce(Vector2 dir)
        {
            velocity.y = dir.y;
            velocity.x = dir.x;
        }

        /// <summary>
        /// Teleport to some position.
        /// </summary>
        /// <param name="position"></param>
        public void Teleport(Vector3 newPos)
        {
            velocity *= 0;
            body.position = newPos;
            body.velocity *= 0;
        }

        protected virtual void ComputeVelocity()
        { /*Empty*/ }

        void PerformMovement(Vector2 move, bool yMovement)
        {
            float distance = move.magnitude;
            
            if(distance > MIN_MOVE_DISTANCE)
            {
                int count = body.Cast(move, contactFilter, hitBuffer, distance + SHELL_RADIUS);
                for(int i = 0; i < count; ++i)
                {
                    Vector2 currentNormal = hitBuffer[i].normal;

                    // is this surface flat enough to land on?
                    if(currentNormal.y > m_minGroundNormalY)
                    {
                        IsGrounded = true;
                        // if moving up, change the groundNormal to new surface normal.
                        if(yMovement)
                        {
                            groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }

                    if(IsGrounded)
                    {
                        float projection = Vector2.Dot(velocity, currentNormal);
                        // 현재 이동 속도와 정반대 방향
                        if(projection < 0)
                            // slower velocity if moving against the normal (up a hill)
                            velocity = velocity - projection * currentNormal;
                    }else
                    {
                        velocity.x *= 0;
                        velocity.y = Mathf.Min(velocity.y, 0);
                    }

                    float modifiedDistance = hitBuffer[i].distance - SHELL_RADIUS;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }

            body.position = body.position + move.normalized * distance;
        }

        void UpdateMovement(float deltaTime)
        {
            //if already falling, fall faster than the jump speed, otherwise use normal gravity.
            if (velocity.y < 0)
                velocity += m_gravityModifier * Physics2D.gravity /*(0.0, -9.8)*/* deltaTime;
            else
                velocity += Physics2D.gravity /*(0.0, -9.8)*/* deltaTime;

            velocity.x = targetVelocity.x;
            IsGrounded = false;

            Vector2 deltaPosition = velocity * deltaTime;
            Vector2 moveAlongGround;
            moveAlongGround.x = groundNormal.y;
            moveAlongGround.y = -groundNormal.x;

            Vector2 move = moveAlongGround * deltaPosition;

            //horizontal movement
            PerformMovement(move, false);
            move = Vector2.up * deltaPosition;

            // vertical movement
            PerformMovement(move, true);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector2 deltaPosition = velocity * Time.deltaTime;
            Vector2 moveAlongGround;
            moveAlongGround.x = groundNormal.y;
            moveAlongGround.y = -groundNormal.x;

            Vector2 move = moveAlongGround * deltaPosition;

            if (!body)
                body = GetComponentInChildren<Rigidbody2D>();

            RaycastHit2D[] rayHits = new RaycastHit2D[16];
            if(body)
            {
                Gizmos.color = Color.red;
                int cnt = body.Cast(move, contactFilter, rayHits, move.magnitude + SHELL_RADIUS);
                for(int i = 0; i < cnt; ++i)
                {
                    Gizmos.DrawLine(transform.position, rayHits[i].normal * 2);
                }
            }
        }
#endif
    }
}


