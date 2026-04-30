using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRollState : PlayerState
{
    public PlayerRollState(PlayerStateMachine _sm, PlayerController _pc) : base(_sm, _pc) { }

    private bool isRolling = false;
    private float currentGhostFrameSpawnTime;
    private bool _wasExitedExternally = false;

    public override void Enter()
    {
        base.Enter();

        _wasExitedExternally = false;
        pc.animator.SetBool("isRolling", true);
        Vector2 rollDir = pc.moveDir == Vector2.zero || pc.moveDir.x == 0
            ? pc._facingRight ? Vector2.right : Vector2.left
            : pc.isGrounded
                ? new Vector2(pc.moveDir.x, pc.rb.linearVelocity.y)
                : pc.moveDir;
        pc.StartCoroutine(Roll(rollDir));
    }

    public override void Update()
    {
        base.Update();

        if (isRolling && pc.useRollAsDash)
        {
            if (currentGhostFrameSpawnTime > 0)
            {
                currentGhostFrameSpawnTime -= Time.deltaTime;
            }
            else
            {
                NewGhostFrame();
                currentGhostFrameSpawnTime = pc.ghostFrameSpawnTime;
            }
        }
    }

    IEnumerator Roll(Vector2 dir)
    {
        isRolling = true;
        currentGhostFrameSpawnTime = pc.ghostFrameSpawnTime;
        float ogGravity = pc.rb.gravityScale;
        pc.rb.gravityScale = 0f;
        pc.rb.linearVelocity = Vector2.zero;

        pc.rb.AddForce(dir * (pc.isGrounded ? pc.rollGroundSpeed : pc.rollAirSpeed), ForceMode2D.Impulse);

        yield return new WaitForSeconds(pc.isGrounded ? pc.rollGroundTime : pc.rollAirTime);

        pc.rb.gravityScale = ogGravity;
        isRolling = false;

        if (!_wasExitedExternally)
        {
            pc.animator.SetBool("isRolling", false);
            sm.ChangeState(sm.IdleState);
        }
    }

    private void NewGhostFrame()
    {
        GameObject frame = new GameObject("Ghost frame");
        frame.transform.position = pc.gameObject.transform.position;
        frame.transform.localScale = pc.gameObject.transform.localScale;

        SpriteRenderer sr = frame.AddComponent<SpriteRenderer>();
        sr.sprite = pc.gameObject.GetComponent<SpriteRenderer>().sprite;

        Color color = pc.ghostFrameColor;
        color.a = pc.ghostFrameStartOpacity;
        sr.color = color;

        GhostFrame ghostFrame = frame.AddComponent<GhostFrame>();
        ghostFrame.deathTime = pc.ghostFrameDeathTime;
        ghostFrame.enableDeathTime = true;
    }

    public override void Exit()
    {
        base.Exit();
        _wasExitedExternally = true;
        isRolling = false;
        pc.animator.SetBool("isRolling", false);
        pc.currentRollCooldownTimer = pc.rollCooldown;
    }

    public override void Move(InputAction.CallbackContext ctx)
    {
        pc.moveDir = ctx.ReadValue<Vector2>();
    }
}