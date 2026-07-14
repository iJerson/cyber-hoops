# Locomotion — Walking and Running

## Walking vs running: the defining difference

- **Walking:** at least one foot always touches the ground (double-support
  phase exists). Body vaults over the stance leg like an inverted pendulum —
  center of mass rises slightly at midstance, falls slightly between steps.
- **Running:** a flight phase exists where neither foot touches the ground.
  Body behaves like a bouncing spring-mass system — center of mass falls during
  stance (leg compresses like a spring) and rises during flight.
- **Practical threshold:** the gait transition (walk→run) happens at a
  predictable relative speed (Froude number ≈ 0.5) — not an arbitrary
  designer-picked speed cutoff, though games commonly fudge this for feel.

## Gait cycle phases (one leg)

1. **Initial contact (heel/foot strike)** — 0%
2. **Loading response** — weight transfers onto the leg, ~0–10%
3. **Midstance** — body passes directly over the foot, COM at its highest
   (walking) or lowest (running) point, ~10–30%
4. **Terminal stance / push-off** — heel rises, ankle plantarflexes, propels
   forward, ~30–50%
5. **Swing phase** — foot off the ground, leg swings forward, ~50–100%

Left and right legs are phase-offset by exactly 50% in symmetric gait — this is
the mathematical statement of "opposition" from the basketball research.

## Speed scaling

- Walking: speed increases mainly via longer stride length, secondarily via
  cadence (steps/min).
- Running: speed increases via both stride length AND cadence increasing
  together, plus flight-phase duration lengthening.
- Cadence has a comfortable range (~180 steps/min is common at moderate run
  pace); pushing cadence far outside natural ranges reads as unnatural
  regardless of correct phase.

## Turning while moving

Turning during locomotion is not a separate gait — it's the same cycle with
asymmetric stride length/direction per leg, plus trunk counter-rotation. Sharp
turns require a widened stance and a plant step (see basketball footwork.md)
because pure gait-cycle turning is limited to gentle arcs.

## Game application (Cyber Hoops)

- MovementComponent's GaitClock already implements distance-driven phase — the
  50% left/right offset is exactly what AnimationController's `Sin(gaitAngle)`
  vs `Sin(gaitAngle + π)` pair encodes for the hips.
- Walk→run visual blend keyed to NormalizedSpeed is a reasonable arcade
  simplification of the Froude-number transition — real players don't
  "run-walk" continuously, but for continuous gameplay speed this avoids a
  discrete gait-swap pop, which matters more for game feel than biomechanical
  purity here (gameplay-designer persona would side with the continuous blend).
- Turning: current MovementComponent rotates the whole body toward velocity
  without a distinct plant-and-pivot phase — acceptable for arcade half-court
  play; would need footwork.md's plant-first sequencing only if sharp
  direction-change animation becomes a dedicated mechanic (crossover state).
