using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerState _currentState;
    private PlayerController pc;

    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerCrouchState CrouchState { get; private set; }
    public PlayerRollState RollState { get; private set; }
    public PlayerClimbState ClimbState { get; private set; }

    void Awake()
    {
        pc = GetComponent<PlayerController>();

        IdleState = new PlayerIdleState(this, pc);
        WalkState = new PlayerWalkState(this, pc);
        RunState = new PlayerRunState(this, pc);
        JumpState = new PlayerJumpState(this, pc);
        WallSlideState = new PlayerWallSlideState(this, pc);
        WallJumpState = new PlayerWallJumpState(this, pc);
        CrouchState = new PlayerCrouchState(this, pc);
        RollState = new PlayerRollState(this, pc);
        ClimbState = new PlayerClimbState(this, pc);
    }

    void Start()
    {
        ChangeState(IdleState);
    }

    public void ChangeState(PlayerState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();

        Debug.Log("[New State] > " + _currentState);
    }

    void Update()
    {
        _currentState?.Update();
    }
    void FixedUpdate()
    {
        _currentState?.FixedUpdate();
    }

    public void Move(InputAction.CallbackContext ctx) => _currentState?.Move(ctx);
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            pc.currentJumpBufferTime = pc.jumpBufferTime;
            pc.isJumpHeld = true;
        }
        else if (ctx.canceled) pc.isJumpHeld = false;

        _currentState?.Jump(ctx);
    }
    public void WallJump(InputAction.CallbackContext ctx)
    {
        _currentState?.WallJump(ctx);
    }
    public void Sprint(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) pc.isSprinting = true;
        else if (ctx.canceled) pc.isSprinting = false;
        _currentState?.Sprint(ctx);
    }

    public void Crouch(InputAction.CallbackContext ctx) => _currentState?.Crouch(ctx);
    public void Roll(InputAction.CallbackContext ctx) => _currentState?.Roll(ctx);
    public void Climb(InputAction.CallbackContext ctx) => _currentState?.Climb(ctx);
}
