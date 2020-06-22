using UnityEngine;
using System.Collections.Generic;

using LSG.LWBehaviorTree;

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

                crouchState.Add(new DecoratorNode( new Condition() { onUpdate = ConditionJumpButton, onStart = null }));
                crouchState.Add(new DecoratorNode( new Condition() { onUpdate = ConditionJumpButtonDown, onStart = null}));
                crouchState.Add(new NotDecoratorNode( new Condition() { onUpdate = ConditionAxisDown, onStart = null }));
                crouchState.Add(new DecoratorNode( CreateNodeTask<HeroCrouchAction>()));
                crouchState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionJumpButton, onStart = null }));
                crouchState.Add(new DecoratorNode( new Condition() { onUpdate = ConditionActionButtonDown, onStart = null }));
            }

            public void Update()
            {
                root.Update(m_bb);
            }
        }

        internal class HeroCrouchAction : ActionNode
        {
            public HeroCrouchAction() :base()
            {
                Debug.Log("HeroCrouchAction constructor");
            }
            public override void OnStart(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                heroBB.playerAniController.CurrentState = EState.CROUCH;
            }

            public override EBTState Update(IBlackboard bb)
            {
                HeroBlackboard heroBB = (HeroBlackboard)bb;
                AnimatorStateInfo info = heroBB.playerAniController.Animator.GetCurrentAnimatorStateInfo(0);
                
                if( info.fullPathHash == Animator.StringToHash(heroBB.aniStateCrouching))
                {
                    heroBB.playerAniController.ControlEnabled = false;
                    return EBTState.SUCCESS;
                }else if ((info.fullPathHash == Animator.StringToHash(heroBB.aniStateJumpFall)) ||
                    info.fullPathHash == Animator.StringToHash(heroBB.aniStateJumping) ||
                    info.fullPathHash == Animator.StringToHash(heroBB.aniStateJumpClimb)
                    )
                {
                    heroBB.playerAniController.ControlEnabled = true;
                    return EBTState.FAILED;
                }

                return EBTState.RUNNING;
            }
        }

        static EBTState ConditionAxisDown(IBlackboard bb)
        {
            if (Input.GetAxisRaw("Vertical") < 0)
                return EBTState.SUCCESS;

            HeroBlackboard heroBB = (HeroBlackboard)bb;
            if (heroBB.playerAniController.CurrentState == EState.CROUCH)
                heroBB.playerAniController.CurrentState = EState.IDLE;

            return EBTState.FAILED;
        }

        static EBTState ConditionJumpButtonDown(IBlackboard bb)
        {
            if (Input.GetButtonDown("Jump") )
                return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

        static EBTState ConditionJumpButton(IBlackboard bb)
        {
            if (Input.GetButton("Jump"))
                return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

        static EBTState ConditionActionButtonDown(IBlackboard bb)
        {
            if (Input.GetButtonDown("Fire1"))
                return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

        static EBTState ConditionActionButton(IBlackboard bb)
        {
            if (Input.GetButton("Fire1"))
                return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

        static T CreateNodeTask<T>() where T : INodeTask, new()
        {
            return new T();
        }
    }
}