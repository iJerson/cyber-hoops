# Center of Mass

## Location (standing, arms at sides)

Roughly 55% of height from the floor, just anterior to the second sacral
vertebra (inside the pelvis, slightly toward the front) — i.e., almost exactly
where the pelvis sits. This is the biomechanical justification for treating
the pelvis as the "root" of a character rig: it IS approximately the body's
mass center.

## COM shifts with posture

- Raising both arms overhead raises the COM slightly and shifts it up toward
  the chest.
- Bending forward at the hips shifts COM forward and down.
- Crouching (bending knees/hips) lowers COM — the core mechanism of "getting
  low" for stability (wider effective stability margin, per balance.md).
- Carrying a held object (the ball, overhead) shifts the combined system's COM
  toward that object, proportional to its mass fraction of the total system
  (ball is ~1% of body mass — the shift is real but small, mostly a visual/
  postural compensation rather than a large physical correction).

## COM trajectory during motion

- **Walking:** COM rises and falls sinusoidally, twice per stride (peaks at
  each midstance) — small amplitude (~2–4cm).
- **Running:** COM falls during stance (leg compresses), rises during flight —
  same 2-per-stride pattern but driven by the spring-mass mechanism rather than
  pendulum vaulting.
- **Jumping:** COM follows a pure ballistic parabola from the instant of
  takeoff — no muscular effort in the air can alter this trajectory. Only limb
  positions relative to the COM can change (tucking, arching).

## COM and rotational moves

Spins and turns conserve angular momentum about the COM (absent outside
torque). Tucking limbs closer to the rotation axis speeds up the spin (same
principle as a figure skater); this is why a spin move naturally has the ball
pulled tight to the body — it's not just protection, it's what keeps the
rotation controllable.

## Game application (Cyber Hoops)

- Confirms the current rig's Pelvis-as-root design is not an arbitrary
  convention — it's the biomechanically correct choice, and it's why
  AnimationController computes bob/lean/crouch on the Pelvis node specifically.
- The "2 bobs per stride, peaks at midstance" fact already validated the bob-
  frequency fix from the biomechanics-expert/animator review (previously 4 per
  stride, now 2 — matches this section exactly).
- Ballistic COM in the air = the exact justification already used for the
  dunk's flattened-sine arc (arcade exaggeration of a real parabola, not a
  contradiction of one) — the persona reviews already confirmed this specific
  point as an accepted "cheat," not a bug.
- Ball-carried-COM shift is small enough to safely ignore in the rig (no need
  to counter-lean for holding a 0.62kg ball) — matches current implementation.
