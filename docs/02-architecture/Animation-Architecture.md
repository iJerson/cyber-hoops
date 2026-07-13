# Animation Component Architecture

Implements [Animation-Spec.md](../01-game-design/Animation-Spec.md) on the existing
component stack. Design only — no code here.

## Design rules

- Gameplay never waits for animation (responsiveness contract). Data flows one way:
  **gameplay → animation**. Animation components read state; they never mutate it.
- Every component keeps one responsibility; composition on the character scene, no
  inheritance.
- All tunables in Resources (`.tres`), zero magic numbers.
- Determinism: every phase/pose derives from gameplay state (distance travelled,
  gait phase, tick time) — no wall-clock, no randomness.

## Component map

```
Player / Opponent (CharacterBody3D)
├── InputComponent            (existing: PlayerInputComponent / AIInputComponent)
├── MovementComponent         (existing, gains GaitPhase)
├── PossessionComponent       (existing, slimmed)
├── DribbleComponent          (NEW — split out of Possession/Ball)
├── ShootingComponent         (existing)
├── DefenseComponent          (existing)
├── CharacterStateMachine     (NEW — animation/action states A–M, reuses Core.StateMachine)
├── AnimationController       (NEW — replaces pose logic in CharacterRig)
│   └── CharacterRig          (becomes dumb pose target surface)
│       └── FootIK            (NEW — post-pass on leg pivots)
Ball (RigidBody3D)
├── BallPhysics               (existing Ball modes: Free/Held/Dribbling)
└── BallVisual                (NEW — squash & stretch, decoupled from physics)
```

## Responsibilities

### 1. Input — `IMovementInputSource` (existing)

- Emits *intent* only: move direction, sprint, action edges (shoot/defend).
- Human polls the input map; AI brain writes desired values. Identical surface —
  nothing downstream knows who is driving.
- Never touches animation. Input latency budget lives here: intents are read on the
  physics tick they occur.

### 2. Movement — `MovementComponent` (existing, extended)

- Owns velocity, acceleration, rotation of the body. Unchanged core.
- **New output: `GaitPhase`** ∈ [0,1) — advanced by horizontal distance travelled
  × stride frequency. Also exposes `Speed`, `NormalizedSpeed` (0..1 vs sprint).
- GaitPhase is the master clock of the whole animation system (sync invariant).
  Movement owns it because movement owns distance.
- Publishes deceleration events (speed derivative) so the state machine can enter
  the Stop state.

### 3. Dribble — `DribbleComponent` (new)

- Split from PossessionComponent/Ball: owns *where and how the ball bounces* while
  possessed. PossessionComponent keeps only ownership/pickup/release rules.
- Responsibilities:
  - Derives `BouncePhase` from Movement.GaitPhase when moving (contact = opposite-
    foot plant), free-runs at gravity period × arcade multiplier when idle.
  - Computes bounce apex height from speed + defender proximity + locomotion state
    (idle / moving / protect) — the continuous-decay invariant lives here.
  - Positions the dribble anchor per state: front-outside (walk), thrown ahead
    (run), far hip low (protect).
  - Feeds Ball the anchor + phase; feeds AnimationController the ball height and
    phase for the arm pump.
- Data-driven via `DribbleStats` resource (heights, tempo multiplier, anchor
  offsets per state).

### 4. State Machine — `CharacterStateMachine` (new, reuses `Core.StateMachine`)

- One instance per character; states = spec sections A–M (Idle, IdleDribble,
  WalkDribble, RunDribble, Protect, Crossover, ExplosiveStart, Stop, JumpShot,
  Layup, Dunk, Steal, Block).
- Inputs: Movement (speed, heading change), Possession (has ball), Defense/Shooting
  (action events), defender proximity probe.
- Owns the transition table from the spec: triggers, hysteresis (protect 1.5m
  in / 2.0m out), pre-load dip queueing, non-interruptible windows (dunk flight).
- Outputs a single `CharacterAnimState` snapshot each tick: current state, time in
  state, blend progress, plus per-state params (dip progress, crossover stage).
- Gameplay components may *observe* it (e.g. Shooting queues behind a dip) but the
  machine never blocks a gameplay effect — it schedules presentation.
- Engine-free decision core (like AIDecider): pure transition function, unit-tested.

### 5. Animation — `AnimationController` (new)

- The only writer to the rig. Consumes: CharacterAnimState, GaitPhase, BouncePhase,
  ball height, focus target (hoop/opponent/ball).
- Responsibilities:
  - Pose bank: one procedural pose function per spec state (lean, pelvis, spine,
    shoulders, arms, hands, head) with magnitudes from an `AnimationStats` resource
    (sim values and arcade overrides both live there).
  - Blending: per-property MoveToward/Lerp with per-transition blend times from the
    spec table; enforces pelvis-leads ordering by lagging shoulder/head channels.
  - Layering (priority): base locomotion pose ← dribble arm layer ← action layer
    (shoot/dunk/steal) ← head look-at layer (always wins on head).
  - Arcade feel: dip compression, apex holds, squash/stretch triggers on the rig,
    exaggeration multipliers.
- `CharacterRig` demotes to a *pose surface*: exposes named channels (PelvisPitch,
  ShoulderPitchL/R, …) and the accent recolor. No behavior of its own.

### 6. Foot IK — `FootIK` (new, post-pass)

- Runs after AnimationController each tick, adjusts only leg/foot channels.
- Responsibilities:
  - Ground snap: cast from each foot, pin the planted foot during its stance half
    of GaitPhase (kills foot sliding — biggest visual win of IK).
  - Slope/step conformance on court edges (flat court now → cheap).
  - Plant lock during Crossover/Stop (spec F/H: the brake foot must not slide).
- Two-bone analytic solve on hip pivot + (future) knee joint; until knees exist,
  degrades to foot pinning + pelvis height compensation.
- Purely visual; never moves the CharacterBody3D.

### 7. Ball Physics — `Ball` (existing) + `BallVisual` (new)

- `Ball` keeps the three physics modes (Free/Held/Dribbling) and gameplay stamps
  (shooter team, shot distance). While Dribbling it now takes anchor + phase from
  the handler's DribbleComponent instead of running its own cycle.
- `BallVisual` (child of Ball): squash & stretch mesh deformation from physics —
  15% squash at contact, stretch by fall speed; trail hooks for the explosive-start
  exaggeration. Reads velocity/phase, never writes physics.

## Data flow (one tick)

```
InputComponent → MovementComponent → GaitPhase/Speed
                        ↓                    ↓
PossessionComponent → DribbleComponent → BouncePhase/ball anchor → Ball(Physics)
                        ↓                    ↓
        CharacterStateMachine  ←  action events (Shooting/Defense)
                        ↓
              CharacterAnimState
                        ↓
              AnimationController → CharacterRig channels
                        ↓
                     FootIK (post-pass)
                        ↓
                  BallVisual (reads Ball)
```

## Migration order

1. `GaitPhase` in MovementComponent (small, unblocks everything).
2. `DribbleComponent` extraction + bounce↔gait sync (biggest realism win).
3. `CharacterStateMachine` with A–E + H (locomotion set) — engine-free core + tests.
4. `AnimationController` + rig channel refactor; port existing dunk/dribble poses.
5. Arcade layer (dips, holds, exaggeration multipliers) via AnimationStats.
6. `BallVisual` squash & stretch.
7. `FootIK` last — needs knee joints on the rig to shine.
```
