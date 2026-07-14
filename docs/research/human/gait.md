# Gait — Detailed Cycle Reference

Companion to [locomotion.md](locomotion.md) (which covers walk-vs-run mechanics).
This file is the detailed phase-by-phase reference for one gait cycle.

## The full cycle, percentage-timed (walking)

| Phase | % of cycle | What happens |
|---|---|---|
| Initial contact | 0% | Heel strikes ground ahead of COM |
| Loading response | 0–10% | Weight transfers on; opposite foot still down (double support) |
| Midstance | 10–30% | COM passes directly over the stance foot; single support |
| Terminal stance | 30–50% | Heel rises, weight rolls onto forefoot |
| Pre-swing | 50–62% | Opposite foot has landed (double support); toe-off begins |
| Initial swing | 62–75% | Foot leaves ground, knee flexes for clearance |
| Mid-swing | 75–87% | Leg swings forward, foot passes under the body |
| Terminal swing | 87–100% | Leg decelerates, foot prepares to plant |

Running compresses/removes the double-support windows and inserts a flight
phase (~0% ground contact) between toe-off and the next foot's contact.

## Stride vs step

- **Step:** one foot's contact to the other foot's contact (half a cycle).
- **Stride:** one foot's contact to that SAME foot's next contact (full cycle).
- Left and right legs run the identical cycle above, offset by exactly 50%
  (one step) — this is the formal definition of the "opposition" pattern.

## Cadence and speed

- Average adult walking cadence: ~100–120 steps/min.
- Average running cadence at moderate pace: ~150–180 steps/min; elite
  distance runners often cluster near 180.
- Cadence rises with speed but has diminishing returns — beyond a point,
  further speed comes from stride length, not turnover rate. Pure "legs spin
  faster" animation without a stride-length increase reads as unnatural at
  high speed (the classic cartoon running trope, useful only as an intentional
  stylization, not a default).

## Arm-leg coupling within the cycle

Arm swing is phase-linked to the OPPOSITE leg with a small lag (not zero,
not exact antiphase) — the arm continues slightly past where perfect antiphase
would place it, an effect of the arm's own pendulum dynamics being lighter and
less damped than the leg's weight-bearing cycle.

## Game application (Cyber Hoops)

- GaitClock's [0,1) phase and the L/R 50% offset already match this table
  exactly — the phase-percentage breakdown above is the authoritative source
  if any future work (foot IK ground-snap timing, footstep SFX triggers) needs
  to know exactly when a foot should be "down" vs "in swing."
- The persona review's `ArmPhaseLag` fix (arms trail by 0.25 rad rather than
  perfect antiphase) is directly supported by the "arm-leg coupling" section
  above — real arm swing isn't exact antiphase either, so the fix made the rig
  MORE correct, not just less robotic-looking by coincidence.
- Footstep SFX (not yet implemented): trigger at 0% and 50% phase (initial
  contact) per this table, not at swing-phase midpoints.
- Cadence ceiling: if sprint ever needs "faster than max stride frequency"
  tuning, prefer raising StrideFrequency's effective stride LENGTH (already
  how MovementComponent's distance-driven clock naturally works) over
  artificially multiplying phase speed — keeps the arcade exaggeration
  physically grounded per this cadence-vs-length note.
