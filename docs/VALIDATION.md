# Validation record

## Executed in the authoring workspace

`Tests/run.sh`: passed, domain build has zero warnings and errors. 200,049 assertions include 200,000 generation-bound/determinism assertions, full-tree C# syntax checks, and focused domain behavior checks.

Coverage: lane clamping, base/capped speed, swept crossing, score monotonicity after a booster, score reset, XP curve, daily eligibility and clock rollback, insufficient funds, invalid character IDs, free re-equipping, upgrade spending, cosmetic duplicate purchase prevention, mission and achievement reward idempotency, daily reward idempotency, personal-best monotonicity, save roundtrip, corrupted-primary backup recovery, and mission period reset.

The headless save harness substitutes Unity's JSON and application path APIs with System.Text.Json and a temporary directory. It compiles the actual SaveManager, MissionManager, ShopManager, RunRules, and ScoreManager source. Engine-dependent files receive Roslyn syntax checks only. This is not a full Unity compile.

## Provided but not executed

Unity Test Runner → PlayMode → LumenRush.PlayMode:

1. Lane change, jump height, and slide state response.
2. Pause freezes distance and resume restarts it.
3. Cargo collision, game over, free revive, and reset.
4. Shield consumption and power duration expiration.

Run Play Mode tests in an isolated local profile: they exercise real saves and can update progression. No test APK, iOS export, editor console, screenshot, frame-time trace, or device session has been produced because Unity is unavailable here.

## Required editor checks

- Fresh import: packages resolve, scripts compile, setup completes, no missing shaders/references.
- Open each of the five scenes directly and confirm the intended entry page.
- Start from Boot. Three lanes and at least two recycled sections remain continuous. No black/pink geometry.
- Test barriers by jump, gates by slide, cargo by lane switch, and patrol pressure after a pipe hit.
- Collect each of the nine power-ups and verify timing, score, shield, doubled tokens, magnet, burst and slow motion effects.
- Pause during a jump, background the app, return, resume, collide, revive once, collide again, retry and finish.
- Earn and claim each reward; restart the app and confirm wallet/settings/character/save recovery.
- Verify purchased cosmetics appear and do not change base movement or hitboxes.

## Required device checks

- Android ARM64 IL2CPP build and iOS Xcode build compile without errors.
- Narrow/large portrait screens and notches: buttons remain visible and safe area is respected.
- Fast swipes, UI taps, slide cancellation, and two consecutive lane changes respond reliably.
- Profile 10 minutes on a representative mid-range phone, 30 minutes for heat/battery behavior. Inspect main/render thread time, GC allocations, draw calls, memory, and pool counts across at least 100 section recycles.
- Confirm 60 FPS where feasible; verify manual battery mode at 30 FPS. Automatic quality selection has not been implemented.
- Inspect touch reaction windows at maximum burst speed. Adjust speed/row spacing only with playtesting.
- Review all audio levels on headphones and speaker; replace prototype cues as needed.
- Supply unique identifiers, release signing, icons, splash screens, accessibility/localization review, platform metadata, and appropriate store submissions before release.
