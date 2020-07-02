
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
        #region CROUCH STATE
        internal class ActionHeroCrouchAttack : ActionNode
        {
            public override void OnStart(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                heroBB.controller.Attack(heroBB.aniClipCrouchingAttack);
                UnityEngine.Debug.LogWarning("ActionJumpAttack()");
            }

            public override EBTState Update(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                if (heroBB.controller.Equals(heroBB.aniStateAttack))
                {
                    if (heroBB.controller.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                    {
                        heroBB.controller.CurrentState = EState.CROUCH;
                        return EBTState.SUCCESS;
                    }
                }

                return EBTState.RUNNING;
            }
        }

        internal class ActionHeroCrouch : ActionNode
        {
            public override void OnStart(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                heroBB.controller.CurrentState = EState.CROUCH;
                heroBB.controller.ControlEnabled = false;
                UnityEngine.Debug.LogWarning("앉음()");
            }

            public override EBTState Update(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;

                if (heroBB.controller.Equals(heroBB.aniStateCrouchingBegins))
                {
                    return EBTState.RUNNING;
                }

                if (heroBB.controller.Equals(heroBB.aniStateCrouching))
                {
                    return EBTState.SUCCESS;
                }
                return EBTState.FAILED;
            }
        }

        // 현재 상태 키입려이 아래인지 확인 만 하고 
        // 상태 이상은 다음 시퀀서에서 처리 할 것
        static EBTState ConditionAxisDown(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;
            if (IsCurFrameInputState(heroBB, EInputState.DOWN_KEY))
                return EBTState.SUCCESS;

            // 일어서는 중 에 이동 처리를 막기 위한 조건문
            if ((heroBB.controller.Equals(heroBB.aniStateStandUp) || heroBB.controller.Equals(heroBB.aniStateCrouching)))
            {
                heroBB.controller.CurrentState = EState.IDLE;
                return EBTState.RUNNING;
            }
            heroBB.controller.ControlEnabled = true;

            return EBTState.FAILED;
        }
        #endregion CROUCH

    }
}