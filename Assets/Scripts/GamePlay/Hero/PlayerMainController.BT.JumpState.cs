
using LSG.LWBehaviorTree;

using EState = LSG.PlayerMainController.EState;
using EInputState = LSG.EInputState;

using static HeroBlackboard;
using static LSG.LWBehaviorTree.ActionNode;

namespace BT.LSG
{
    public partial class PlayerMainController
    {
        #region JUMP STATE
        class ActionJumpAttack : ActionNode
        {
            public override void OnStart(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                heroBB.controller.CurrentState = EState.ATTACK;
            }

            public override EBTState Update(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;

                //heroBB.controller.Debug("");


                if (!heroBB.isGrounded)
                {
                    if (!heroBB.controller.Equals(heroBB.aniStateJumpAttack))
                        return EBTState.RUNNING;

                    if (heroBB.controller.Equals(heroBB.aniStateJumpAttack))
                    {
                        if (heroBB.controller.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9)
                            return EBTState.RUNNING;
                        heroBB.controller.CurrentState = EState.FALL;

                        return EBTState.SUCCESS;
                    }
                }
                heroBB.controller.CurrentState = EState.IDLE;
                return EBTState.FAILED;
            }
        }

        static EBTState ConditionHeroJumpAniState(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;
            if (heroBB.controller.Equals(heroBB.aniStateJumpFall) ||
                heroBB.controller.Equals(heroBB.aniStateJumping) ||
                heroBB.controller.Equals(heroBB.aniStateJumpingBegins))
                return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

        static EBTState ConditionHeroJumping(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;
            if (!heroBB.isGrounded && heroBB.controller.CurrentState != EState.FALL)
            {
                return EBTState.SUCCESS;
            }
            else if (heroBB.controller.CurrentState == EState.FALL)
            {
                return EBTState.SUCCESS;
            }

            return EBTState.FAILED;
        }

        #endregion
    }
}