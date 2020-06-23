using UnityEngine;
using LSG.LWBehaviorTree;
using static HeroBlackboard;
using static LSG.LWBehaviorTree.Condition;

namespace LSG
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
                SequenceNode crouchState = new SequenceNode();
                root.Add(crouchState);

                crouchState.Add(new NotDecoratorNode(new Condition() { onUpdate = ConditionIdlingState, onStart = null }));
                crouchState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionJumpButtonDown, onStart = null }));
                crouchState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionJumpButton, onStart = null }));
                crouchState.Add(new NotDecoratorNode( new Condition() { onUpdate = ConditionAxisDown, onStart = null }));
                crouchState.Add(new NotDecoratorNode(CreateNodeTask<ActionHeroCrouch>()));
                crouchState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionJumpButton, onStart = null }));
                crouchState.Add(new NotDecoratorNode(new Condition() { onUpdate = ConditionActionButtonDown, onStart = null }));
                crouchState.Add(new NotDecoratorNode(CreateNodeTask<ActionHeroCrouchAttack>()));
            }

            public void Update()
            {
                root.Update(m_bb);
            }
        }

        internal class ActionHeroCrouchAttack : ActionNode
        {
            int m_fullpath;

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

                if ( heroBB.controller.Equals(heroBB.aniStateCrouching))
                {
                    heroBB.controller.Debug("HeroCrouchAction true");

                    return EBTState.SUCCESS;

                }else if (
                    (heroBB.controller.Equals(heroBB.aniStateJumpingBegins) ||
                    heroBB.controller.Equals(heroBB.aniStateJumpFall) ||
                    heroBB.controller.Equals(heroBB.aniStateJumping)  ||
                    heroBB.controller.Equals(heroBB.aniStateJumpClimb)))
                {
                    heroBB.controller.ControlEnabled = true;

                    heroBB.controller.Debug("HeroCrouchAction Jumping");

                    return EBTState.FAILED;
                }

                heroBB.controller.Debug("HeroCrouchAction fail");

                return EBTState.FAILED;
            }
        }

        static EBTState ConditionIdlingState(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;

            heroBB.controller.Debug("ConditionIdlingState");

            switch (heroBB.controller.CurrentState)
            {
                case EState.IDLE:
                case EState.CROUCH:
                    heroBB.controller.Debug("Crouch start ");
                    return EBTState.SUCCESS;
            }
            
            return EBTState.FAILED;
        }


        
        // 현재 상태 키입려이 아래인지 확인 만 하고 
        // 상태 이상은 다음 시퀀서에서 처리 할 것
        static EBTState ConditionAxisDown(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;

            heroBB.controller.Debug("ConditionAxisDown");

            if (IsCurFrameInputState(heroBB, EInputState.DOWN_KEY))
            {
                heroBB.controller.ControlEnabled = false;

                heroBB.controller.Debug("current Push DownKey");

                return EBTState.SUCCESS;
            }

            // 일어서는 중 에 이동 처리를 막기 위한 조건문
            if ((IsCurFrameInputState(heroBB, EInputState.LEFT_KEY) || IsCurFrameInputState(heroBB, EInputState.RIGHT_KEY)) 
                && (heroBB.Equals(heroBB.aniStateStandUp)|| heroBB.Equals(heroBB.aniStateCrouching))
                )
            {
                heroBB.controller.Debug("current Left or Right DownKey");

                heroBB.controller.CurrentState = EState.IDLE;
                return EBTState.RUNNING;
            }
              
            if (heroBB.controller.CurrentState == EState.CROUCH)
                heroBB.controller.CurrentState = EState.IDLE;

            heroBB.controller.Debug("ConditionAxisDown false");
            heroBB.controller.ControlEnabled = true;
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

        static EBTState ConditionActionButton(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;

            if (IsPreFrameInputState(heroBB, EInputState.ACTION_1))
                return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

    }
}