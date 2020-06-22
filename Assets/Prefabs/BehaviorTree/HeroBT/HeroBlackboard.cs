using LSG;
using LSG.LWBehaviorTree;
using System;
using UnityEngine;

public class HeroBlackboard : ScriptableObject, IBlackboard
{
    [SerializeField]
    public string aniStateCrouching = "Base Layer.STATE_CROUCH.STATE_CROUCHING";
    [SerializeField]
    public string aniSateStandup = "Base Layer.STATE_CROUCH.STATE_STAND_UP";
    [SerializeField]
    public string aniStateJumping = "Base Layer.STATE_JUMP.STATE_JUMPING";
    [SerializeField]
    public string aniStateJumpClimb = "Base Layer.STATE_JUMP.STATE_JUMP_CLIMB";
    [SerializeField]
    public string aniStateJumpFall = "Base Layer.STATE_JUMP.STATE_JUMP_FALL";
    [SerializeField]
    public string aniStateIdle = "Base Layer.STATE_IDLE";
    [SerializeField]
    public string aniStateCrouchAttack = "Base Layer.STATE_ATTACK.STATE_CROUCH_ATTACK";
    [SerializeField]
    public string aniStateJumpAttack = "Base Layer.STATE_ATTACK.STATE_JUMP_ATTACK";
    [SerializeField]
    public string aniNormalAttack = "Base Layer.STATE_ATTACK.STATE_NORMAL_ATTACK";

    [NonSerialized]
    public Animator animator;

    [NonSerialized]
    public PlayerAniController playerAniController;
}
