using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(PlayerStateMachine _sm, PlayerController _pc) : base(_sm, _pc) { }

    private float gravityScale;

    public override void Enter()
    {
        base.Enter();

        pc.animator.SetBool("isWallSliding", true);
        pc.isWallSliding = true;
        gravityScale = pc.rb.gravityScale;
        pc.rb.gravityScale = pc.wallSlideSpeed;
        pc.isWallJumping = false;
        pc.wallJumpDir = -pc.transform.localScale.x;
        pc.wallJumpCounter = pc.wallJumpTime;
    }


    public override void Update()
    {
        base.Update();

        if (pc.rb.linearVelocity.y > 0) pc.rb.linearVelocity = new Vector2(pc.rb.linearVelocity.x, 0);

        if (!pc.isOnWall || pc.isGrounded)
        {
            pc.isWallSliding = false;
            sm.ChangeState(new PlayerIdleState(sm, pc));
            return;
        }

        if (pc.isWallSliding)
        {
            if (pc._facingRight && pc.moveDir.x < -0.1f)
            {
                pc.isWallSliding = false;
                sm.ChangeState(new PlayerIdleState(sm, pc));
                return;
            }
            else if (!pc._facingRight && pc.moveDir.x > 0.1f)
            {
                pc.isWallSliding = false;
                sm.ChangeState(new PlayerIdleState(sm, pc));
                return;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        pc.rb.gravityScale = gravityScale;
        pc.animator.SetBool("isWallSliding", false);
        pc.isWallSliding = false;
    }

    public override void WallJump(InputAction.CallbackContext ctx)
    {
        base.WallJump(ctx);
        if (!pc.isOnWall) return;
        if (ctx.performed) sm.ChangeState(new PlayerWallJumpState(sm, pc));
    }

    public override void Move(InputAction.CallbackContext ctx)
    {
        pc.moveDir = ctx.ReadValue<Vector2>();
    }
}