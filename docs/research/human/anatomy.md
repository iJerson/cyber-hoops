# Human Anatomy — Proportions Reference

## Standard proportions (average adult, fractions of total height H)

| Segment | Length |
|---|---|
| Head | 1/7.5 H |
| Head + neck to chin | 1/7 H |
| Shoulder width | ~0.25 H |
| Hip width | ~0.19 H |
| Arm span (fingertip to fingertip) | ≈ H |
| Upper arm | ~0.19 H |
| Forearm | ~0.16 H |
| Hand | ~0.11 H |
| Torso (shoulder to hip) | ~0.30 H |
| Thigh | ~0.245 H |
| Shin | ~0.246 H |
| Foot length | ~0.15 H |

Navel/midpoint ≈ 0.5–0.53 H from the floor — the body's rough center of mass
height while standing.

## Athlete variance

Basketball players skew tall with disproportionately long limbs (wingspan often
exceeds height by 3–8%, "positive ape index") — arm span/H can run 1.03–1.08
instead of 1.00. Guards trend closer to average proportions; centers trend
longer-limbed.

## Segment mass fractions (% of total body mass, standard biomechanical tables)

| Segment | % mass |
|---|---|
| Head + neck | 8.1% |
| Trunk | 49.7% |
| Upper arm (each) | 2.8% |
| Forearm (each) | 1.6% |
| Hand (each) | 0.6% |
| Thigh (each) | 10.0% |
| Shank/shin (each) | 4.65% |
| Foot (each) | 1.45% |

These mass fractions matter for center-of-mass math (see center_of_mass.md) —
the trunk alone is nearly half the body's mass, which is why torso lean
dominates balance far more than arm position.

## Game application (Cyber Hoops)

- Current rig (game/Presentation/Characters/CharacterRig.tscn) proportions:
  verify thigh/shin/torso ratios against the table above when resizing.
- Wingspan exaggeration is a free "looks athletic" lever — arms slightly longer
  than the anatomical 1.0×H default reads as more basketball-shaped.
- Segment mass fractions aren't simulated (no ragdoll/IK solver yet) but should
  inform any future center-of-mass-driven balance system — the trunk's ~50%
  mass share is why AnimationController correctly drives lean/crouch from the
  pelvis rather than distributing balance logic across limbs.
