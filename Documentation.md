# RuneBreak Card-Match Prototype

## Summary
- Functional card-matching prototype built in **Unity 2021 LTS**.
- Code architecture and implementation designed by the developer; written inside Cursor CLI workflow.
- Supports dynamic layouts (2×2, 2×3, 4×4, 4×5, 5×6), persistent save/load, scoring with combo, audio feedback, and layered card visuals.

## Key Systems
- `Card.cs`: UI-based card with flip animation, glow effects, and hover audio.
- `GridManager.cs`: Flexible grid generator with automatic scaling based on aspect ratio.
- `GameManager.cs`: Game flow, layout persistence, save/load integration.
- `ScoreManager.cs`: Score, moves, combo tracking.
- `AudioManager.cs`: Centralised SFX playback, including hover pitch randomisation.
- `SaveLoadManager.cs`: JSON serialisation.
- `UIManager.cs`: HUD updates, level selection, game-over presentation.

## Getting Started
1. Open the project in Unity 2021 LTS.
2. Load `Assets/_Runebreak/Scenes/GameplayScene.unity`.
3. Press Play to test. Use top-level buttons to switch layouts or restart the session.
4. Builds target desktop by default; mobile build settings can be configured per platform requirements.

## Assets & Credits
- **Card Back**: Custom artwork created by the developer.
- **Card Front Symbols**: AI-generated imagery, curated specifically for this prototype.
- **Frame & Glow**: Custom overlays authored for layered visuals.
- **Audio**: Sourced from [Zapsplat](https://www.zapsplat.com) and [Freesound.org](https://freesound.org). Original filenames retained in `Assets/_Runebreak/Sounds/`.
- **NaughtyAttributes**: Imported (MIT licence) to improve Inspector usability—see `Assets/_Runebreak/Documentation/NaughtyAttributesUsage.png`.

## Tooling
- IDE / Assistant: Cursor CLI environment (similar capabilities to Claude CLI and Codex CLI).
- Version Control: Git (initialised from empty project, meaningful commits throughout development).

## Reference Screenshots
Located in `Assets/_Runebreak/Documentation/`:
- `FolderStructure.png`: Project folder layout.
- `Hierarchy.png`: Scene hierarchy overview.
- `NaughtyAttributesUsage.png`: Inspector preview showcasing NaughtyAttributes integration.

## Additional Notes
- Save data lives at `Application.persistentDataPath/gamesave.json`; delete to reset progress.
- Card aspect ratio is configurable via `GridManager.cardAspectRatio` (default matches custom card art at ~0.704).
- Hover audio pitch variance can be tuned through `AudioManager` inspector fields.
