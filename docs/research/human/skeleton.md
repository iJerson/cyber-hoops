# Skeleton — Joint Structure and Limits

## Relevant joints, bottom-up

- **Ankle:** hinge-dominant (dorsiflex/plantarflex ~20°/50°) + limited inversion/
  eversion. Absorbs first on landing.
- **Knee:** hinge, flexion only 0°–150° (≈2.6 rad), zero hyperextension in a
  healthy joint. Never bends backward.
- **Hip:** ball-and-socket, largest range of any joint — flexion ~120° (2.1 rad),
  extension ~20–30° (-0.4 rad), plus ab/adduction and rotation. This is why hips
  can lead almost any movement (see biomechanics.md).
- **Spine (lumbar+thoracic, treated as one bendable segment for animation):**
  flexion ~90° total forward, ~30° back, ~30° each side, ~90° total rotation
  across the whole spine (not one joint).
- **Shoulder:** ball-and-socket, most mobile joint in the body — flexion to
  ~180° overhead, extension ~60° behind, plus rotation. Nearly the full circle
  is reachable, which is why overhead poses (dunk, block, shot) are unrestricted
  compared to hip/knee.
- **Elbow:** hinge, flexion only 0°–150°, zero hyperextension.
- **Wrist:** flex/extend ~70°/80°, ulnar/radial deviation ~35°/20°.
- **Neck:** flexion/extension ~45°/55°, rotation ~80° each way, lateral ~45°.

## Kinetic chain order

Skeleton links in a fixed chain: foot → ankle → shin → knee → thigh → hip →
pelvis → spine → shoulder → upper arm → elbow → forearm → wrist → hand. Force
and motion propagate along this chain — you cannot rotate the shoulder without
some reaction at the spine unless the chain is deliberately decoupled (e.g.
countering with the opposite arm).

## What breaks realism fastest (joint-limit violations)

1. Knee or elbow bending backward — the single most obvious "wrong" pose.
2. Shoulder pitch beyond ~+170° (past straight overhead, tipping backward past
   vertical) without a corresponding spine arch.
3. Hip yaw beyond ~90° without a foot pivot — hips don't twist independently of
   planted feet past a limited range.
4. Neck yaw beyond ~90° — reads as a head-spin, not a look.

## Game application (Cyber Hoops)

- Rig joint limit reference lives in .claude/agents/anatomist.md — mirrors this
  table in radians for direct code review.
- Current pivots (shoulder, hip, knee) map 1:1 to hinge/ball-socket joints
  above; knee is correctly flexion-only (never negative) per migration 7.
- Spine is currently a single pelvis rotation (no separate spine bend) —
  acceptable simplification for a low-poly rig; would matter more if torso
  twist independent of hip twist becomes a visible need (e.g. a pass-fake).
