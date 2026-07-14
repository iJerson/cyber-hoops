# Balance

## The core rule

Balance is maintained by keeping the center of mass (COM) above (or its
predicted future position within) the base of support (BOS — the area
enclosed by ground-contact points, usually the feet). Fall risk isn't about
being tilted; it's about the COM's projection leaving the BOS with no way to
recover it.

## Static vs dynamic balance

- **Static:** COM projection must stay inside the BOS at all times (standing
  still). Small continuous postural sway is normal and constant — perfectly
  motionless standing doesn't exist.
- **Dynamic:** during locomotion the COM projection routinely moves OUTSIDE the
  current BOS — this is normal and expected, because the next foot placement
  is already "catching" it. Walking is technically a continuous controlled
  fall arrested by each new footfall.

## Recovery strategies (when balance is challenged)

1. **Ankle strategy** — small perturbations, corrected by ankle torque alone,
   body stays rigid.
2. **Hip strategy** — larger perturbations, corrected by bending at the hips
   (upper body counter-leans opposite the disturbance).
3. **Stepping strategy** — largest perturbations, a step or stumble relocates
   the BOS under the COM. This is the origin of the "off-balance stumble step"
   after a hard stop or contested landing.

## Anticipatory postural adjustments

Before any voluntary movement that will disturb balance (e.g. raising an arm
fast, taking a big step), the body pre-activates postural muscles a fraction
of a second BEFORE the movement starts — an anticipatory lean or weight shift.
This is a distinct phenomenon from the stretch-shortening pre-load dip, though
both are "wind-up before the visible action."

## Wide-base vs narrow-base tradeoff

Wider BOS = more stable but slower to initiate movement (COM has farther to
travel to leave a wide BOS in a chosen direction). Narrow BOS = less stable but
faster direction changes. This is precisely why defensive stance (wide) trades
speed for stability, and why sprinting (narrow base, feet nearly in one line)
trades stability for straight-line speed.

## Game application (Cyber Hoops)

- Base width already varies by context in the animation spec (defend/protect
  wide, sprint narrow) — this section is the "why," useful for justifying any
  future tuning question.
- Stepping-strategy stumble = the exact real-world justification for the
  "landing stumble-step" flourish suggested in dunking.md — it's not
  decorative, it's what actually happens under a hard landing.
- Anticipatory lean: the pre-load dip already covers most of this, but a
  distinct SMALL anticipatory weight-shift (not a full crouch) could precede
  quick non-explosive actions like a pass — a cheap future polish item, low
  priority.
- COM leaving the BOS during normal running is a reminder that "torso lean"
  during sprint (already in AnimationStats.SprintLean) is not a balance flaw
  to fix — it's correct dynamic balance, not a stumble.
