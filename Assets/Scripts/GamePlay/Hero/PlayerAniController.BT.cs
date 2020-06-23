using UnityEngine;
using LSG.LWBehaviorTree;
using static HeroBlackboard;
using static LSG.LWBehaviorTree.Condition;

namespace LSG
{
    public partial class PlayerAniController
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
                //crouchState.Add(new DecoratorNode( new Condition() { onUpdate = ConditionJumpButton, onStart = null }));
                //crouchState.Add(new DecoratorNode( new Condition() { onUpdate = ConditionJumpButtonDown, onStart = null}));
                crouchState.Add(new NotDecoratorNode( new Condition() { onUpdate = ConditionAxisDown, onStart = null }));
                crouchState.Add(new NotDecoratorNode(CreateNodeTask<HeroCrouchAction>()));
                crouchState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionJumpButton, onStart = null }));
                crouchState.Add(new NotDecoratorNode(new Condition() { onUpdate = ConditionActionButtonDown, onStart = null }));
                crouchState.Add(new NotDecoratorNode(CreateNodeTask<HeroCrouchAttack>()));
            }

            public void Update()
            {
                root.Update(m_bb);
            }
        }

        internal class HeroCrouchAttack : ActionNode
        {
            int m_fullpath;

            public override void OnStart(IBlackboard bb)
            {
                
            }

            public override EBTState Update(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                AnimatorStateInfo info = heroBB.controller.StateInfo;
                if (info.fullPathHash
                    == Animator.StringToHash(heroBB.aniStateCrouchAttack))
                {
                    if(info.normalizedTime >= 0.9f)
                    {
                        return EBTState.SUCCESS;
                    }   
                }

                return EBTState.RUNNING;
            }
        }

        internal class HeroCrouchAction : ActionNode
        {
            public override void OnStart(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                heroBB.controller.CurrentState = EState.ATTACK;                
            }

            public override EBTState Update(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                AnimatorStateInfo info = heroBB.controller.StateInfo; ;
                int state = info.fullPathHash;

                if (state  == Animator.StringToHash(heroBB.aniStateCrouching))
                {
                    heroBB.controller.ControlEnabled = false;

                    heroBB.controller.Debug("HeroCrouchAction true");
 
                    return EBTState.SUCCESS;

                }else if ((state == Animator.StringToHash(heroBB.aniStateJumpFall)) ||
                    state == Animator.StringToHash(heroBB.aniStateJumping) ||
                    state == Animator.StringToHash(heroBB.aniStateJumpClimb))
                {
                    heroBB.controller.ControlEnabled = true;

                    heroBB.controller.Debug("HeroCrouchAction false");

                    return EBTState.FAILED;
                }

                heroBB.controller.Debug("HeroCrouchAction running");

                return EBTState.RUNNING;
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
                    return EBTState.SUCCESS;
            }
            return EBTState.FAILED;
        }

        static EBTState ConditionAxisDown(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;
            AnimatorStateInfo info = heroBB.controller.StateInfo;

            heroBB.controller.Debug("ConditionAxisDown");


            if (IsCurFrameInputState(heroBB, EInputState.DOWN_KEY))
            {
                heroBB.controller.ControlEnabled = false;
                return EBTState.SUCCESS;
            }

            if(Animator.StringToHash(heroBB.aniStateCrouching) == info.fullPathHash ||
                Animator.StringToHash(heroBB.aniStateStandUp) == info.fullPathHash)
            {
                heroBB.controller.Debug("ConditionAxisDown stand up");
                return EBTState.RUNNING;
            }

            heroBB.controller.Debug("ConditionAxisDown false");

            if (heroBB.controller.CurrentState == EState.CROUCH)
                heroBB.controller.CurrentState = EState.IDLE;

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
            if (IsCurFrameInputState(heroBB, EInputState.ACTION_1))
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