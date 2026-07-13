# Defense Mechanics

## Stance

- Lower than the attacker: knees 40°+, hips back, chest up, arms wide and active.
  The lower player wins the first step.
- Weight on balls of feet, never heels; feet wider than shoulders.
- One hand traces the ball, the other denies the pass lane / mirrors.

## Positioning

- Always between man and basket ("ball–you–basket" line), off-arm distance:
  close enough to touch, far enough not to be beaten by the first step.
- Angle the stance to funnel the ballhandler toward the sideline/help — never
  square-on (square = both directions open).
- Eyes on the attacker's hips/chest, not the ball or eyes — hips can't fake.

## Slides

- Step-slide: lead foot steps, trail foot pushes and closes to (not past)
  shoulder width. Feet never cross while the man still has the dribble.
- Crossover run only when beaten — sprint to reposition, then re-slide.

## Steals

- High-percentage steals are **on the bounce**: poke at the ball's rise, hand
  moves up-to-down or side-swipe, body stays balanced (no lunge unless it ends
  the play). Reaching while off-balance = blow-by.
- Timing beats speed: the window is floor-contact to hand-contact of the dribble.
- Dig from the top of the ball's rise; slap down, never up (fouls).

## Blocks

- Verticality: jump straight up, arm fully extended, wrist firm — meet the ball
  at its release rise or apex.
- Block *after* the shooter commits (ball leaving the hand), off two feet when
  possible.
- Target: ball's upper half, redirect — best blocks keep the ball in play.

## Closeouts

- Sprint 2/3 of the distance, chop steps the last 1/3, high hand up, weight back
  to absorb a drive. Full-speed arrival = flyby.

## Game application (Cyber Hoops)

- AI defend position already implements ball–you–basket. Add funnel angle later.
- Steal window mechanic: our cooldown model approximates it; a per-bounce-phase
  window (steal only succeeds near the ball's floor contact) would add real timing
  skill for both sides.
- Block verticality: our block deflects radially; upward-biased deflection reads
  more real and keeps play alive.
- Defensive stance = ProtectBall's mirror; reuse the low-stance pose with arms
  wide for the AI when guarding.
