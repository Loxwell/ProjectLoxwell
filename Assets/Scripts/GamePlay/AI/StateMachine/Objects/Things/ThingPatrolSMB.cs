using UnityEngine;
using Platformer.Mechanics.AI.StateMachine;
using Platformer.Mechanics.AI;
using LSG.LWBehaviorTree;

namespace InGame.StateMachine.Thing
{
    public class ThingPatrolSMB : SceneLinkedSMB<ThingBehaviour>
    {
        [SerializeField]
        PlayerBlackboard m_blackboard;
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //We do this explicitly here instead of in the enemy class, that allow to handle obstacle differently according to state
            // (e.g. look at the ChomperRunToTargetSMB that stop the pursuit if there is an obstacle) 
            float dist = m_MonoBehaviour.speed;
            if (m_MonoBehaviour.CheckForObstacle(dist))
            {
                //this will inverse the move vector, and UpdateFacing will then flip the sprite & forward vector as moveVector will be in the other direction
                m_MonoBehaviour.SetHorizontalSpeed(-dist);
                m_MonoBehaviour.UpdateFacing();
            }
            else
            {
                m_MonoBehaviour.SetHorizontalSpeed(dist);
            }

            m_MonoBehaviour.ScanForPlayer(m_blackboard.player);
        }
    }
}
