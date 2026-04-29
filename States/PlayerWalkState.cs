using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkState : PlayerState
{
    public PlayerWalkState(PlayerStateMachine _sm, PlayerController _pc) : base(_sm, _pc) { }

    public override void Enter()
    {
        base.Enter();
        pc.animator.SetBool("isWalking", true);
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.Distance(pc.moveDir, Vector2.zero) < 0.01f) { sm.ChangeState(new PlayerIdleState(sm, pc)); }
        if (pc.isSprinting) { sm.ChangeState(new PlayerRunState(sm, pc)); }
        if (pc.isOnWall && !pc.isGrounded) sm.ChangeState(new PlayerWallSlideState(sm, pc));
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        pc.rb.linearVelocity = new Vector2(pc.moveDir.x * pc.walkSpeed, pc.rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        pc.animator.SetBool("isWalking", false);
    }

    public override void Move(InputAction.CallbackContext ctx)
    {
        pc.moveDir = ctx.ReadValue<Vector2>();
    }

    public override void Jump(InputAction.CallbackContext ctx)
    {
        base.Jump(ctx);
        if (!pc.isGrounded) return;
        if (ctx.performed) sm.ChangeState(new PlayerJumpState(sm, pc));
    }

    public override void Crouch(InputAction.CallbackContext ctx)
    {
        base.Crouch(ctx);

        if (ctx.performed) sm.ChangeState(new PlayerCrouchState(sm, pc));
    }

    public override void Roll(InputAction.CallbackContext ctx)
    {
        base.Roll(ctx);

        if (pc.currentRollCooldownTimer > 0) return;

        if (ctx.performed) sm.ChangeState(new PlayerRollState(sm, pc));
    }
}
