using System.Collections;
using System.Dynamic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(PlayerStateMachine _sm, PlayerController _pc) : base(_sm, _pc) { }

    public override void Enter()
    {
        base.Enter();

        pc.rb.linearVelocity = new Vector2(pc.wallJumpDir * pc.wallJumpPower.x, pc.wallJumpPower.y);
        pc.wallJumpCounter = 0;

        pc.canFlip = false;
        if (pc.transform.localScale.x != pc.wallJumpDir)
        {
            pc._facingRight = !pc._facingRight;
            Vector3 localScale = pc.transform.localScale;
            localScale.x *= -1;
            pc.transform.localScale = localScale;
        }

        pc.StartCoroutine(WallJumpEnd());
    }

    private IEnumerator WallJumpEnd()
    {
        yield return new WaitForSeconds(pc.wallJumpDuration);
        sm.ChangeState(new PlayerIdleState(sm, pc));
    }

    public override void Exit()
    {
        base.Exit();
        pc.canFlip = true;
    }

    public override void Move(InputAction.CallbackContext ctx)
    {
        pc.moveDir = ctx.ReadValue<Vector2>();
    }
}