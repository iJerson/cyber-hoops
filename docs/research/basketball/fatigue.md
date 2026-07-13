# Fatigue

## Physiology (what actually degrades)

- Basketball is repeated-sprint exercise: 1–4s bursts, incomplete recovery.
  Fatigue shows first in *explosiveness* (jump height, first step), then sprint
  speed, then shooting mechanics (legs die → arc flattens → short misses).
- Visible cues, in order of onset: hands on shorts/knees, slower recovery jog,
  reduced knee bend (standing taller — ironically the tired stance is the
  "amateur" stance), later and shorter jumps, head drops.
- Skill degradation: dribble gets higher and slower (less crouch), defensive
  slides become crossover runs, closeouts arrive flat-footed.

## Recovery

- Partial recovery in 20–60s of low activity (free throws, dead balls);
  repeated max efforts without recovery compound quickly.

## Fatigue in arcade games (design survey)

- NBA Jam: turbo meter — pure resource, no mechanical degradation. Simple, readable.
- Sim titles: hidden stamina degrading attributes — realistic, but opaque.
- Arcade sweet spot: **visible meter + few, exaggerated effects** (sprint gate,
  dunk gate) rather than many subtle nerfs players can't perceive.

## Game application (Cyber Hoops — future stamina system)

- Meter drains on sprint/dunk/steal-spam, refills when walking/idle (20–60s beat
  matches real recovery feel).
- Effects, arcade-clean: empty meter → no sprint, no dunk (layup fallback);
  everything else untouched. No hidden accuracy nerfs.
- Animation hooks (cheap, high-read): tired idle (hands near knees, deeper
  breathing amplitude), reduced dribble crouch, slower gait frequency at same
  speed. Rig already parameterizes all three.
- Deterministic: drain/refill rates in a StaminaStats resource, no randomness.
