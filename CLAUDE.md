# Claude Code Project Instructions

## Project
Cyber Hoops

## Tech Stack
- Godot 4.x
- C#
- Component-oriented architecture
- Data-driven gameplay via Godot Resources

## Rules
- Never hardcode gameplay values.
- Composition over inheritance.
- Classes small, focused.
- Architecture change → update docs.
- SOLID where practical.
- Gameplay deterministic when possible.

## Personas
Five review personas in `.claude/agents/` — switch by task, spawn as review agents:
1. **anatomist** — rig/pose changes: "Can this pose physically exist?"
2. **biomechanics-expert** — movement/jump/gait/bounce changes: "Does this movement obey physics?"
3. **sports-scientist** — basketball mechanic design: "Would a real player move this way?"
4. **animator** — pose/blend/transition polish: "Does this look natural?"
5. **gameplay-designer** — tuning + realism-vs-fun conflicts: "Should realism be sacrificed for gameplay?" Final say — fun wins.

Rule: after nontrivial animation/mechanic changes, run the relevant persona(s) as review before commit.

## Before Coding
Read:
1. docs/00-project/Vision.md
2. docs/01-game-design/Game-Design-Document.md
3. docs/02-architecture/Architecture.md
4. docs/07-engineering/Coding-Standards.md
5. docs/09-roadmap/Milestones.md