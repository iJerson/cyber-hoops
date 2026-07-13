# Ball Physics

## Regulation values

- Size 7 ball: mass 0.60–0.65kg, circumference 74.9–78cm → radius ≈ 0.119–0.124m.
- Inflation spec: dropped from 1.8m, must rebound to 1.2–1.4m → coefficient of
  restitution (COR) ≈ 0.81–0.87 on hardwood.
- Rim: 45.7cm inner diameter (2.4× ball diameter — two balls nearly fit), height
  3.05m. Backboard inner square: 59×45cm, bottom at 3.05m... (bottom edge 2.9m).

## Bounce behavior

- COR drops with impact speed (felt as "dead" hard slams) and varies with surface.
- Spin converts on bounce: backspin kills horizontal velocity (shots die on the
  rim), topspin accelerates the bounce forward (bounce passes), sidespin kicks
  laterally. Friction with floor converts ~spin ↔ velocity every contact.
- Rolling resistance is low: loose balls roll far on hardwood.

## Shot interactions

- Backspin (2–3 rev/s) on every shot: on rim/board contact it reduces the
  tangential rebound — "soft touch" is physics, not luck.
- Bank shots: 45° approach to backboard, aim upper corner of the square; board
  contact deadens the ball into a drop.
- Rim hits: front-rim contact with high arc pops up and often still drops ("rim
  roll"); flat front-rim hits fire back at the shooter.

## Dribble physics

- Push, not slap: hand applies force through ~1/3 of ball travel; ball leaves at
  4–8 m/s downward, returns losing ~15–20% energy per bounce.
- Control dribble ≈ 1.2–2 bounces/s; speed dribble bounce travels forward with the
  player (horizontal velocity imparted at push).

## Cyber Hoops current values (arcade-tuned)

| Param | Real | Ours | Note |
|---|---|---|---|
| Radius | 0.12m | 0.12 | match |
| Mass | 0.62kg | 0.62 | match |
| Bounciness | 0.81–0.87 | 0.8 | close |
| Dribble | physics-driven | deterministic parabola | intentional: arcade control |
| Backspin | 2–3 rev/s | none | future: visual + rim damping |

Deterministic dribble cycle stays — it is the control feel. Real-physics candidates:
backspin visual, spin-aware rim rebound damping, dead-ball COR falloff on hard slams.
