using UnityEngine;
using Platformer.Mechanics.AI.StateMachine;
using Platformer.Mechanics.AI;

namespace InGame.StateMachine.Thing
{
    public class ThingRuntToTargetSMB : SceneLinkedSMB<ThingBehaviour>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.OrientToTarget();
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.CheckTargetStillVisible();
            float amount = m_MonoBehaviour.speed * 2.0f;
            if (m_MonoBehaviour.CheckForObstacle(amount))
            {
                m_MonoBehaviour.ForgetTarget();
                return;
            }
            else
                m_MonoBehaviour.SetHorizontalSpeed(amount);
            m_MonoBehaviour.CheckMeleeAttack();
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.SetHorizontalSpeed(0);
        }
    }
}