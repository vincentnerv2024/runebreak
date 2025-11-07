# Unity Project Notes

## Overview
- Engine: Unity 2021 LTS
- Rendering: URP (default configuration from empty project)
- Core namespace: `Assets/_Runebreak`
- Scene in use: `GameplayScene`

## Architecture Highlights
- `GameManager`: orchestrates game flow, manages saves, and tracks current layout selection.
- `GridManager`: generates card layouts with automatic scaling through `GridLayoutGroup`.
- `Card`: UI-based card implementation with layered visuals (front/back/frame/glow) and flip animations.
- `ScoreManager`: handles scoring, moves, and combo system.
- `AudioManager`: centralised SFX playback with hover pitch randomisation.
- `SaveLoadManager`: JSON persistence at `Application.persistentDataPath`.
- `UIManager`: updates HUD, handles level selection buttons, and displays end-of-round summary.

## Packages & Tools
- **NaughtyAttributes**: Imported for improved inspector visibility, especially the sprite previews (`ShowAssetPreview`). Usage captured in `NaughtyAttributesUsage.png`.
- **2D Sprite Package**: Unity package manager entry to access sprite utilities.

## Visual Assets
- Card back (`CardBack.png`): custom-made graphic.
- Front symbols (`1.png` – `15.png`): AI-generated source art, refined for this prototype.
- Frame & glow (`Frame.png`, `FrameStone.png`, `Glow.png`): custom layered overlays to support animation.
- Reference screenshots:
  - `FolderStructure.png`: project hierarchy.
  - `Hierarchy.png`: active scene object structure.

## Audio Assets
- Flip, hover, match, mismatch, and game-over sounds sourced from **Zapsplat** and **Freesound.org** (licence friendly for prototypes). Original filenames are preserved in `Assets/_Runebreak/Sounds/`.

## Scene Setup Checklist
1. `Canvas` configured with `Canvas Scaler` → Scale With Screen Size (1920×1080 reference).
2. `GridContainer` panel hosts a `GridLayoutGroup` and receives card prefabs.
3. `TopPanel` contains score, moves, combo, and level selection buttons.
4. `GameManagers` object aggregates all manager scripts for easy reference.
5. `GameOverPanel` remains inactive until triggered by `UIManager`.

## Card Prefab Notes
- Root GameObject contains `Button`, `Card` script, and transparent `Image` used as raycast target.
- Children:
  - `BackImage` (Sprite)
  - `FrontImage` (Sprite)
  - `FrameImage` (Sprite)
  - `GlowImage` (Sprite with initial alpha 0 and `Raycast Target` disabled)
- Flip animation uses scale interpolation to keep UI orientation intact.

## Save Data
- Location: `Application.persistentDataPath/gamesave.json`
- Contents: grid layout, matched state, score data, and elapsed play time.
- Deleting the save forces a new session on next run.

## Testing Tips
- Use level buttons (2×2, 2×3, 4×4, 4×5, 5×6) to validate scaling.
- Confirm hover audio variation with rapid mouse movement across unmatched cards.
- Restart respects previously selected layout.
