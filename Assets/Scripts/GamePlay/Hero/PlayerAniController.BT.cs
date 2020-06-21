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

            internal HeroBT()
            {
                root = new RepeatNode();
                SequenceNode crouchState = new SequenceNode();
                root.Add(crouchState);

                crouchState.Add(new DecoratorNode( new Condition() { onUpdate = JumpButton, onStart = null }));
                crouchState.Add(new DecoratorNode( new Condition() { onUpdate = JumpButtonDown, onStart = null}));
                crouchState.Add(new NotDecoratorNode( new Condition() { onUpdate = AxisDown, onStart = null }));
                crouchState.Add(new DecoratorNode( CreateNodeTask<HeroCrouchAction>()));
                crouchState.Add(new DecoratorNode( new Condition() { onUpdate = ActionButtonDown, onStart = null }));
            }

            public void Update(IBlackboard bb)
            {
                root.Update(bb);
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
                PlayerAniController hero = (PlayerAniController)bb;
                hero.CurrentState = EState.CROUCH;
            }

            public override EBTState Update(IBlackboard bb)
            {
                return EBTState.RUNNING;
            }
        }

        static EBTState AxisDown(IBlackboard bb)
        {
            if (Input.GetAxisRaw("Vertical") < 0)
                return EBTState.SUCCESS;
            PlayerAniController hero = (PlayerAniController)bb;
            if (hero.CurrentState == EState.CROUCH)
                hero.CurrentState = EState.IDLE;

            return EBTState.FAILED;
        }

        static EBTState JumpButtonDown(IBlackboard bb)
        {
            if (Input.GetButtonDown("Jump") )
                return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

        static EBTState JumpButton(IBlackboard bb)
        {
            if (Input.GetButton("Jump"))
                return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

        static EBTState ActionButtonDown(IBlackboard bb)
        {
            if (Input.GetButtonDown("Fire1"))
                return EBTState.SUCCESS;
            return EBTState.FAILED;
        }

        static EBTState ActionButton(IBlackboard bb)
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