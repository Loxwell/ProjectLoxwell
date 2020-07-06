
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Mechanics.AI;
using Platformer.Core;

namespace Platformer.LSG.Object
{
    [RequireComponent(typeof(ThingBehaviour), typeof(Animator), typeof(MovementController))]
    public class GolemAnimatorController : MonoBehaviour
    {
        readonly int HASH_PARAM_ISGROUNDED = Animator.StringToHash("Grounded");
        readonly int HASH_PARAM_DEATH = Animator.StringToHash("Death");
        readonly int HASH_PARAM_HIT = Animator.StringToHash("Hit");
        readonly int HASH_PARAM_MELEEATTACK = Animator.StringToHash("MeleeAttack");
        readonly int HASH_PARAM_SPOTTED = Animator.StringToHash("Spotted");

        Animator m_animator;
        MovementController m_mc;

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            m_mc = GetComponent<MovementController>();

            ThingBehaviour tb = GetComponent<ThingBehaviour>();

            tb.OnFIxedUpdate += () => { m_animator.SetBool(HASH_PARAM_ISGROUNDED, m_mc.IsGrounded); };
            tb.OnAttackingBegins += () => { m_animator.SetTrigger(HASH_PARAM_MELEEATTACK); };
            tb.OnDie += () => { m_animator.SetTrigger(HASH_PARAM_DEATH); };
            tb.OnForgetTarget += () => { m_animator.SetBool(HASH_PARAM_SPOTTED, false); };
            tb.OnTakeDamaged += () => { m_animator.SetTrigger(HASH_PARAM_HIT); };
            tb.OnSpottedTarget += () => { m_animator.SetBool(HASH_PARAM_SPOTTED, true); };
        }
    }
}
