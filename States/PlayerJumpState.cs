using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerStateMachine _sm, PlayerController _pc) : base(_sm, _pc) { }

    public override void Enter()
    {
        base.Enter();
        pc.currentJumpBufferTime = -1f;
        pc.currentCoyoteTime = -1f;
        pc.rb.linearVelocity = new Vector2(pc.rb.linearVelocity.x, pc.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        if (pc.isGrounded) sm.ChangeState(new PlayerIdleState(sm, pc));
        if (pc.isOnWall && !pc.isGrounded) sm.ChangeState(new PlayerWallSlideState(sm, pc));
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float vy = pc.rb.linearVelocity.y;
        bool isFastFalling = pc.moveDir.y < -0.5f && vy < 0f;

        if (isFastFalling)
            pc.rb.gravityScale = pc.baseGravityScale * pc.fastFallMultiplier;
        else if (vy < 0f)
            pc.rb.gravityScale = pc.baseGravityScale * pc.fallGravityMultiplier;
        else if (Mathf.Abs(vy) < pc.apexThreshold)
            pc.rb.gravityScale = pc.baseGravityScale * pc.apexGravityMultiplier;
        else
            pc.rb.gravityScale = pc.baseGravityScale;
    }

    public override void Exit()
    {
        base.Exit();
        pc.rb.gravityScale = pc.baseGravityScale;
    }

    public override void Move(InputAction.CallbackContext ctx)
    {
        pc.moveDir = ctx.ReadValue<Vector2>();
    }

    public override void Jump(InputAction.CallbackContext ctx)
    {
        base.Jump(ctx);

        if (ctx.canceled && pc.rb.linearVelocity.y > 0f)
        {
            pc.rb.linearVelocity = new Vector2(pc.rb.linearVelocity.x, pc.rb.linearVelocity.y * pc.jumpCutMultiplier);
        }
    }

    public override void Roll(InputAction.CallbackContext ctx)
    {
        base.Roll(ctx);

        if (pc.currentRollCooldownTimer > 0) return;

        if (ctx.performed) sm.ChangeState(new PlayerRollState(sm, pc));
    }
}