using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine _sm, PlayerController _pc) : base(_sm, _pc) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        if (pc.moveDir.x != 0)
        {
            if (pc.isSprinting) sm.ChangeState(new PlayerRunState(sm, pc));
            else sm.ChangeState(new PlayerWalkState(sm, pc));
        }

        bool movingIntoWall = (pc._facingRight && pc.moveDir.x > 0.1f) || (!pc._facingRight && pc.moveDir.x < -0.1f);

        if (pc.isOnWall && !pc.isGrounded && movingIntoWall) sm.ChangeState(new PlayerWallSlideState(sm, pc));
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Move(InputAction.CallbackContext ctx)
    {
        pc.moveDir = ctx.ReadValue<Vector2>();
    }

    public override void Jump(InputAction.CallbackContext ctx)
    {
        base.Jump(ctx);

        if (ctx.performed) sm.ChangeState(new PlayerJumpState(sm, pc));
    }

    public override void Crouch(InputAction.CallbackContext ctx)
    {
        base.Crouch(ctx);

        if (ctx.performed) sm.ChangeState(new PlayerCrouchState(sm, pc));
    }
}
