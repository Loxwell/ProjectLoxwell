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

                root.Add(crouchngState);
                root.Add(jumpingState);

                // Crouch State
                crouchngState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionAxisDown, onStart = null }));
                crouchngState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionIsGrounded, onStart = null }));
                crouchngState.Add(new DecoratorNode(CreateNodeTask<ActionHeroCrouch>()));
                crouchngState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionActionButtonDown, onStart = null }));
                crouchngState.Add(new DecoratorNode(CreateNodeTask<ActionHeroCrouchAttack>()));

                // Jump State
                jumpingState.Add(new NegationDecoratorNode(new Condition() { onUpdate = ConditionIsGrounded, onStart = null }));
                jumpingState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionHeroJumping, onStart = null }));
                jumpingState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionActionButtonDown, onStart = null }));
                jumpingState.Add(new DecoratorNode(CreateNodeTask<ActionJumpAttack>()));
            }

            public void Update()
            {
                root.Update(m_bb);
            }
        }

        #region CROUCH STATE
        internal class ActionHeroCrouchAttack : ActionNode
        {
            public override void OnStart(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                heroBB.controller.CurrentState = EState.ATTACK;
            }

            public override EBTState Update(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                if (heroBB.controller.Equals(heroBB.aniStateCrouchAttack))
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
            }

            public override EBTState Update(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;

                if (heroBB.controller.Equals(heroBB.aniStateCrouchingBegins))
                {
                    heroBB.controller.Debug("aniStateCrouchingBegins Running");

                    return EBTState.RUNNING;
                }

                if (heroBB.controller.Equals(heroBB.aniStateCrouching))
                {
                    heroBB.controller.Debug("HeroCrouchAction true");

                    return EBTState.SUCCESS;
                }

                heroBB.controller.Debug("HeroCrouchAction fail");

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

        #endregion
        static EBTState ConditionHeroJumping(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;
            if (!heroBB.isGrounded && heroBB.controller.CurrentState != EState.FALL)
            {
                return EBTState.SUCCESS;
            }else if (heroBB.controller.CurrentState == EState.FALL)
            {
                return EBTState.SUCCESS;
            }
            
            return EBTState.FAILED;
        }

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