using System;
using UnityEngine;
using UnityEngine.Events;
using Platformer.GenericData.Utils;

namespace Platformer.Module
{
    public class Damageable : MonoBehaviour, IDataPersister
    {
        [Serializable]
        public class HealthEvent : UnityEvent<Damageable>
        { }

        [Serializable]
        public class DamageEvent : UnityEvent<Damager, Damageable>
        { }

        [Serializable]
        public class HealEvent : UnityEvent<int, Damageable>
        { }

        public int startingHealth = 5;

        public bool invulnerableAfterDamage = true;
        public float invulnerabilityDuration = 3f;

        public bool disableOnDeath { get; 
            private set; 
        } = false;

        [Tooltip("An offset from the obejct position used to set from where the distance to the damager is computed")]
        public Vector2 centreOffset = new Vector2(0f, 1f);

        public HealthEvent OnHealthSet;
        public DamageEvent OnTakeDamage;
        public DamageEvent OnDie;
        public HealEvent OnGainHealth;
        [HideInInspector]
        public DataSettings dataSettings;

        protected Vector2 m_DamageDirection;
        /// <summary>
        /// 안전한, 해칠 수 없는
        /// </summary>치
        protected bool m_Invulnerable;
        protected bool m_ResetHealthOnSceneReload;
        /// <summary>
        /// 안전 보장 시간
        /// </summary>
        protected float m_InvulnerabilityTimer;
        protected int m_CurrentHealth;

        void OnEnable()
        {
            //PersistentDataManager.RegisterPersister(this);
            m_CurrentHealth = startingHealth;

            OnHealthSet.Invoke(this);

            DisableInvulnerability();
        }

        void OnDisable()
        {
            //PersistentDataManager.UnregisterPersister(this);
        }

        void Update()
        {
            if (m_Invulnerable)
            {
                if ((m_InvulnerabilityTimer -= Time.deltaTime)<= 0f)
                {
                    m_Invulnerable = false;
                }
            }
        }

        public int CurrentHealth
        {
            get { return m_CurrentHealth; }
        }


        /// <summary>
        /// 무적 상태 활성
        /// </summary>
        /// <param name="ignoreTimer"></param>
        public void EnableInvulnerability(bool ignoreTimer = false)
        {
            m_Invulnerable = true;
            //technically don't ignore timer, just set it to an insanly big number. Allow to avoid to add more test & special case.
            m_InvulnerabilityTimer = ignoreTimer ? float.MaxValue : invulnerabilityDuration;
        }

        public void DisableInvulnerability()
        {
            m_Invulnerable = false;
        }

        public Vector2 GetDamageDirection()
        {
            return m_DamageDirection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damager"></param>
        /// <param name="ignoreInvincible">무적 상태 판정 무시</param>
        public void TakeDamage(Damager damager, bool ignoreInvincible /*Invincible == 천하무적의*/= false)
        {
            if ((m_Invulnerable && !ignoreInvincible) || m_CurrentHealth <= 0)
                return;

            //we can reach that point if the damager was one that was ignoring invincible state.
            //We still want the callback that we were hit, but not the damage to be removed from health.
            if (!m_Invulnerable)
            {
                m_CurrentHealth -= damager.damage;
                // HP 이벤트
                OnHealthSet.Invoke(this);
            }

            m_DamageDirection = transform.position + (Vector3)centreOffset - damager.transform.position;

            OnTakeDamage.Invoke(damager, this);

            if (m_CurrentHealth <= 0)
            {
                OnDie.Invoke(damager, this);
                m_ResetHealthOnSceneReload = true;
                EnableInvulnerability();
                if (disableOnDeath) 
                    gameObject.SetActive(false);
            } 
        }

        public void GainHealth(int amount)
        {
            m_CurrentHealth += amount;

            if (m_CurrentHealth > startingHealth)
                m_CurrentHealth = startingHealth;

            OnHealthSet.Invoke(this);

            OnGainHealth.Invoke(amount, this);
        }

        public void SetHealth(int amount)
        {
            m_CurrentHealth = amount;

            if (m_CurrentHealth <= 0)
            {
                OnDie.Invoke(null, this);
                m_ResetHealthOnSceneReload = true;
                EnableInvulnerability();
                if (disableOnDeath) 
                    gameObject.SetActive(false);
            }

            OnHealthSet.Invoke(this);
        }

        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Platformer.GenericData.Utils.Data SaveData()
        {
            return new Platformer.GenericData.Utils.Data<float, bool>(CurrentHealth, m_ResetHealthOnSceneReload);
        }

        public void LoadData(Platformer.GenericData.Utils.Data data)
        {
            Platformer.GenericData.Utils.Data<int, bool> healthData = (Data<int, bool>)data;
            m_CurrentHealth = healthData.value1 ? startingHealth : healthData.value0;
            OnHealthSet.Invoke(this);
        }
    }
}
