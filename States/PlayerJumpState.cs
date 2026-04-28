using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerStateMachine _sm, PlayerController _pc) : base(_sm, _pc) { }

    public override void Enter()
    {
        base.Enter();
        pc.rb.linearVelocity = new Vector2(pc.rb.linearVelocity.x, pc.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        if (pc.isGrounded) sm.ChangeState(new PlayerIdleState(sm, pc));
        if (pc.isOnWall && !pc.isGrounded) sm.ChangeState(new PlayerWallSlideState(sm, pc));
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Move(InputAction.CallbackContext ctx)
    {
        pc.moveDir = ctx.ReadValue<Vector2>();
    }
}