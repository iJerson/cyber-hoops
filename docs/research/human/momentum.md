# Momentum

## Linear momentum (p = mv)

A moving body resists changes to its velocity proportional to its mass. In
human movement this shows up as:
- Heavier/bigger players decelerate and change direction more slowly for the
  same braking force — momentum, not just "weight," is why big players look
  less agile.
- Stopping quickly requires dissipating momentum through the ground via
  friction — the two-step brake pattern (long step, short step, both sinking
  through the knees) exists specifically to spread that force over more time
  and more joint travel, reducing peak joint stress.
- Momentum carries THROUGH a contact if not fully absorbed — an incompletely
  stopped player will drift/stumble in their prior direction of travel, which
  is a real cue, not just an animation flourish.

## Angular momentum (L = Iω)

Momentum of rotation, where I (moment of inertia) depends on how mass is
distributed relative to the rotation axis. Tucking limbs in reduces I,
increasing spin rate for the same angular momentum (figure-skater effect,
referenced in center_of_mass.md). This is conserved absent external torque —
a spinning body in the air cannot change its total rotation rate by moving
limbs, only redistribute where the speed happens (tuck to spin faster now,
extend to slow down before landing square).

## Momentum transfer between body and object

Throwing/shooting a ball transfers momentum from the body's kinetic chain to
the ball — the "follow-through" exists partly because the arm's momentum must
go somewhere even after release; abruptly stopping the arm at the release
point would require an equal-and-opposite braking force that changes the
release itself (this is the biomechanical reason follow-through discipline
affects shot consistency in real players, not just style).

## Momentum and first-step explosiveness

The explosive first step (per footwork.md/jump_mechanics.md) works partly by
using the body's LEAN (a controlled partial fall, converting gravitational
potential energy into forward momentum) combined with a powerful push — not
purely muscular force from a standing start. This is why elite first steps
begin with the body already tipping forward before the foot even leaves the
ground.

## Game application (Cyber Hoops)

- Ball physics (RigidBody3D) already carries real linear momentum through
  bounces/collisions — no extra work needed there.
- Character momentum is currently velocity-based in MovementComponent with
  accel/decel rates but no mass-scaling — fine for a single player-controlled
  character with fixed stats; would matter if player archetypes with different
  "weight" ever ship (heavier character = lower Deceleration stat, not a new
  system).
- Follow-through justification: the existing dip→release pattern for shots
  should ideally NOT hard-stop the arm exactly at ball release — a brief
  continued arm motion after release (already implicit in the arm returning to
  neutral via MoveToward rather than snapping) is the correct physical
  behavior, already achieved incidentally by the blend-based animation system.
- Explosive-start lean-before-push: matches the arcade spec's "ball thrown
  ahead of the body" note in dribbling.md — momentum theory confirms this
  isn't just a game-feel trick, real sprinters do lean before push-off.
