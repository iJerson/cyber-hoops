# Human Research Library — Index

General human movement science underlying the basketball-specific research in
[../basketball/](../basketball/). Where basketball/ covers "how a player does X",
human/ covers "why the body can/must move that way at all."

| Document | Content |
|---|---|
| [anatomy.md](anatomy.md) | Proportions, segment lengths, mass fractions |
| [skeleton.md](skeleton.md) | Joint types, ranges of motion, kinetic chain order |
| [biomechanics.md](biomechanics.md) | Newton's laws applied to the body, stretch-shortening cycle, kinetic chain |
| [locomotion.md](locomotion.md) | Walk vs run mechanics, gait transition, turning |
| [gait.md](gait.md) | Detailed phase-by-phase cycle, cadence, arm-leg coupling |
| [balance.md](balance.md) | Base of support, recovery strategies, anticipatory adjustments |
| [center_of_mass.md](center_of_mass.md) | COM location, shifts with posture, ballistic flight, rotation |
| [momentum.md](momentum.md) | Linear/angular momentum, follow-through, explosive-start lean |
| [muscle_groups.md](muscle_groups.md) | Which muscles drive which visible motions |

## How this maps to the persona system

Each file feeds one or more review personas in `.claude/agents/`:
- **anatomist** ← anatomy.md, skeleton.md
- **biomechanics-expert** ← biomechanics.md, balance.md, center_of_mass.md, momentum.md
- **animator** ← gait.md, locomotion.md, muscle_groups.md (for plausible secondary motion)

## Key facts already validated against the implementation

- COM ≈ pelvis height (55% of standing height) → confirms Pelvis-as-rig-root design.
- 2 gait bobs per stride, peaks at midstance → confirmed the bob-frequency bug
  fix from the first persona review round (was 4 per stride, now 2).
- Arms lag legs slightly, not exact antiphase → confirmed the ArmPhaseLag fix.
- Knee flexion is one-directional (0 to ~2.6 rad) → confirmed in rig joint limits.
- Ballistic COM in flight (dunk) → confirmed the flattened-hang exponent is an
  arcade exaggeration of real physics, not a contradiction of it.

See [animation_notes.md](../basketball/animation_notes.md) for the basketball-side
research-to-backlog map.
