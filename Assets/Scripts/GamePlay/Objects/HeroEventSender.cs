using LSG;
using LSG.Utilities;
using Platformer.Helper;
using Platformer.Mechanics;
using Platformer.Module;
using UnityEngine;

using Damageable = Platformer.Module.Damageable;

namespace InGame.Player
{
    enum ESndEffect
    {
        ATTACK = 0, LANDING = 1
    }

    [RequireComponent(typeof(Damageable), typeof(Damager), typeof(PlayerCharacter))]
    public class HeroEventSender : MonoBehaviour
    {
        PlayerCharacter m_player;
        Event.EventRespawn m_respawnEvent;
        private void Awake()
        {
            Damageable damageable = GetComponent<Damageable>();
            damageable.OnHealthSet.AddListener(OnChangeHP);

            Damager damager = GetComponent<Damager>();
            damager.OnDamageableHit.AddListener(OnAttackSuccess);

            m_player = GetComponent<PlayerCharacter>();
            m_player.onAttack += OnAttack;
            m_player.onRespawn += OnReapwn;
            m_player.onGrounded += OnGrounded;
        }

        void OnChangeHP(Damageable damageable)
        {
            Messenger<EInGameID>.SendMessage(EInGameID.HP, ECommand.SET, damageable.CurrentHealth);
        }

        void OnReapwn()
        {
            if (!m_respawnEvent || !Event.EventRespawn.DoingEvnet)
            {
                m_respawnEvent = ScheduleSystem.Core.Simulation.Schedule<Event.EventRespawn>(0, m_player, false, true);
            }
        }

        void OnAttack()
        {
            AudioSourceController.Instance.Play((int)ESndEffect.ATTACK, transform.position);
        }

        void OnAttackSuccess(Damager damager, Damageable damageable)
        {
            Messenger<EInGameID>.SendMessage(EInGameID.EFFECT_MANAGER, 0, damager.LastHit.bounds.center);
        }

        void OnGrounded()
        {
            AudioSourceController.Instance.Play((int)ESndEffect.LANDING, transform.position);
        }

    }
}


