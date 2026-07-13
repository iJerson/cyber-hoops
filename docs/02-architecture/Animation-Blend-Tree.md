# Animation Blend Tree

Blend structure for `AnimationController`
(see [Animation-Architecture.md](Animation-Architecture.md)). Our poses are
procedural, but the structure is a classic blend tree; the same design maps 1:1 to a
Godot `AnimationTree` (equivalents noted) if we ever swap to authored clips.

## Tree

```
ROOT (final pose)
│
├─ LAYER 4  Head Look-At            [override: head channels only, weight 1 always]
│
├─ LAYER 3  Landing OneShot         [override: legs + pelvis squash]
│
├─ LAYER 2  Action OneShot          [override: full body]
│     ├─ Shoot   (dip→release→follow-through)
│     ├─ Layup   (gather→drive→release)
│     ├─ Dunk    (dip→flight→slam)   ◄ non-cancellable
│     ├─ Block   (dip→jump→reach)
│     ├─ Steal   (lunge→recover)
│     └─ Pass    (snap→recover)
│
├─ LAYER 1  Ball Layer              [override: arms + posture channels, weight = hasBall]
│     ├─ blend1d(NormalizedSpeed):
│     │      0.0  DribbleIdle pump   (bounce free-run tempo)
│     │      0.3  DribbleWalk pump   (bounce = gait-synced)
│     │      1.0  DribbleRun push    (forward push, bounce per 2–3 strides)
│     └─ Protect override (weight = defenderNear blend, 200ms):
│            shield bar + far-hip pump + sideways posture add
│
└─ LAYER 0  Locomotion Base         [1D blend space by NormalizedSpeed]
       0.00 Idle (breathing)
       0.02 Walk gait
       0.60 Run gait
       1.00 Sprint gait (max lean, max pump)
```

Godot mapping: Layer 0 = `BlendSpace1D`, Layer 1 = `Blend2`+`Add2`, Layers 2–3 =
`OneShot`, Layer 4 = `ModifyBone`/look-at modifier. Procedurally: each layer is a
pose function + per-channel weight; ROOT = weighted channel merge, higher layer wins
its channels.

## Phase sync (what makes Layer 0 blendable)

All four locomotion poses are driven by the SAME `GaitPhase` clock — they are one
gait evaluated at different amplitudes/leans, not four separate cycles. Blending
between adjacent points can therefore never produce double-steps or foot pops.
(Godot equivalent: sync tracks in the blend space.) `Jumping` has no cycle: airborne
poses (Block jump, Dunk flight, Layup flight) live in Layer 2 one-shots, not in the
locomotion space — see Interrupts.

## Blend (continuous) vs Interrupt (override)

### These BLEND — continuous weights, never snap

| Blend | Driver | Time |
|---|---|---|
| Idle ⇄ Walk ⇄ Run ⇄ Sprint | `NormalizedSpeed` (continuous value) | instantaneous by value — the value itself changes smoothly via accel/decel, so no timer needed |
| Ball layer in/out | pickup / release | 120ms weight ramp |
| DribbleIdle ⇄ DribbleWalk ⇄ DribbleRun | `NormalizedSpeed` | continuous, same rule as locomotion |
| Protect on/off | `defenderNear` (with hysteresis) | 200ms weight ramp |
| Head look-at target changes | focus switch | 150ms slerp |
| Bounce apex height | speed + pressure | continuous function, never stepped |

Why blend: these are *modulations of an ongoing activity*. The player is always
"locomoting"; speed and possession change how it looks. Snapping any of them reads
as a glitch because the underlying activity never stopped.

### These INTERRUPT — one-shots that seize their channels

| Interrupt | Seizes | Blend-in | Cancellable? |
|---|---|---|---|
| Shoot | full body | 60ms (dip is the blend-in) | yes, until release tick |
| Layup | full body | 60ms | yes, until release tick |
| **Dunk** | full body | 50ms | **NO — the one locked window** |
| Block | full body | 50ms dip | no after takeoff (airborne) |
| Steal | arms + lean (legs keep locomotion!) | 40ms | auto-recovers 150ms |
| Pass | arms + torso | 40ms | yes, until release tick |
| Landing | legs + pelvis | instant on touch | no (100–150ms, short) |

Why interrupt: these are *discrete events with a contact moment*. Blending a shoot
50/50 with a run produces a pose that is neither — the contact moment (release,
slam, touch) must be unambiguous. Fast blend-in (40–60ms, hidden inside the
pre-load dip) keeps responsiveness; the override guarantees pose integrity at the
moment that matters.

Steal is the deliberate hybrid: arms/lean override while legs continue the
locomotion blend — the player can steal while moving without a hitch.

## Interrupt arbitration

1. Higher layer always wins its channels; within Layer 2, a new one-shot **replaces**
   the current one only if the current is cancellable (table above). Requests
   against Dunk flight are dropped, not queued.
2. Landing (Layer 3) fires on touch-down regardless of what Layer 2 was doing —
   it only owns legs/pelvis, so a shoot follow-through can finish above it.
3. Head layer (4) is never interrupted and never blended out: whatever the body
   does, gaze logic owns the head (spec invariant 6). Exception encoded in the
   look-at logic itself: Steal targets the ball; Dunk targets the rim.
4. After any Layer 2 one-shot ends, control returns to whatever Layers 0/1 currently
   evaluate (Recover state supplies the 150ms re-blend) — one-shots never "return
   to" a stored pose, they return to the live tree.

## Jumping and Landing specifics

- There is no standalone Jump state/clip: every jump belongs to its action (Block,
  Dunk, Layup). The airborne section is the middle of that one-shot.
- Landing always plays as its own short override so touch-down feels weighty even
  if the action above is still finishing (e.g. layup follow-through).
- Squash & stretch triggers ride the landing one-shot (arcade layer), never the
  locomotion blend.

## Weight summary per channel group

| Channel group | Owner (normal) | Stolen by |
|---|---|---|
| Legs, feet | Layer 0 | Landing, Dunk/Block/Layup flight |
| Pelvis | Layer 0 (+ Layer 1 crouch add) | Layer 2, Landing |
| Torso/spine | Layer 0 + Layer 1 add | Layer 2 |
| Ball-side arm | Layer 1 | Layer 2 |
| Free arm | Layer 0 (gait) / Layer 1 (shield) | Layer 2 |
| Head | Layer 4 always | never |
```
