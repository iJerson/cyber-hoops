# Animation Notes — Research Index

Bridge between this research library and the implementation specs.

## Where things live

| Document | Content |
|---|---|
| [biomechanics.md](biomechanics.md) | 12 universal elite movement patterns (the invariants) |
| [dribbling.md](dribbling.md) | full dribble biomechanics by situation |
| [../../01-game-design/Animation-Spec.md](../../01-game-design/Animation-Spec.md) | pose-by-pose spec (9 properties × 13 states) + arcade overrides |
| [../../02-architecture/Animation-Architecture.md](../../02-architecture/Animation-Architecture.md) | component design (7 components, data flow, migration order) |
| [../../02-architecture/Player-State-Machine.md](../../02-architecture/Player-State-Machine.md) | 16 states + transition table |
| [../../02-architecture/Animation-Blend-Tree.md](../../02-architecture/Animation-Blend-Tree.md) | 5 layers, blend vs interrupt rules |

## The five rules that carry everything

1. **Rhythm over magnitude:** the eye validates timing (bounce↔step sync,
   opposition) before it validates pose accuracy. Get phase right first.
2. **Pelvis leads, limbs lag, head is independent.**
3. **Every explosive action pre-loads** (dip) — arcade-compressed to 50–70ms with
   motion starting under it.
4. **Continuity invariants:** bounce height, gait amplitude, lean all vary
   continuously with speed/pressure — nothing steps.
5. **Arcade formula:** realism in the rhythm, exaggeration in the amplitude,
   arcade in the timing. Gameplay never waits for animation.

## Research-to-backlog map

| Research insight | Feature | Status |
|---|---|---|
| bounce = opposite-foot plant | GaitPhase sync | specced, not built |
| catch-not-slap hand contact | hand rides ball top third | specced, not built |
| pre-load dip | dunk/shot/block anticipation | specced, not built |
| gather 1-2 step | layup/dunk takeoff | research only |
| bank shots (layups.md) | backboard-target solver | research only |
| steal on the bounce (defense.md) | phase-windowed steal | research only |
| stamina (fatigue.md) | turbo meter + gates | research only |
| rebound prediction (rebounding.md) | AI landing-spot solve | research only |
| squash & stretch (ball_physics.md) | BallVisual | specced, not built |
