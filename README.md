# Lumen Rush

An original Unity/C# mobile endless runner set on the Afterlight transit line. Aria, a helmeted night courier, navigates three lanes of neon city infrastructure. All geometry and synthesized audio are original, generated locally; no third-party game artwork, branding, characters, or sounds are included.

**Status: connected MVP source project with several progression systems.** Placeholder geometry and procedural limb animation are included. This is not a finished realistic-art game or a store-ready binary. Unity is not installed in the authoring environment, so editor compilation, Play Mode, Android/iOS builds, visual review, and device performance remain unverified.

## Open and play

1. Install Unity **6000.0.23f1**, including Android Build Support and/or iOS Build Support, through Unity Hub. A compatible newer Unity 6.0 patch can be tried in a copy of the project.
2. In Hub, choose **Add project from disk** and select this repository.
3. Let Unity restore packages and compile scripts. The editor setup automatically creates the URP renderer/pipeline, the configuration asset, build scene list, portrait settings, and ARM64 mobile settings. Package restore needs network access.
4. Open `Assets/Scenes/Boot.unity` and press **Play**. Choose **START A RUN**.
5. If setup was interrupted, use **Lumen Rush → Configure project**. No Inspector references need to be dragged manually.

Touch: swipe left/right to change lanes, up to jump, down to slide. Keyboard: arrows or A/D, Space to jump, S to slide, P/Escape to pause. Follow tokens through an unobstructed lane; orange barriers can be jumped, overhead gates require sliding, cargo pods require lane changes.

## Included

- Three lanes, automatic scrolling world, CharacterController movement, jump gravity, slide collider resizing, difficulty curve, swept obstacle checks, pause, game over, restart, and one free revive per run.
- Six reusable city sections, pooled coins and obstacles, jump barriers, slide gates, cargo containers, cargo pods, and stumble pipes. Every obstacle row reserves one clear lane. Speed caps and row spacing preserve lane-change reaction time.
- Distance scoring that accumulates the multiplier at earning time; token collection; nine timed power-ups: magnet, score booster, shield, speed burst, double coins, air dash (automatic hazard bypass), hover board protection, slow motion, and invincibility.
- Aria plus two unlockable courier color styles, a cosmetic deck and trail, five earnable duration upgrades, local wallet/high score, XP levels, daily reward, daily/weekly assignments, four career achievements, and a personal leaderboard.
- Touch-scaled UI, safe area handling, settings, 30/60 FPS targets, haptics, generated sound effects and an original synth loop, a following camera with boost FOV, and a patrol drone after a stumble.
- Shared instanced materials and scenery combined by material per section. No runtime instantiation in ordinary recycling after pool warmup unless configuration exceeds the pool budget.
- Local JSON save with previous-file recovery, plus `ISaveStore` and `ILeaderboardService` boundaries for future services.

## Validation

Run `Tests/run.sh` with a .NET 6 SDK installed. It compiles the actual domain code, tests 100,000 generated safe-lane selections and determinism, and checks scoring, purchases, rewards, period resets, and save recovery. It also syntax-parses every C# source, including Unity scripts. It does **not** validate Unity API types or execute the engine.

Last run: **200,049 checks passed**, zero build warnings/errors in the domain harness. Four Unity Play Mode tests are included but have **not** been run. See [the validation checklist](docs/VALIDATION.md).

## Mobile builds

Use **Lumen Rush → Build Android APK** or **Export iOS Xcode project** with the appropriate Unity platform module installed. Outputs go under `Builds/`. iOS compilation/signing requires macOS, Xcode, and your Apple signing identity. Android signing and release bundle configuration must be supplied for distribution. Use your own application identifiers before publishing.

## Project guide

- [System wiring and Inspector defaults](docs/SETUP.md)
- [Validation and device acceptance checks](docs/VALIDATION.md)
- [Implemented scope and production backlog](docs/ROADMAP.md)

The project pins URP 17.0.3 for Unity 6.0, consistent with [Unity's release notes](https://legacy-activation.unity3d.com/releases/editor/whats-new/6000.0.0f1). The UI assembly reference follows [Unity's uGUI source](https://github.com/Unity-Technologies/uGUI/blob/6000.0/com.unity.ugui/Runtime/UGUI/UnityEngine.UI.asmdef).
