# Scene and system wiring

`Bootstrap` runs after a scene loads and adds one `GameManager` if absent. `GameManager.Awake` builds and connects all runtime objects. The five checked-in scenes are thin entry points into the shared application; they do not contain five duplicated worlds. Boot/MainMenu open the menu; Gameplay starts a run; CharacterSelection and Shop open their matching pages. Navigation keeps one world and switches UI panels.

The generated geometry appears in the Hierarchy during Play Mode. Empty authoring folders are intentionally reserved for future imported art. The MVP uses factories instead of hand-authored prefab files; there are no missing prefab references.

| Source under Assets/Scripts | Runtime owner and responsibility |
| --- | --- |
| Core/Bootstrap, GameManager | Root application; lifecycle, dependency wiring, run state and reward banking |
| Core/RunnerConfig | `Resources/RunnerConfig.asset`; editable movement and generation settings |
| Core/RunRules, ScoreManager | Pure rules and accumulated score |
| Core/ShopManager | Purchases and local leaderboard service adapter |
| Input/SwipeInputManager | Root input component; touch plus keyboard events |
| Player/PlayerController | Aria GameObject, CharacterController, replaceable model child, limb animation |
| Player/FollowCamera | Tagged Main Camera plus AudioListener; third-person follow and patrol drone |
| Player/CosmeticEffects | Aria child effects; equipped deck, trail, protection indicator |
| Level/LevelGenerator | Pooled city root; modular scenery, obstacle patterns and recycling |
| Level/ObjectPoolManager | Inactive pool root; reusable item factories |
| Level/Visuals, SceneryBatch | Shared URP materials and batched city meshes |
| Obstacles/TrackItem | Pooled item components; swept longitudinal and lane/height checks |
| PowerUps/PowerUpManager | Duration/effect state owned by GameManager |
| Missions/MissionManager | UTC daily/weekly rollovers, achievements, run counters and rewards |
| Save/SaveManager | Local persistence adapter; JSON and previous save recovery |
| Audio/AudioManager | Root AudioSources, original synthesized music and cues |
| UI/UIManager, SafeArea | Overlay canvas, input EventSystem, pages and HUD |

## Default configuration

The editor creates `Assets/Resources/RunnerConfig.asset`. In its Inspector:

| Setting | Default | Meaning |
| --- | --- | --- |
| Lane response | 15 | Exponential interpolation speed |
| Jump height | 2.6 | World units above the road |
| Gravity | 26 | Downward acceleration |
| Slide duration | 0.8 | Seconds; standing collider shrinks from 1.8 to 0.8 |
| Power duration | 8 | Seconds, plus purchased upgrades |
| Segment count | 6 | Recycled world sections |
| Rows per segment | 4 | Obstacle/coin rows |
| Row spacing | 18 | World units; retest fairness before reducing |

Lane width is 2.7, base forward speed 12, normal speed cap 30, burst speed cap 39. The player remains near Z=0 while the world moves backward, avoiding floating-point drift in long runs. A continuous static collider provides the controller floor. Obstacle checks use the interval between previous and current Z positions, avoiding a simple single-frame overlap test.

Obstacles always leave a lane clear. They do not require a power-up or purchased item to pass. Each row uses a deterministic hash of a fresh run seed and row number. This guarantees a geometrically clear route; touchscreen reaction difficulty still requires playtesting.

The editor creates `Assets/Settings/MobileRenderer.asset` and `MobileURP.asset`, assigns URP, retains the dynamically loaded Lit shader for builds, uses 2x MSAA and a 40-unit shadow distance. Runtime graphics settings select 30 FPS/no shadows or 60 FPS/hard shadows. These are targets, not measured performance results. Legacy input is used; keep Active Input Handling set to Input Manager (Old) or Both if changing project settings.

## Replacing art

Replace the courier's `Replaceable animated model` construction in PlayerController with an imported prefab and Animator while retaining the controller dimensions and movement root. The existing limb animation is procedural, not authored animation clips. Use animation states for idle, run, jump, slide, stumble, hit, victory, and revive in the production model.

Replace primitive children in ObjectPoolManager.Create with prefab instances, keeping the TrackItem root, ItemKind, and obstacle footprints. Generation only calls Take/Return and requires no changes for replacement assets. Replace building construction in BuildScenery with modular meshes; retain per-section ownership and pooling. Imported meshes combined by SceneryBatch must be readable; use authored static batches/LODGroups for production art instead where appropriate.

## Save and rewards

Save files live at Unity's `Application.persistentDataPath/lumen-save.json`, with `.bak` recovery and a temporary write file. A corrupt primary is read from the backup; both unreadable files start a fresh profile and log warnings. The local save is not encrypted, cloud synchronized, or cheat resistant.

Run earnings and mission progress are banked on Finish/Home, Retry, or orderly application quit. A suspended active run pauses and saves previously banked progress; operating-system termination during that run can lose its unbanked earnings. Reward buttons, purchases, and settings save immediately. Daily/weekly scheduling uses device UTC and is suitable for local play, not authoritative competitive rewards.

Cloud save can implement ISaveStore. A hosted leaderboard can implement ILeaderboardService; authentication, async service calls, offline submission queues, reconciliation, and anti-cheat remain integration work. No network calls or external service credentials are used by this MVP.
