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

        if (Vector2.Distance(pc.moveDir, Vector2.zero) < 0.01f) { sm.ChangeState(sm.IdleState); }
        if (pc.isSprinting) { sm.ChangeState(sm.RunState); }
        if (pc.isOnWall && !pc.isGrounded) sm.ChangeState(sm.WallSlideState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float targetSpeed = pc.moveDir.x * pc.walkSpeed;
        float currentSpeed = pc.rb.linearVelocity.x;

        float accel = (pc.moveDir.x != 0) ? pc.acceleration : pc.deceleration;

        bool isTurning = pc.moveDir.x != 0 && Mathf.Abs(currentSpeed) > 0.1f && Mathf.Sign(pc.moveDir.x) != Mathf.Sign(currentSpeed);
        if (isTurning) accel *= pc.turnAccelMultiplier;

        float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);

        pc.rb.linearVelocity = new Vector2(newSpeed, pc.rb.linearVelocity.y);

        if (pc.isGrounded && pc.currentJumpBufferTime > 0)
        {
            sm.ChangeState(sm.JumpState);
        }
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