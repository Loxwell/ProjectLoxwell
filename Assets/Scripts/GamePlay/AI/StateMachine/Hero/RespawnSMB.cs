using UnityEngine;

using Mechanics.AI.StateMachine;
using Platformer.Player;

namespace GamePlay.StateMachine
{
    public class RespawnSMB : SceneLinkedSMB<PlayerCharacter>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnSLStateEnter(animator, stateInfo, layerIndex);
            if(m_MonoBehaviour)
                m_MonoBehaviour.SetMoveVector(Vector2.zero);
        }
    }
}

