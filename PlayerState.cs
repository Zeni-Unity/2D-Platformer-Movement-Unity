using UnityEngine.InputSystem;

public class PlayerState
{
    public PlayerStateMachine sm { get; private set; }
    public PlayerController pc { get; private set; }
    public PlayerState(PlayerStateMachine _sm, PlayerController _pc) { sm = _sm; pc = _pc; }
    virtual public void Enter() { }
    virtual public void Update() { }
    virtual public void FixedUpdate() { }
    virtual public void Exit() { }


    virtual public void Move(InputAction.CallbackContext ctx) { }
    virtual public void Jump(InputAction.CallbackContext ctx) { }
    virtual public void WallJump(InputAction.CallbackContext ctx) { }
    virtual public void Sprint(InputAction.CallbackContext ctx) { }
    virtual public void Crouch(InputAction.CallbackContext ctx) { }
    virtual public void Roll(InputAction.CallbackContext ctx) { }
    virtual public void Climb(InputAction.CallbackContext ctx) { }
}
