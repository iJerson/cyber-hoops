# Muscle Groups — Function Reference

Purpose: which muscles drive which visible motions, so pose/animation choices can
be checked against what's actually capable of producing them.

## Lower body

- **Glutes (gluteus maximus/medius):** hip extension (driving forward/upward in
  a jump or stride push-off) and hip stabilization (medius keeps the pelvis
  level on a single stance leg — its weakness is the classic cause of a
  visible pelvic "drop" on one side during a stride, a real pathological gait
  tell, useful to know so it's never accidentally animated as normal).
- **Quadriceps:** knee extension — straightening the leg, dominant in jump
  takeoff and landing absorption (eccentric control on the way down).
- **Hamstrings:** hip extension + knee flexion — decelerate the swinging leg
  before foot strike, and drive the leg back during sprinting (posterior
  chain, not the quads, is what makes elite sprinters fast).
- **Calves (gastrocnemius/soleus):** ankle plantarflexion — the final push-off
  snap at terminal stance, and the "spring" absorption on landing.
- **Hip flexors (iliopsoas):** lift the thigh forward/up — drives the swing leg
  forward and the knee-drive in a sprint or jump.
- **Adductors/abductors:** side-to-side leg control — lateral shuffles,
  defensive slides, cutting stability.

## Core / trunk

- **Rectus abdominis + obliques:** trunk flexion and rotation — the "crunch"
  motion in a dunk slam, and rotational power transfer in a shot or pass.
- **Erector spinae:** trunk extension — keeps the back from folding forward
  under load, resists the forward lean, controls the arch in a dunk.
- **Core as a unit:** primarily stabilizes the spine so limb muscles have a
  solid base to pull against — a weak/uncontrolled core shows up as excess
  torso wobble during otherwise-normal limb motion.

## Upper body

- **Deltoids:** shoulder flexion/abduction — raising the arm in any direction,
  dominant in the overhead reach (dunk, block, shot).
- **Rotator cuff:** stabilizes the shoulder ball-and-socket joint through its
  huge range of motion — not usually visible in gross motion but is why the
  shoulder can move to extreme angles without the joint "popping" visually.
- **Triceps:** elbow extension — the final push/snap in a shot release or a
  block's arm extension.
- **Biceps + forearm flexors:** elbow flexion and grip — controlling/gathering
  the ball, cushioning a catch.
- **Trapezius/rhomboids:** shoulder blade control — subtle, mostly relevant to
  overhead reach ceiling (full extension needs the shoulder blade to rotate
  too, part of why a genuine max-reach dunk pose involves slight upper-back
  motion, not shoulder-joint rotation alone).

## Which muscles explain which animation beats

| Visible motion | Primary driver |
|---|---|
| Explosive first step | Glutes + hip flexors + calves |
| Jump takeoff | Glutes + quads + calves (triple extension) |
| Landing absorption | Quads (eccentric) + calves |
| Dunk slam / crunch | Abs + shoulder/triceps |
| Shot release snap | Triceps + wrist flexors, powered up-chain by legs/core |
| Defensive slide | Adductors/abductors + glute medius |
| Protect-dribble shield arm | Deltoid (held position) — static, low-effort hold |

## Game application (Cyber Hoops)

- No muscle simulation exists or is needed (procedural joint-angle animation,
  not a musculoskeletal model) — this reference exists to sanity-check that a
  proposed pose has a plausible muscular driver, not to implement one.
- The dunk "crunch" already correctly uses a forward pelvis/torso motion for
  the slam — matches the abs-driven crunch description above.
- Landing absorption (added in the persona-review fix pass) matches the
  quad-eccentric-control model — knee flexion on touchdown is exactly what
  that muscle group produces, confirming the fix direction was physiologically
  correct, not just "looks better."
- Glute medius pelvic drop is worth remembering as a NEGATIVE example — if any
  future single-leg stance pose (e.g. a one-foot landing) shows a hip-drop, it
  should be treated as an intentional "off-balance" tell, not left in as an
  accidental default for a normal stance.
