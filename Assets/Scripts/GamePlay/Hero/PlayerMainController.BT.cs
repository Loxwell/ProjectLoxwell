using UnityEngine;
using LSG.LWBehaviorTree;

using EState = LSG.PlayerMainController.EState;
using EInputState = LSG.EInputState;

using static HeroBlackboard;
using static LSG.LWBehaviorTree.ActionNode;
using static BT.LSG.PlayerMainController;

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
                SelectNode subCrouchngState = new SelectNode();
                SequenceNode idleState = new SequenceNode();
                SelectNode subIdleState = new SelectNode();
                SelectNode subJumpingState = new SelectNode();

                root.Add(jumpingState);
                root.Add(crouchngState);
                root.Add(idleState);
             
                // 현재 버그 상황
                // 첫번째 점프 확인. 두번째 앉는 모션 확인. 마지막으로 서있는 상태 확인

                // 점프 중 아래를 누르면서 공격 할 경우  점프 후 한번 지상인지 조건 판단 함
                // 이후 앉아서 공격은 현재 점프해서 공격 및 점프 애니메이션이라서 앉는 모션이 동작 하지 못함
                // 그래서 공격 버튼을 연타 시 서있는 상태 이지만 애니메이션은 점프 공격과 공중 점프 상태 사이를 오감
                // 현재 공격 모션이면 공격 버튼 정보 버릴지는 판단 할 것

                // 움직임 BT와 공격 BT를 분리 할 것

                // Crouch State
                crouchngState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionAxisDown, onStart = null }));
                crouchngState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionIsGrounded, onStart = null }));
                crouchngState.Add(subCrouchngState);
                crouchngState.Add(new DecoratorNode(CreateNodeTask<ActionHeroCrouch>()));
                crouchngState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionActionButtonDown, onStart = null }));
                crouchngState.Add(new NegationDecoratorNode(CreateNodeTask<ActionHeroCrouchAttack>()));
                subCrouchngState.Add(new NegationDecoratorNode(new Condition() { onUpdate = ConditionHeroJumpAniState, onStart = 
                    (IBlackboard v) => { Debug.Log("앉으려고 하는데 점프 모션이면"); }
                }));
                subCrouchngState.Add(new NegationDecoratorNode(new Condition() { onUpdate = ConditionHeroJumpClimbAniState, onStart = 
                    (IBlackboard v) => { Debug.Log("앉으려고 하는데 점프 중 올라가기"); } }));
                subCrouchngState.Add(new NegationDecoratorNode(CreateNodeTask<ActionHeroIdle>()));

                // Jump State
                jumpingState.Add(new NegationDecoratorNode(new Condition() { onUpdate = ConditionIsGrounded, onStart = (IBlackboard value)=> {
                    ((HeroBlackboard)value).controller.Print("점프 단계 지상 조건 판단");
                } }));
                jumpingState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionHeroJumping, onStart = (IBlackboard value)=> {
                    ((HeroBlackboard)value).controller.Print("점프 단계 현재 점프중인지 판단");
                } }));
                jumpingState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionActionButtonDown, onStart = null }));
                jumpingState.Add(subJumpingState);
                subJumpingState.Add(new NegationDecoratorNode(CreateNodeTask<ActionJumpAttack>()));
                subJumpingState.Add(new NegationDecoratorNode(new Condition()
                {
                    onUpdate = ConditionIsGrounded,
                    onStart = (IBlackboard value) => {
                        Debug.Log("점프 단계 공격 후 판단");
                    }
                }));
                subJumpingState.Add(new DecoratorNode(CreateNodeTask<ActionHeroIdle>()));

                // 그 외 상태
                idleState.Add(new DecoratorNode(new Condition() { onUpdate = ConditionActionButton, onStart = null }));
                idleState.Add(subIdleState);
                idleState.Add(new DecoratorNode(CreateNodeTask<ActionStandAttack>()));
                idleState.Add(new DecoratorNode(CreateNodeTask<ActionHeroIdle>()));

                subIdleState.Add(new NegationDecoratorNode(new Condition() { onUpdate = ConditionMovingState, onStart = null}));
                subIdleState.Add(new DecoratorNode(CreateNodeTask<ActionStopMoving>()));
            }

            public void Update()
            {
                root.Update(m_bb);
            }
        }

    
        static EBTState ConditionIsGrounded(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;

            if (heroBB.isGrounded)
            {
                heroBB.controller.Print("ConditionIsGrounded");
                return EBTState.SUCCESS;
            }

            return EBTState.FAILED;
        }

        static EBTState ConditionActionButtonDown(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;
            if (IsCurFrameInputState(heroBB, EInputState.ACTION_1) && !IsPreFrameInputState(heroBB, EInputState.ACTION_1))
            {
#if UNITY_EDITOR

                heroBB.controller.Print("ConditionActionButtonDown");
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

        static EBTState ConditionMovingState(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;
            if (IsCurFrameInputState(heroBB, EInputState.RIGHT_KEY) ||
                IsCurFrameInputState(heroBB, EInputState.LEFT_KEY))
                return EBTState.SUCCESS;

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

        static EBTState ConditionHeroControlEnabled(IBlackboard bb)
        {
            HeroBlackboard heroBB = (HeroBlackboard)bb;

            if (heroBB.isControled)
                return EBTState.SUCCESS;

            return EBTState.FAILED;
        }
    }
}