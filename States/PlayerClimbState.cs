using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClimbState : PlayerState
{
    public PlayerClimbState(PlayerStateMachine _sm, PlayerController _pc) : base(_sm, _pc) { }

    public override void Enter()
    {
        base.Enter();
        pc.rb.gravityScale = 0f;
        pc.rb.linearVelocity = Vector2.zero;
        pc.animator.SetBool("isClimbing", true);
    }

    public override void Update()
    {
        base.Update();

        // Leiter verlassen
        if (!pc.canClimb)
        {
            sm.ChangeState(sm.IdleState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        // Alles hier — kein Konflikt mit Update
        float xVel = pc.moveDir.x * pc.climbSpeed;
        float yVel = pc.moveDir.y * pc.climbSpeed;
        pc.rb.linearVelocity = new Vector2(xVel, yVel);
    }

    public override void Exit()
    {
        base.Exit();
        pc.rb.gravityScale = pc.baseGravityScale;
        pc.animator.SetBool("isClimbing", false);
    }

    public override void Move(InputAction.CallbackContext ctx)
    {
        pc.moveDir = ctx.ReadValue<Vector2>();
    }

    public override void Jump(InputAction.CallbackContext ctx)
    {
        base.Jump(ctx);
        if (ctx.performed) sm.ChangeState(sm.JumpState);
    }
}