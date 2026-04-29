using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRollState : PlayerState
{
    public PlayerRollState(PlayerStateMachine _sm, PlayerController _pc) : base(_sm, _pc) { }

    public override void Enter()
    {
        base.Enter();

        pc.animator.SetBool("isRolling", true);
        Vector2 rollDir = pc.moveDir == Vector2.zero || pc.moveDir.x == 0 ? pc._facingRight ? Vector2.right : Vector2.left : pc.isGrounded ? new Vector2(pc.moveDir.x, pc.rb.linearVelocity.y) : pc.moveDir;
        pc.StartCoroutine(Roll(rollDir));
    }

    IEnumerator Roll(Vector2 dir)
    {
        float ogGravity = pc.rb.gravityScale;
        pc.rb.gravityScale = 0f;
        pc.rb.linearVelocity = Vector2.zero;

        pc.rb.AddForce(dir * (pc.isGrounded ? pc.rollGroundSpeed : pc.rollAirSpeed), ForceMode2D.Impulse);

        yield return new WaitForSeconds(pc.isGrounded ? pc.rollGroundTime : pc.rollAirTime);

        pc.rb.gravityScale = ogGravity;

        sm.ChangeState(new PlayerIdleState(sm, pc));
        pc.animator.SetBool("isRolling", false);
        yield return null;
    }

    public override void Exit()
    {
        base.Exit();
        pc.animator.SetBool("isRolling", false);
        pc.currentRollCooldownTimer = pc.rollCooldown;
    }

    public override void Move(InputAction.CallbackContext ctx)
    {
        pc.moveDir = ctx.ReadValue<Vector2>();
    }
}
