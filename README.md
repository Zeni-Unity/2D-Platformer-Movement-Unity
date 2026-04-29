# 2D Platformer Movement
 
A modular 2D platformer movement system for Unity built around a State Machine pattern. Each movement behaviour lives in its own state class, making it straightforward to add, remove, or tweak individual behaviours without touching unrelated code.
 
The system currently includes idle, walk, run, jump, crouch, wall slide, and wall jump. More advanced features like coyote time and jump buffering are planned.
 
 
## File Structure
 
```
PlayerController.cs           Holds all shared data, physics checks, and Inspector settings
PlayerState.cs                Base class that all states inherit from
PlayerStateMachine.cs         Manages state transitions and routes input events
States/
  PlayerIdleState.cs          Standing still, listens for movement, jump, and crouch input
  PlayerWalkState.cs          Walking, transitions to run, idle, jump, or crouch
  PlayerRunState.cs           Running at run speed while sprint is held
  PlayerJumpState.cs          Applies jump force and watches for landing or wall contact
  PlayerCrouchState.cs        Crouched movement at walk speed
  PlayerWallSlideState.cs     Sliding down a wall, sets up wall jump direction
  PlayerWallJumpState.cs      Launches player off wall with locked flip during jump
  PlayerStateTemplate.cs      Empty template for adding new states
```
 
 
## Setup
 
The Player GameObject needs the following components: Rigidbody2D, Animator, PlayerController, and PlayerStateMachine.
 
**Ground Check** — Set a child Transform as the Ground Check Pos, positioned just below the player's feet. Set Ground Radius to a small value like 0.1 and assign your ground LayerMask.
 
**Wall Check** — Set a child Transform as the Wall Check Pos, positioned at the side of the player facing the wall. Set Wall Radius to a small value like 0.1 and assign your wall LayerMask.
 
**Movement Settings** — Set Walk Speed, Run Speed, Jump Force, and Wall Slide Speed. Wall Slide Speed controls the gravity scale while sliding, so lower values mean a slower slide.
 
**Wall Jump Settings** — Wall Jump Time is the window in which a wall jump can be triggered after leaving the wall. Wall Jump Duration is how long the wall jump state lasts before returning to idle. Wall Jump Power is a Vector2 where X controls horizontal force and Y controls vertical force.
 
 
## Input Setup
 
The system uses Unity's new Input System with callback-based events. Wire up the following actions in your Player Input component to the corresponding methods on PlayerStateMachine:
 
- Move -> PlayerStateMachine.Move
- Jump -> PlayerStateMachine.Jump
- WallJump -> PlayerStateMachine.WallJump
- Sprint -> PlayerStateMachine.Sprint
- Crouch -> PlayerStateMachine.Crouch
- Roll -> PlayerStateMachine.Roll
## Animator Parameters
 
The following Bool and Float parameters must exist on your Animator Controller. They are set automatically by the system.
 
- isGrounded (Bool)
- yVelocity (Float)
- isWalking (Bool)
- isRunning (Bool)
- isCrouching (Bool)
- isCrouchingWalking (Bool)
- isWallSliding (Bool)
## Adding New States
 
Use PlayerStateTemplate.cs as your starting point. Copy it, rename the class, and add it to the States folder. Override only the methods you need: Enter and Exit for setup and cleanup, Update for per-frame logic, FixedUpdate for physics, and the input methods for responding to input.
 
To transition into your new state, call the following from within any existing state:
 
```csharp
sm.ChangeState(new YourNewState(sm, pc));
```
 
 
## Planned Features
 
- Coyote Time — brief jump window after walking off a ledge
- Jump Buffering — queuing a jump input just before landing
- Roll / Dash State
