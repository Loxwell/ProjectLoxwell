using UnityEngine;
using Platformer.Mechanics.AI.StateMachine;
using Platformer.Mechanics.AI;

namespace InGame.StateMachine.Player
{
    public class ThingDeathSMB : SceneLinkedSMB<ThingBehaviour>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.DisableDamage();
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.gameObject.SetActive(false);
        }
    }
}