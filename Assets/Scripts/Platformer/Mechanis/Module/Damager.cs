
using UnityEngine;
using UnityEngine.Events;

namespace Platformer.Module
{
    public class Damager : MonoBehaviour
    {
        [System.Serializable]
        public class DamagableEvent : UnityEvent<Damager, Damageable>
        { }

        [System.Serializable]
        public class NonDamagableEvent : UnityEvent<Damager>
        { }

        //call that from inside the onDamageableHIt or OnNonDamageableHit to get what was hit.
        public Collider2D LastHit { get { return m_LastHit; } }

        public float damage = 1;
        /// <summary>
        /// Position으로 부터 Raycast 시작점
        /// </summary>
        public Vector2 offset = new Vector2(1.5f, 1f);
        /// <summary>
        /// 공격 범위
        /// </summary>
        [Tooltip("공격 범위")]
        public Vector2 size = new Vector2(2.5f, 1f);

        [Tooltip("If this is set, the offset x will be changed base on the sprite flipX setting. e.g. Allow to make the damager alway forward in the direction of sprite")]
        public bool offsetBasedOnSpriteFacing = true;

        [Tooltip("SpriteRenderer used to read the flipX value used by offset Based OnSprite Facing")]
        public SpriteRenderer spriteRenderer;

        [Tooltip("If disabled, damager ignore trigger when casting for damage")]
        public bool canHitTriggers;

        [Tooltip("If set, the player will be forced to respawn to latest checkpoint in addition to loosing life")]
        public bool forceRespawn = false;

        [Tooltip("If set, an invincible damageable hit will still get the onHit message (but won't loose any life)")]
        public bool ignoreInvincibility = false;

        public bool disableDamageAfterHit = false;

        [Tooltip("공격 대상 Layer")]
        public LayerMask hittableLayers;

        #region 대미지 이벤트
        public DamagableEvent OnDamageableHit;
        public NonDamagableEvent OnNonDamageableHit;
        #endregion


#pragma warning disable
        protected bool m_SpriteOriginallyFlipped;
        protected bool m_CanDamage = true;
        protected ContactFilter2D m_AttackContactFilter;
        protected Collider2D[] m_AttackOverlapResults = new Collider2D[10];
        protected Transform m_DamagerTransform;
        protected Collider2D m_LastHit;

        void Awake()
        {
            m_AttackContactFilter.layerMask = hittableLayers;
            m_AttackContactFilter.useLayerMask = true;
            m_AttackContactFilter.useTriggers = canHitTriggers;

            if (offsetBasedOnSpriteFacing && spriteRenderer != null)
                m_SpriteOriginallyFlipped = spriteRenderer.flipX;

            m_DamagerTransform = transform;
        }

        void FixedUpdate()
        {
            if (!m_CanDamage)
                return;
            Attack();
        }

        public void Attack()
        {
            // 공격을 가할 물체의 크기
            Vector2 bodyScale = m_DamagerTransform.lossyScale;

            // 캐릭터 크기의 x,y의 비율을 offset에 적용함 
            Vector2 facingOffset = Vector2.Scale(offset, bodyScale);

            // sprite flip과 현재 방향이 안맞을 경우 반대로 적용
            if (offsetBasedOnSpriteFacing && spriteRenderer != null && spriteRenderer.flipX != m_SpriteOriginallyFlipped)
                facingOffset = new Vector2(-offset.x * bodyScale.x, offset.y * bodyScale.y);

            // 현재 객체의 크기를 공격 범위에 적용
            Vector2 scaledSize = Vector2.Scale(size, bodyScale);

            // 공격 범위 시작점
            // 사각형의 시작 꼭지점
            Vector2 pointA = (Vector2)m_DamagerTransform.position + facingOffset - (scaledSize * 0.5f)/*공격 범위 절반*/;
            // 시작 꼭지점의 대각선 방향의 끝점
            Vector2 pointB = pointA + scaledSize;

            int hitCount = Physics2D.OverlapArea(pointA, pointB, m_AttackContactFilter, m_AttackOverlapResults);

            for (int i = 0; i < hitCount; i++)
            {
                m_LastHit = m_AttackOverlapResults[i];
                Damageable damageable = m_LastHit.GetComponent<Damageable>();

                if (damageable)
                {
                    OnDamageableHit.Invoke(this, damageable);
                    damageable.TakeDamage(this, ignoreInvincibility);
                    if (disableDamageAfterHit)
                        DisableDamage();
                }
                else
                {
                    OnNonDamageableHit.Invoke(this);
                }
            }
        }


        public void EnableDamage()
        {
            m_CanDamage = true;
        }

        public void DisableDamage()
        {
            m_CanDamage = false;
        }
    }
}
