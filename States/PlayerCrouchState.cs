using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouchState : PlayerState
{
    public PlayerCrouchState(PlayerStateMachine _sm, PlayerController _pc) : base(_sm, _pc) { }

    public override void Enter()
    {
        base.Enter();
        pc.animator.SetBool("isCrouching", true);
    }

    public override void Update()
    {
        base.Update();

        if (!pc.isGrounded)
        {
            sm.ChangeState(sm.IdleState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        pc.animator.SetBool("isCrouchingWalking", pc.rb.linearVelocity.x > 0.1f || pc.rb.linearVelocity.x < -0.1f);
        pc.rb.linearVelocity = new Vector2(pc.moveDir.x * pc.walkSpeed, pc.rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        pc.animator.SetBool("isCrouching", false);
        pc.animator.SetBool("isCrouchingWalking", false);
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
        if (ctx.performed) sm.ChangeState(sm.IdleState);
    }

    public override void Roll(InputAction.CallbackContext ctx)
    {
        base.Roll(ctx);
        if (pc.currentRollCooldownTimer > 0) return;
        if (ctx.performed) sm.ChangeState(sm.RollState);
    }
}