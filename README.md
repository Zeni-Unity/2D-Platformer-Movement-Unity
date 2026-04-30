# 2D Platformer Movement

A modular 2D platformer movement system for Unity built around a State Machine pattern. Each movement behaviour lives in its own state class, making it straightforward to add, remove, or tweak individual behaviours without touching unrelated code.

The system includes idle, walk, run, jump, crouch, roll, climb, wall slide, and wall jump — with coyote time, jump buffering, jump cut, and a full physics-based movement feel (acceleration, deceleration, turn boost, fall gravity, apex float, fast fall).


## File Structure

```
PlayerController.cs           Holds all shared data, physics checks, and Inspector settings
PlayerState.cs                Base class that all states inherit from
PlayerStateMachine.cs         Manages state transitions and routes input events
GhostFrame.cs                 Spawned during dash — fades out and destroys itself
States/
  PlayerIdleState.cs          Standing still, decelerates to a stop, listens for movement/jump/crouch
  PlayerWalkState.cs          Acceleration-based walking, handles jump buffer and transitions
  PlayerRunState.cs           Acceleration-based running with turn boost
  PlayerJumpState.cs          Jump with cut, fall gravity, apex float, and fast fall
  PlayerCrouchState.cs        Crouched movement at walk speed
  PlayerRollState.cs          Roll on ground or in air; optionally used as a dash with ghost frames
  PlayerClimbState.cs         Full directional movement on climbable surfaces, gravity disabled
  PlayerWallSlideState.cs     Sliding down a wall, sets up wall jump direction
  PlayerWallJumpState.cs      Launches player off wall with locked flip during jump
  PlayerStateTemplate.cs      Empty template for adding new states
```


## Setup

The Player GameObject needs the following components: Rigidbody2D, Animator, PlayerController, and PlayerStateMachine.

**Ground Check** — Set a child Transform as the Ground Check Pos, positioned just below the player's feet. Set Ground Radius to a small value like 0.1 and assign your ground LayerMask.

**Wall Check** — Set a child Transform as the Wall Check Pos, positioned at the side of the player facing the wall. Set Wall Radius to a small value like 0.1 and assign your wall LayerMask.

**Climb Check** — Set a child Transform as the Climb Check Pos, positioned at the center of the player. Set Climb Radius to cover the width of your ladder or climbable object and assign your climb LayerMask. The player enters climb mode when this overlap is active and the Climb input is pressed. They exit automatically when the overlap ends or when they jump.

**Movement Settings** — All values are tunable in the Inspector with Tooltips explaining each one.

- **Walk Speed / Run Speed** — Target horizontal speeds for walking and sprinting.
- **Climb Speed** — Movement speed in all directions while climbing.
- **Acceleration / Deceleration** — How quickly the player reaches target speed or comes to a stop.
- **Coyote Time** — How long after walking off a ledge the player can still jump (seconds).
- **Jump Buffer Time** — How early before landing a jump input is remembered and triggered (seconds).

**Jump Feel** — A separate Inspector section for tuning the jump arc.

- **Jump Cut Multiplier** — Fraction of upward velocity kept when the jump button is released early. Lower = shorter minimum jump.
- **Fall Gravity Multiplier** — Extra gravity while falling. Higher = snappier fall.
- **Apex Gravity Multiplier** — Reduced gravity near the top of the arc. Lower = floatier apex.
- **Apex Threshold** — Vertical speed below which apex gravity activates. Higher = wider apex window.
- **Fast Fall Multiplier** — Extra gravity when holding down while airborne.
- **Turn Accel Multiplier** — Acceleration boost when changing direction. Higher = snappier turnaround.

**Roll / Dash Settings** — Roll works both on the ground and in the air. Enable Use Roll As Dash in the Inspector to add ghost frames during the roll.

- **Roll Ground Speed / Time** — Impulse force and duration while rolling on the ground.
- **Roll Air Speed / Time** — Impulse force and duration while rolling in the air.
- **Roll Cooldown** — Minimum time between rolls.
- **Ghost Frame Start Opacity** — Starting alpha of each ghost frame sprite.
- **Ghost Frame Death Time** — How long each ghost frame takes to fade out.
- **Ghost Frame Spawn Time** — Interval between ghost frame spawns during the dash.
- **Ghost Frame Color** — Tint color applied to each ghost frame.

**Wall Jump Settings** — Wall Jump Time is the window in which a wall jump can be triggered after leaving the wall. Wall Jump Duration is how long the wall jump state lasts before returning to idle. Wall Jump Power is a Vector2 where X controls horizontal force and Y controls vertical force.


## Input Setup

The system uses Unity's new Input System with callback-based events. Wire up the following actions in your Player Input component to the corresponding methods on PlayerStateMachine:

- Move → PlayerStateMachine.Move
- Jump → PlayerStateMachine.Jump
- WallJump → PlayerStateMachine.WallJump
- Sprint → PlayerStateMachine.Sprint
- Crouch → PlayerStateMachine.Crouch
- Roll → PlayerStateMachine.Roll
- Climb → PlayerStateMachine.Climb


## Animator Parameters

The following Bool and Float parameters must exist on your Animator Controller. They are set automatically by the system.

- isGrounded (Bool)
- yVelocity (Float)
- isWalking (Bool)
- isRunning (Bool)
- isCrouching (Bool)
- isCrouchingWalking (Bool)
- isWallSliding (Bool)
- isRolling (Bool)
- isClimbing (Bool)


## Adding New States

Use PlayerStateTemplate.cs as your starting point. Copy it, rename the class, and add it to the States folder. Override only the methods you need: Enter and Exit for setup and cleanup, Update for per-frame logic, FixedUpdate for physics, and the input methods for responding to input.

Register the new state in PlayerStateMachine.cs the same way as the existing ones — declare a public property, instantiate it in Awake, and transition into it with:

```csharp
sm.ChangeState(sm.YourNewState);
```


## Planned Features

- Demo Scene
- WebGL Build
