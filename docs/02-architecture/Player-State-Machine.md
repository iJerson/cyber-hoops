# Player State Machine

Concrete state design for `CharacterStateMachine`
(see [Animation-Architecture.md](Animation-Architecture.md)). Drives both the human
player and the AI — the machine reads intents from `IMovementInputSource` and world
facts; it does not care who produces them.

## Inputs (guards read every tick)

| Symbol | Source | Meaning |
|---|---|---|
| `speed` | MovementComponent | horizontal speed, m/s |
| `sprint` | input source | sprint held |
| `hasBall` | PossessionComponent | possession |
| `defenderNear` | proximity probe | opponent within ProtectRadius (1.5m in / 2.0m out hysteresis) |
| `ballDist` | rim probe | ground distance to rim |
| `shootPressed` / `passPressed` / `defendPressed` | input source | action edges |
| `ballInFlightNear` | ball probe | free ball airborne within BlockRange, above BlockMinHeight |
| `opponentDribbling` | ball probe | opponent has live dribble within StealRange |
| `airborne` | body | not on floor |
| `t` | machine | seconds in current state |

Thresholds (all from resources, values here per current tuning): `WalkMax = 0.1`,
`RunMax = speed ≤ MoveSpeed (5.0)`, sprint above; `DunkRange = 2.6`, `LayupRange = 4.5`.

## States

### Locomotion (no ball)
| State | Enter pose (spec ref) | Notes |
|---|---|---|
| **Idle** | A | loaded stance, breathing |
| **Walk** | C minus ball layer | 0.1 < speed ≤ walk/run blend, gait swing |
| **Run** | D minus ball layer | speed above walk, no sprint |
| **Sprint** | D full lean | sprint held and moving |

### Locomotion (ball)
| State | Pose | Notes |
|---|---|---|
| **DribbleIdle** *(implied by user's set; folded into Dribble Walk at speed 0 if preferred — kept explicit here)* | B | bounce free-runs, arcade tempo |
| **DribbleWalk** | C | bounce locked to gait |
| **DribbleRun** | D | includes sprint-with-ball; push-ahead bounce every 2–3 strides |
| **ProtectBall** | E | overrides Dribble* while defenderNear |

### Actions (ball)
| State | Pose | Duration |
|---|---|---|
| **Shoot** | I | dip 60ms + release; ball leaves on gameplay tick |
| **Layup** | J | moving finish, inside-leg drive |
| **Dunk** | K | non-interruptible flight (the one exception) |
| **Pass** | arm snap toward target | 120ms; mechanic reserved for future teammates — state specced now so adding 2v2 later doesn't reshape the machine |

### Actions (no ball)
| State | Pose | Duration |
|---|---|---|
| **Steal** | L | 80ms lunge + contact window |
| **Block** | M | dip + jump + airborne until Landing |

### Terminal/recovery
| State | Pose | Duration |
|---|---|---|
| **Landing** | knees absorb, squash | 100ms (150ms after Dunk) |
| **Recover** | return blend to context stance | 150ms; universal exit funnel after actions |

## Transition table

Priority top-down within each state; first true condition wins. `→Recover(X)` means
Recover remembers a return context X.

### From locomotion (no ball)
| From | Condition | To |
|---|---|---|
| Idle/Walk/Run/Sprint | pickup (ball enters possession) | DribbleIdle/Walk/Run (match speed) |
| Idle/Walk/Run/Sprint | `defendPressed && opponentDribbling` | Steal |
| Idle/Walk/Run/Sprint | `defendPressed && ballInFlightNear` | Block |
| Idle | `speed > 0.1` | Walk |
| Walk | `speed ≤ 0.1` | Idle |
| Walk | `speed > WalkRunBlend` | Run |
| Run | `sprint && speed > RunMax·0.8` | Sprint |
| Run | `speed ≤ WalkRunBlend` | Walk |
| Sprint | `!sprint` | Run |

### From locomotion (ball)
| From | Condition | To |
|---|---|---|
| any Dribble*/Protect | ball lost (steal/knock) | Recover(no-ball locomotion) |
| any Dribble* | `defenderNear` (1.5m) | ProtectBall |
| ProtectBall | `!defenderNear` (2.0m out) | Dribble* by speed |
| any Dribble*/Protect | `shootPressed && ballDist ≤ DunkRange` | Dunk |
| any Dribble*/Protect | `shootPressed && ballDist ≤ LayupRange && speed > 0.5` | Layup |
| any Dribble*/Protect | `shootPressed` (else) | Shoot |
| any Dribble*/Protect | `passPressed && teammateExists` | Pass |
| DribbleIdle | `speed > 0.1` | DribbleWalk |
| DribbleWalk | `speed ≤ 0.1` | DribbleIdle |
| DribbleWalk | `speed > WalkRunBlend` | DribbleRun |
| DribbleRun | `speed ≤ WalkRunBlend` | DribbleWalk |

Notes: Sprint-with-ball is DribbleRun with `NormalizedSpeed` driving lean — no
separate state, amplitude is continuous. Explosive-start and Stop from the spec are
*entry/exit ramps* of DribbleRun/DribbleIdle (sub-phases), not machine states —
they cannot be interrupted meaningfully and adding them as states doubles the table.

### From actions
| From | Condition | To |
|---|---|---|
| Shoot | ball released (gameplay tick) + 60ms follow-through | Recover |
| Layup | ball released + landed | Landing |
| Dunk | slam completed (flight end) | Landing |
| Pass | ball released + 120ms | Recover |
| Steal | contact resolved or `t > 0.15` | Recover |
| Block | `airborne == false` | Landing |
| Landing | `t > duration` | Recover |
| Recover | `t > 0.15` | context: hasBall ? Dribble* by speed : locomotion by speed |

### Global interrupts (any state except Dunk flight)
| Condition | To |
|---|---|
| match reset / made basket teleport | Idle (hard set, no blend) |
| knocked ball while in Dribble*/Protect | Recover |

## Rules

1. **Dunk flight is the only non-interruptible window.** Everything else yields to
   its transition table every tick.
2. **Actions resolve on the gameplay tick.** Shoot/Pass release, Steal contact,
   Block deflection happen when gameplay says so; the state supplies presentation
   timing only (responsiveness contract).
3. **Recover is the single exit funnel** for all actions — one blend path to tune
   instead of N², and it re-derives the correct locomotion state from live guards.
4. **Hysteresis** on ProtectBall (1.5/2.0m) and on Walk/Run blend boundaries
   (±10%) to prevent state flicker.
5. **Speed states are thin:** Walk/Run/Sprint (and Dribble variants) share one
   pose function parameterized by `NormalizedSpeed`; states exist for the
   transition table and event hooks, not for separate poses.
6. Engine-free core: transition function takes a guard snapshot struct, returns a
   state name — unit-tested like `AIDecider`; the Godot node wraps it.

## Diagram (condensed)

```
             pickup                    lost ball
  [Idle⇄Walk⇄Run⇄Sprint] ─────→ [DribbleIdle⇄Walk⇄Run] ⇄ [ProtectBall]
        │        ▲                    │ shoot/pass by range/target
        │defend  │Recover             ▼
        ▼        │            [Shoot|Layup|Dunk|Pass]
  [Steal|Block] ─┤                    │
        │        │                    ▼
        └──→ [Landing] ──────────→ [Recover] ──→ (re-derive locomotion)
```
