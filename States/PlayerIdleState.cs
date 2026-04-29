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
            if (pc.isSprinting) sm.ChangeState(sm.RunState);
            else sm.ChangeState(sm.WalkState);
        }

        bool movingIntoWall = (pc._facingRight && pc.moveDir.x > 0.1f) || (!pc._facingRight && pc.moveDir.x < -0.1f);

        if (pc.isOnWall && !pc.isGrounded && movingIntoWall) sm.ChangeState(sm.WallSlideState);

        if (pc.isGrounded && pc.currentJumpBufferTime > 0)
        {
            sm.ChangeState(sm.JumpState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float currentSpeed = pc.rb.linearVelocity.x;
        float newSpeed = Mathf.MoveTowards(currentSpeed, 0f, pc.deceleration * Time.fixedDeltaTime);
        pc.rb.linearVelocity = new Vector2(newSpeed, pc.rb.linearVelocity.y);
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
        if (!pc.canJump) return;
        if (ctx.performed) sm.ChangeState(sm.JumpState);
    }

    public override void Crouch(InputAction.CallbackContext ctx)
    {
        base.Crouch(ctx);

        if (ctx.performed) sm.ChangeState(sm.CrouchState);
    }

    public override void Roll(InputAction.CallbackContext ctx)
    {
        base.Roll(ctx);

        if (pc.currentRollCooldownTimer > 0) return;

        if (ctx.performed) sm.ChangeState(sm.RollState);
    }
}
