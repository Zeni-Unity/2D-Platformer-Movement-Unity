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

        if (pc.isGrounded) sm.ChangeState(sm.IdleState);
        if (pc.isOnWall && !pc.isGrounded) sm.ChangeState(sm.WallSlideState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float currentSpeed = pc.rb.linearVelocity.x;
        float targetSpeed = pc.moveDir.x * (pc.isSprinting ? pc.runSpeed : pc.walkSpeed);
        float accel = Mathf.Sign(targetSpeed) != Mathf.Sign(currentSpeed) && targetSpeed != 0f ? pc.acceleration * pc.turnAccelMultiplier : pc.acceleration;
        float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);
        pc.rb.linearVelocity = new Vector2(newSpeed, pc.rb.linearVelocity.y);

        float vy = pc.rb.linearVelocity.y;
        bool isFastFalling = pc.moveDir.y < -0.5f && vy < 0f;
        if (isFastFalling) pc.rb.gravityScale = pc.baseGravityScale * pc.fastFallMultiplier;
        else if (vy < 0f) pc.rb.gravityScale = pc.baseGravityScale * pc.fallGravityMultiplier;
        else if (Mathf.Abs(vy) < pc.apexThreshold) pc.rb.gravityScale = pc.baseGravityScale * pc.apexGravityMultiplier;
        else pc.rb.gravityScale = pc.baseGravityScale;
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

        if (ctx.performed) sm.ChangeState(sm.RollState);
    }

    public override void Climb(InputAction.CallbackContext ctx)
    {
        base.Climb(ctx);

        if (ctx.performed && pc.canClimb) sm.ChangeState(sm.ClimbState);
    }
}