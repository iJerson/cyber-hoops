# Animation Specification

Derived from [Dribble-Biomechanics.md](Dribble-Biomechanics.md) and
[Elite-Movement-Patterns.md](Elite-Movement-Patterns.md). Specification only — no
implementation here.

## Conventions

- Rig faces **-Z**. All angles in radians unless noted.
- **Pelvis pitch:** negative = forward lean. **Shoulder pitch:** positive = hanging arm swings forward.
- `gaitPhase` ∈ [0,1): one full stride cycle (left step at 0.0, right step at 0.5).
- `bouncePhase` ∈ [0,1): dribble cycle, floor contact at 0.0/1.0, apex at 0.5.
- **Master sync rule:** while dribbling, `bouncePhase` is *derived from* `gaitPhase` (not independent): floor contact aligns with the plant of the foot opposite the dribbling hand. At idle, `bouncePhase` free-runs at the gravity-derived period.
- All magnitudes are targets at full expression; every pose blends in/out over 80–150ms (see Transitions).

---

## A. Idle (no ball)

| Property | Spec |
|---|---|
| Body lean | ~0; upright but loaded |
| Pelvis rotation | pitch -0.05 (slight hinge); yaw 0 |
| Spine angle | neutral, +0.03 counter-pitch vs pelvis (chest up) |
| Shoulder rotation | breathing sway ±0.02 @ 0.35Hz |
| Arm position | hanging, elbows soft ~0.15 bend |
| Hand position | at sides, slightly forward of hip line |
| Head direction | level, toward current focus (ball if free, else hoop) |
| Ball position | n/a |
| Foot placement | shoulder width, weight mid-foot, knees 0.15–0.2 flexed |

## B. Idle dribble

| Property | Spec |
|---|---|
| Body lean | forward 0.10–0.14 |
| Pelvis rotation | pitch -0.12; yaw +0.05 toward ball side |
| Spine angle | follows pelvis, head end returns to vertical |
| Shoulder rotation | ball-side shoulder dropped 0.05, yaw follows pelvis |
| Arm position | ball arm: pitch 0.35 → 0.90 tracking bounce; elbow leads; free arm half-raised bar (pitch 0.3, elbow 0.5 bend) |
| Hand position | ball hand palm-down above ball; *contacts at apex, rides down last third, pushes* — hand y = ballTop for bouncePhase ∈ [0.4, 0.75] |
| Head direction | level, at opponent/hoop, never at ball |
| Ball position | front-outside of dribble-side foot: local (±0.3, bounce, -0.45); height ≈ knee (0.55–0.7m apex) |
| Foot placement | staggered — ball-side foot half step back; width 1.1× shoulders; knees 0.25–0.35 flexed |

## C. Walk dribble

As B, plus:

| Property | Spec |
|---|---|
| Body lean | forward 0.08–0.12 |
| Pelvis rotation | yaw oscillates ±0.06 with stride, counter to shoulders |
| Shoulder rotation | counter-yaw ±0.06; opposition preserved |
| Arm position | free arm gait swing (±0.3); ball arm pumps as B |
| Ball position | beside front-outside of same-side foot, apex ≈ hip (0.7–0.9m) |
| Foot placement | normal walk stride; **bounce contact on opposite-foot plant** (bouncePhase 0 == gaitPhase of opposite foot strike) |
| Head direction | scanning: hoop-biased, ±0.2 yaw drifts every 1–2s |

## D. Run/sprint dribble

| Property | Spec |
|---|---|
| Body lean | forward 0.18–0.28 scaling with speed |
| Pelvis rotation | pitch -0.2; minimal yaw (travel-locked) |
| Spine angle | continues lean, small extension at head |
| Shoulder rotation | pumping ±0.5 via arms, torso yaw < ±0.04 |
| Arm position | free arm full sprint pump; ball arm extends forward on push (pitch up to 1.1), recovers between bounces to gait swing |
| Hand position | pushes ball forward-down ~45°, ahead of body |
| Ball position | thrown 1.0–1.5m ahead, apex waist–chest (0.9–1.2m); one bounce per 2–3 strides |
| Foot placement | sprint stride, on balls of feet, narrow base |
| Head direction | locked on target lane |

## E. Protect dribble (defender < ~1.5m)

| Property | Spec |
|---|---|
| Body lean | forward 0.10 plus lateral 0.08 into defender |
| Pelvis rotation | yaw 0.6–0.9 — body turned sideways, ball side away |
| Spine angle | slight twist: shoulders rotate 0.15 further than pelvis |
| Shoulder rotation | free shoulder points at defender |
| Arm position | free arm horizontal bar (pitch 0.7, elbow 0.9 bend, forearm across chest line); ball arm short pump |
| Hand position | ball hand on top-outside of ball |
| Head direction | over free shoulder, at defender |
| Ball position | far hip, slightly behind body plane, apex ≈ knee (0.4–0.5m), tempo 1.5× |
| Foot placement | wide (1.4× shoulders), deep 0.4 knee flex, weight on defender-side leg |

## F. Direction change / crossover

Timeline (total ~0.35s): plant → hips → shoulders → ball → push-off.

| Property | Spec |
|---|---|
| Body lean | into new direction by end; brief 0.1 lean to fake side first |
| Pelvis rotation | snaps to new heading over 0.1s, *leading* shoulders |
| Spine angle | shoulder yaw lags pelvis 60–80ms |
| Shoulder rotation | dip 0.08 toward fake side, then whip |
| Arm position | ball arm sweeps across body low; receiving arm drops to catch |
| Hand position | rolls from top to outside of ball; pushes diagonal across |
| Head direction | stays on play; no head fake in the base move |
| Ball position | crosses midline at lowest, fastest bounce (apex < 0.4m), lands front-outside of new-side foot |
| Foot placement | outside foot plants wide (brake), push-off from that leg |

## G. Explosive start (attack first step)

| Property | Spec |
|---|---|
| Pre-load | 100–150ms: pelvis drops 0.06m, knees +0.15 flex, lean +0.05 — MANDATORY before the burst |
| Body lean | jumps to 0.25 on step 1 |
| Pelvis rotation | pitch -0.22, drives forward |
| Spine/shoulders | sprint posture immediately |
| Arm position | free arm rips through; ball arm long push |
| Hand position | behind-top of ball, long forward push |
| Ball position | first bounce thrown 1.5m+ ahead, apex hip+ |
| Foot placement | back leg full extension, first two steps long |
| Head direction | at attack lane |

## H. Stop (decelerate to idle dribble)

| Property | Spec |
|---|---|
| Timeline | penultimate step long (brake) → final step short (stabilize), ~0.3s |
| Body lean | decays from run lean to 0.1; torso stays over feet, never behind |
| Pelvis rotation | pitch eases to -0.12; sinks 0.05m through the two steps |
| Shoulder rotation | gait pump decays to zero with speed |
| Arm/hand | ball arm shortens pump as bounce height decays |
| Ball position | bounce apex decays with speed (1.0 → 0.6m over the stop); ends at idle-dribble spot |
| Foot placement | brake foot ahead of COG, stabilizer under COG, ends staggered as B |
| Head direction | level throughout |

## I. Jump shot

| Property | Spec |
|---|---|
| Pre-load | dip: pelvis -0.07m, 120ms |
| Body lean | vertical at release (0), slight back ≤0.05 allowed |
| Pelvis rotation | squares to hoop (yaw → 0 relative to hoop line) |
| Spine angle | extends through jump |
| Shoulder rotation | both arms rise; shooting shoulder pitch 2.6–2.9 |
| Arm position | shooting arm: elbow under ball, extends to full; off arm guides then peels at release |
| Hand position | ball on pads, wrist cocked, snaps at apex |
| Head direction | at rim from gather to release |
| Ball position | gather at hip → set point above forehead → release above head at jump apex |
| Foot placement | squared, shoulder width; lands same spot |

## J. Layup

As I, but moving:

| Property | Spec |
|---|---|
| Body lean | keeps travel lean into gather, verticalizes in flight |
| Gather | two-step rhythm: outside foot, inside foot, jump off inside leg |
| Arm position | ball arm extends high toward rim, one-handed finish; knee on ball side drives up |
| Ball position | gather at waist → carried up the chest → release at full extension |
| Head direction | at rim/backboard target |

## K. Dunk

| Property | Spec |
|---|---|
| Pre-load | mandatory dip 120ms before lunge (see G) |
| Body lean | forward 0.2 during approach, arches slightly back mid-flight (+0.08), snaps forward on slam |
| Pelvis rotation | pitch follows lean; no yaw |
| Shoulder rotation | both arms rise to overhead (pitch 2.7) during first half of flight |
| Arm position | two-hand overhead carry; elbows extend at rim; arms recoil after slam |
| Hand position | both hands on ball sides, overhead |
| Head direction | locked on rim |
| Ball position | overhead anchor (~2.3m) through flight, slammed down through rim |
| Foot placement | takeoff both/inside leg, lands just in front of rim, knees absorb (+0.2 flex on land) |

## L. Steal attempt

| Property | Spec |
|---|---|
| Body lean | lunge toward ball 0.2, one frame of full reach |
| Pelvis rotation | yaw toward ball, drop 0.08m |
| Arm position | stealing arm snaps to full extension low (pitch 0.9, downward line to ball), other arm counterbalances back |
| Hand position | palm up/sideways flick at ball |
| Foot placement | lead foot slides toward ball, wide base kept |
| Head direction | at ball (exception to eyes-up: reactive reach) |
| Recovery | return to E-stance over 150ms |

## M. Block attempt

| Property | Spec |
|---|---|
| Pre-load | dip 100ms |
| Body lean | vertical jump, lean ≤0.05 toward ball |
| Shoulder rotation | blocking arm pitch to full overhead 2.9, other arm half-raised |
| Arm/hand | full extension, fingers spread, wrist firm |
| Head direction | tracking ball |
| Foot placement | jumps from wide base, lands same spot, knees absorb |

---

## Transition rules

| From → To | Trigger | Blend time | Notes |
|---|---|---|---|
| A ↔ B | pickup / release | 120ms | crouch + arm pose ease in |
| B ↔ C ↔ D | speed thresholds (0.1 / 4.0 m/s) | continuous | all gait properties scale with `speed/RunSpeed`; never a discrete pop |
| C/D → F | heading change > 60° while dribbling | immediate | sequencing fixed: plant(0ms) → pelvis(0–100ms) → shoulders(60–160ms) → ball cross(100–250ms) → push(250–350ms) |
| B/E → G | sprint input while dribbling | pre-load first | dip is not skippable; 100–150ms |
| D → H | input released / arrive | 300ms two-step | bounce apex decays in lockstep with speed |
| B ↔ E | defender distance < 1.5m (enter), > 2.0m (exit) | 200ms | hysteresis prevents flicker |
| B/C/E → I/J/K | shoot action by range | gather 100ms | I: stationary or slow; J: moving, mid range; K: dunk range |
| any → L | defend action, ball held by opponent | 80ms in | recovery 150ms back to E |
| any → M | defend action, ball in flight | pre-load 100ms | falls back to A on land |
| I/J/K → A | ball released + land | 150ms | arms lower, lean resets |

Global rules:

1. **No pose is entered instantly** except F's plant and L's snap; everything else blends 80–300ms.
2. **Pelvis leads:** in any transition involving rotation, pelvis reaches target before shoulders (50–100ms lag), head is either first (spins, vision) or last (everything else).
3. **Dip precedence:** G, I, K, M must play the pre-load dip; queue the action, don't skip the dip.
4. **Sync invariant:** while moving with ball, bounce contact = opposite-foot plant. On any transition, re-derive `bouncePhase` from `gaitPhase`, never let them drift.
5. **Decay invariant:** bounce apex height is a function of current speed and defender proximity — it may never change discontinuously.
6. **Head invariant:** head pitch/yaw driven by focus target only; body transitions never drag the head with them (except spin: head leads).

---

## Arcade adjustments

Cyber Hoops is arcade, not simulation. The spec above is the *believability floor*;
this section overrides it wherever realism and fun conflict. Governing rule:
**realism in the rhythm, exaggeration in the amplitude, arcade in the timing.**

### Keep at realistic values (these SELL the motion; cost nothing in responsiveness)

- Opposition and bounce↔step sync (global rule 4) — the brain checks rhythm, not magnitude.
- Pelvis-leads ordering (global rule 2).
- Hand rides the ball at contact (B) — reads at a glance.
- Head level + eyes on play (rule 6).
- Continuous decay of bounce height (rule 5).

### Cut or compress (realism that fights responsiveness)

| Spec item | Sim value | Arcade override |
|---|---|---|
| Pre-load dips (G, I, K, M) | 100–150ms mandatory | 50–70ms, and motion starts DURING the dip — never delay input response; the dip is read as anticipation, not felt as lag |
| Blend times | 80–300ms | halve everything: 40–150ms; input latency reads as "heavy" |
| Stop pattern (H) | two-step 300ms | one hard 120ms stop with deep knee sink; exaggerate the sink to explain the impossible physics |
| Crossover sequence (F) | 350ms full chain | 200ms; keep the ORDER, compress the gaps |
| Layup two-step gather (J) | strict footwork | ignore footwork legality entirely; keep only the inside-leg knee drive because it looks athletic |

### Exaggerate beyond realism (fun multipliers)

| Item | Realistic | Arcade target | Why |
|---|---|---|---|
| Sprint lean (D) | 0.18–0.28 | 0.35–0.4 + speed lines feel | cartoon speed read; pairs with FOV/camera pull |
| Explosive first step (G) | ball 1.5m ahead | 2.5m+, body stretches, brief motion blur/trail | burst must feel superhuman |
| Dunk flight (K) | jump to rim | +50% hang time at apex, deeper back-arch (+0.15), slower rise / snap-fast slam | anticipation → payoff curve; the slam frame is the poster |
| Dunk landing | knees absorb 0.2 | full crouch (0.35) + rim shake + already-added burst | impact sells power |
| Protect stance (E) | pelvis yaw 0.6–0.9 | full 1.1 dramatic shield, free arm fully horizontal | readability at game camera distance |
| Steal lunge (L) | short reach | lunge covers 0.5m, body stretches like fencing touch | telegraphs the mechanic to both players |
| Crossover ball (F) | apex <0.4m | nearly floor-scraping snap with audible tick | crunchy game-feel moment |
| Idle dribble tempo | gravity period | 15% faster than physics | energy at rest; games feel dead at true tempo |
| Jump shot apex hold (I) | none | 60ms freeze at release | shot timing readability, screenshot frame |

### Squash and stretch (pure arcade, no sim basis)

- Ball: 15% vertical squash at every floor contact, 8% stretch at peak fall speed.
- Rig: 5% torso stretch during dunk rise, 8% squash on dunk land and hard stop.
- Never on: idle, walk — reserve deformation for high-energy moments so it stays special.

### Responsiveness contract (never violate)

1. Input → visible reaction ≤ 50ms (one dip frame allowed, movement starts under it).
2. No animation may delay a gameplay effect: ball release, steal window, movement change happen on the gameplay tick; animation catches up.
3. Player-controlled character never plays a non-interruptible animation longer than 200ms (dunk flight is the single exception — it is the reward).
