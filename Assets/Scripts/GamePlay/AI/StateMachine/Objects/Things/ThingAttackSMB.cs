using UnityEngine;
using Platformer.Mechanics.AI.StateMachine;
using Platformer.Mechanics.AI;

namespace InGame.StateMachine.Player
{
    public class ThingAttackSMB : SceneLinkedSMB<ThingBehaviour>
    {
        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateExit(animator, stateInfo, layerIndex);

            m_MonoBehaviour.SetHorizontalSpeed(0);
        }
    }
}