
using LSG.LWBehaviorTree;

using EState = LSG.PlayerMainController.EState;
using EInputState = LSG.EInputState;

using static PlayerBlackboard;
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
                PlayerBlackboard heroBB = (PlayerBlackboard)bb;
                //heroBB.controller.Attack(heroBB.aniClipJumpingAttack);
                UnityEngine.Debug.LogWarning("ActionJumpAttack()");
            }

            public override EBTState Update(IBlackboard bb)
            {
                PlayerBlackboard heroBB = (PlayerBlackboard)bb;

                //if (!heroBB.isGrounded)
                //{
                //    if (!heroBB.controller.Equals(heroBB.aniStateAttack))
                //        return EBTState.RUNNING;

                //    if (heroBB.controller.Equals(heroBB.aniStateAttack))
                //    {
                //        if (heroBB.controller.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9)
                //            return EBTState.RUNNING;
                //        heroBB.controller.CurrentState = EState.FALL;

                //        return EBTState.SUCCESS;
                //    }
                //}
                //heroBB.controller.CurrentState = EState.IDLE;
                return EBTState.FAILED;
            }
        }

        static EBTState ConditionHeroJumpClimbAniState(IBlackboard bb)
        {
            PlayerBlackboard heroBB = (PlayerBlackboard)bb;
            //if (heroBB.controller.Equals(heroBB.aniStateJumpClimb))
            //    return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

        static EBTState ConditionHeroJumpAniState(IBlackboard bb)
        {
            PlayerBlackboard heroBB = (PlayerBlackboard)bb;

            //if (heroBB.controller.Equals(heroBB.aniStateJumpFall) ||
            //    heroBB.controller.Equals(heroBB.aniStateJumping) ||
            //    heroBB.controller.Equals(heroBB.aniStateJumpingBegins))
            //    return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

        static EBTState ConditionHeroJumping(IBlackboard bb)
        {
            PlayerBlackboard heroBB = (PlayerBlackboard)bb;
            //if (!heroBB.isGrounded && heroBB.controller.CurrentState != EState.FALL)
            //{
            //    return EBTState.SUCCESS;
            //}
            //else if (heroBB.controller.CurrentState == EState.FALL)
            //{
            //    return EBTState.SUCCESS;
            //}

            return EBTState.FAILED;
        }

        #endregion
    }
}