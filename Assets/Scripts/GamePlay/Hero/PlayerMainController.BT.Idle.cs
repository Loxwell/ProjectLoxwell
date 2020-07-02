
using LSG.LWBehaviorTree;

using EState = LSG.PlayerMainController.EState;
using EInputState = LSG.EInputState;

using static HeroBlackboard;
using static LSG.LWBehaviorTree.ActionNode;
using System.Diagnostics;

namespace BT.LSG
{
    public partial class PlayerMainController
    {
        #region IDLE STATE

        static EBTState ConditionPlayingIdleState(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;

            if(heroBB.controller.Equals(heroBB.aniStateIdle))
                return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

        internal class ActionStopMoving : ActionNode
        {
            public override void OnStart(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                heroBB.controller.ControlEnabled = false;
            }

            public override EBTState Update(IBlackboard bb)
            {
                return EBTState.SUCCESS;
            }
        }

        internal class ActionStandAttack : ActionNode
        {
            public override void OnStart(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                heroBB.controller.Attack(heroBB.aniClipStandAttack);
                UnityEngine.Debug.LogWarning("ActionStandAttack()");
            }

            public override EBTState Update(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                if (heroBB.controller.Equals(heroBB.aniStateAttack) || heroBB.controller.Equals(heroBB.aniStateIdle))
                {
                    if (heroBB.controller.Equals(heroBB.aniStateAttack) &&
                        heroBB.controller.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9)
                        return EBTState.SUCCESS;

                    return EBTState.RUNNING;
                }

                return EBTState.FAILED;
            }
        }

        internal class ActionHeroIdle : ActionNode
        {
            public override void OnStart(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                heroBB.controller.CurrentState = EState.IDLE;
                heroBB.controller.ControlEnabled = true;
                UnityEngine.Debug.Log("ActionHeroIdle()");
            }
            public override EBTState Update(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                if (heroBB.controller.Equals(heroBB.aniStateIdle))
                    return EBTState.SUCCESS;
                return EBTState.RUNNING;
            }
        }
        #endregion
    }
}