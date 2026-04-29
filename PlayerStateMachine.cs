using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerState _currentState;
    private PlayerController pc;

    void Awake()
    {
        pc = GetComponent<PlayerController>();
    }

    void Start()
    {
        ChangeState(new PlayerIdleState(this, pc));
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
        _currentState.Update();
    }
    void FixedUpdate()
    {
        _currentState.FixedUpdate();
    }

    public void Move(InputAction.CallbackContext ctx) => _currentState?.Move(ctx);
    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!pc.isGrounded) return;
        _currentState?.Jump(ctx);
    }
    public void WallJump(InputAction.CallbackContext ctx)
    {
        if (!pc.isOnWall) return;
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
}
