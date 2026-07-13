# Elite Movement Patterns — Universal Mechanics

Companion to [dribbling.md](dribbling.md). This documents the
mechanics shared by virtually all elite players (NBA / FIBA / NCAA), as established
by slow-motion coaching analysis and sports-biomechanics literature. Signature and
style-specific moves are deliberately excluded. These are the invariants — the layer
a game animation system should encode as defaults.

## 1. The hips lead everything

Every elite action — start, stop, turn, shot, pass — begins at the hips, not the
feet or hands. Slow-motion shows the pelvis rotating or dropping 1–3 frames before
any limb visibly moves. Limbs express what the hips already decided.

Game rule: rotate/translate the pelvis first, let torso and limbs lag by ~50–100ms.

## 2. Constant knee flexion ("never tall")

Pros almost never stand at full leg extension while the ball is live. Baseline is
15–25° knee bend even when "standing", deepening to 40°+ under pressure or before
any explosive action. Full extension appears only mid-air and at relaxed dead balls.

Game rule: live-ball idle pose keeps visible knee bend; straight legs read amateur.

## 3. Pre-loading (the dip before everything)

Every explosive action is preceded by a small counter-movement in the opposite
direction: dip before jump, sink before sprint, small hop before landing into a
stop. Duration ~100–150ms. It is never skipped — elite players are faster *with*
the dip because of the stretch-shortening cycle.

Game rule: insert a 2–4 frame crouch before dunk lunge, sprint start, jump.

## 4. Wide base under load, narrow base in flight

Feet at shoulder width or wider whenever force is exchanged with the floor
(stopping, turning, shielding). Feet narrow only while gliding at speed. Base
width tracks the magnitude of horizontal force, almost linearly.

## 5. Opposition (contralateral coordination)

Right arm forward with left leg forward, always, in every gait. Also in the
dribble: the bounce of a right-hand dribble syncs with the *left* foot plant at a
walk. Breaking opposition is the single most common tell of bad animation.

## 6. Head stays level

Elite locomotion shows almost no vertical head bob (~2–4cm even at sprint) —
knees and ankles absorb what the gait generates. The head also stays near-level
through direction changes; only spins break this, and there the head *leads*
rotation to re-acquire vision early.

Game rule: damp head/torso bob to a fraction of pelvis bob; whip head first in spins.

## 7. Deceleration is a skill of its own

Pros stop in a stereotyped pattern: penultimate step long (the brake), final step
short (the stabilizer), hips sink through both. Torso stays forward over the feet;
leaning back to stop is a non-elite marker. Bounce height and gait amplitude decay
*together* through the stop.

## 8. The ball is caught, not slapped

At every dribble contact the hand meets the rising ball early and travels with it
before pushing back down (~1/3 of the rise). Contact time is long — the ball looks
briefly "held". Slap-style instant contact does not appear in elite footage.

Game rule: hand tracks ball position through the top third of the bounce.

## 9. Ball on the outside third

Live dribble stays outside the body's silhouette — beside or ahead of the
dribbling-side foot, never centered in front of the torso except during a cross.
The midline is crossed *quickly* (lowest, fastest bounce of any) and never dwelt in.

## 10. Eyes decoupled from mechanics

Gaze is on the play, not the ball, through every mechanic including crossovers and
behind-the-backs. Head orientation changes are driven by scanning (~2–3 gaze shifts
per second in half-court play), independent of what the body does.

Game rule: head look-at target = hoop/opponent, never the ball.

## 11. Rhythm variance, not constant tempo

Elite dribbling is *not* metronomic. Baseline tempo locks to gait, but pros insert
tempo breaks — a hold, a double-quick bounce — before attacking. Constant-frequency
dribble reads robotic. (For the game: small deterministic tempo shifts on state
transitions — attack start, defender proximity — rather than randomness.)

## 12. Arms as counterweights

Whichever arm is not handling the ball is doing balance work: extended slightly
during turns (like a tightrope pole), pumping in sprint, barring in protection.
It is never limp. Its position mirrors and stabilizes whatever the ball arm does.

## Priority mapping for Cyber Hoops rig

| Pattern | Current rig | Gap |
|---|---|---|
| Opposition + bounce↔step sync (#5) | gait swings, dribble separate | sync dribble phase to gait phase |
| Never tall (#2) | upright idle | add baseline knee/pelvis flexion |
| Pre-load dip (#3) | dunk starts instantly | 100ms crouch before lunge |
| Catch not slap (#8) | arm sine pump | hand follows ball top-third |
| Ball outside third (#9) | done (front-right anchor) | ok |
| Level head (#6) | pelvis bob moves all | damp upper body |
| Decel pattern (#7) | none | bounce/gait decay on stop |
| Counterweight arm (#12) | gait swing only | shield bar when defender near |
