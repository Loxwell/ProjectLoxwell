using UnityEngine;
using Platformer.Mechanics;
using Platformer.Mechanics.AI.StateMachine;

namespace InGame.StateMachine.Player
{
    public class PlayerRespawnSMB : SceneLinkedSMB<PlayerCharacter>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(m_MonoBehaviour)
                m_MonoBehaviour.SetMoveVector(Vector2.zero);
        }
    }
}

