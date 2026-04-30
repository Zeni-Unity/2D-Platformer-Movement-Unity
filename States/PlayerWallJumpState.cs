using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(PlayerStateMachine _sm, PlayerController _pc) : base(_sm, _pc) { }

    private bool _controlLocked;
    private bool _jumpCutApplied;

    public override void Enter()
    {
        base.Enter();

        pc.rb.linearVelocity = new Vector2(pc.wallJumpDir * pc.wallJumpPower.x, pc.wallJumpPower.y);
        pc.wallJumpCounter = 0;
        _controlLocked = true;
        _jumpCutApplied = false;

        pc.canFlip = false;
        if (pc.transform.localScale.x != pc.wallJumpDir)
        {
            pc._facingRight = !pc._facingRight;
            Vector3 localScale = pc.transform.localScale;
            localScale.x *= -1;
            pc.transform.localScale = localScale;
        }

        pc.StartCoroutine(UnlockControl());
    }

    private IEnumerator UnlockControl()
    {
        yield return new WaitForSeconds(pc.wallJumpDuration);
        _controlLocked = false;
        pc.canFlip = true;
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

        float vy = pc.rb.linearVelocity.y;

        if (!pc.isJumpHeld && !_jumpCutApplied && vy > 0.01f)
        {
            _jumpCutApplied = true;
            Vector2 vel = pc.rb.linearVelocity;
            vel.y *= pc.jumpCutMultiplier;
            pc.rb.linearVelocity = vel;
        }

        bool isFastFalling = pc.moveDir.y < -0.5f && vy < 0f;
        if (isFastFalling) pc.rb.gravityScale = pc.baseGravityScale * pc.fastFallMultiplier;
        else if (vy < 0f) pc.rb.gravityScale = pc.baseGravityScale * pc.fallGravityMultiplier;
        else if (Mathf.Abs(vy) < pc.apexThreshold) pc.rb.gravityScale = pc.baseGravityScale * pc.apexGravityMultiplier;
        else pc.rb.gravityScale = pc.baseGravityScale;

        if (!_controlLocked)
        {
            float currentSpeed = pc.rb.linearVelocity.x;
            float targetSpeed = pc.moveDir.x * (pc.isSprinting ? pc.runSpeed : pc.walkSpeed);

            float accel;
            if (targetSpeed == 0f) accel = pc.deceleration;
            else if (Mathf.Sign(targetSpeed) != Mathf.Sign(currentSpeed) && currentSpeed != 0f) accel = pc.acceleration * pc.turnAccelMultiplier;
            else accel = pc.acceleration;

            float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);
            pc.rb.linearVelocity = new Vector2(newSpeed, pc.rb.linearVelocity.y);
        }
    }

    public override void Exit()
    {
        base.Exit();
        pc.canFlip = true;
        pc.rb.gravityScale = pc.baseGravityScale;
    }

    public override void Move(InputAction.CallbackContext ctx)
    {
        pc.moveDir = ctx.ReadValue<Vector2>();
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