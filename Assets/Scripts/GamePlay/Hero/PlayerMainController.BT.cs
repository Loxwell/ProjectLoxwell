using UnityEngine;
using LSG.LWBehaviorTree;

using EState = LSG.PlayerMainController.EState;
using EInputState = LSG.EInputState;

using static HeroBlackboard;
using static LSG.LWBehaviorTree.ActionNode;

namespace BT.LSG
{
    public partial class PlayerMainController
    {
        internal class HeroBT
        {
            RepeatNode root;
            HeroBlackboard m_bb;
            internal HeroBT(HeroBlackboard bb)
            {
                m_bb = bb;
                root = new RepeatNode();
                SequenceNode crouchngState = new SequenceNode();
                SequenceNode jumpingState = new SequenceNode();
                SelectNode idleState = new SelectNode();

                SelectNode subCrouchngState = new SelectNode();

                root.Add(crouchngState);
                root.Add(jumpingState);
                /*
                 이 사이에 추가 상태 넣을 것
                 */
                
                // Crouch State
                crouchngState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionAxisDown, onStart = null }));
                crouchngState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionIsGrounded, onStart = null }));
                
                crouchngState.Add(subCrouchngState);
                crouchngState.Add(new DecoratorNode(CreateNodeTask<ActionHeroCrouch>()));
                crouchngState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionActionButtonDown, onStart = null }));
                crouchngState.Add(new NegationDecoratorNode(CreateNodeTask<ActionHeroCrouchAttack>()));

                subCrouchngState.Add(new NegationDecoratorNode(new Condition() { onUpdate = ConditionHeroJumpAniState, onStart = null }));
                subCrouchngState.Add(new NegationDecoratorNode(CreateNodeTask<ActionHeroIdle>()));

                // Jump State
                jumpingState.Add(new NegationDecoratorNode(new Condition() { onUpdate = ConditionIsGrounded, onStart = null }));
                jumpingState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionHeroJumping, onStart = null }));
                jumpingState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionActionButtonDown, onStart = null }));
                jumpingState.Add(new DecoratorNode(CreateNodeTask<ActionJumpAttack>()));

                // Idle State
                idleState.Add(new DecoratorNode(CreateNodeTask<ActionHeroIdle>()));
                
            }

            public void Update()
            {
                root.Update(m_bb);
            }
        }

        #region IDLE STATE
        internal class ActionHeroIdle : ActionNode
        {
            public override void OnStart(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                heroBB.controller.CurrentState = EState.IDLE;

                heroBB.controller.Debug("ActionHeroIdle()");
            }
            public override EBTState Update(IBlackboard bb)
            {
                return EBTState.SUCCESS;
            }
        }
        #endregion

        static EBTState ConditionIsGrounded(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;

            heroBB.controller.Debug("ConditionIdlingState");
            if (heroBB.isGrounded)
            {
                heroBB.controller.Debug("Crouch start ");
                return EBTState.SUCCESS;
            }

            return EBTState.FAILED;
        }

        static EBTState ConditionHeroControlEnabled(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;

            if (heroBB.isControled)
                return EBTState.SUCCESS;

            return EBTState.FAILED;
        }

        static EBTState ConditionActionButtonDown(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;
            if (IsCurFrameInputState(heroBB, EInputState.ACTION_1) && !IsPreFrameInputState(heroBB, EInputState.ACTION_1))
            {
#if UNITY_EDITOR

                heroBB.controller.Debug("Crouch Begin Attack");
#endif

                return EBTState.SUCCESS;
            }

            return EBTState.FAILED;
        }

        static EBTState ConditionJumpButtonDown(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;
            if (IsCurFrameInputState(heroBB, EInputState.JUMP) && !IsPreFrameInputState(heroBB, EInputState.JUMP))
                return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

        static EBTState ConditionJumpButton(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;
            if (IsCurFrameInputState(heroBB, EInputState.JUMP))
                return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

        static EBTState ConditionActionButton(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;

            if (IsPreFrameInputState(heroBB, EInputState.ACTION_1))
                return EBTState.SUCCESS;
            return EBTState.FAILED;
        }
    }
}