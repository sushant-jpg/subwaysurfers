# Scope and next production milestones

The brief describes a full commercial game. This repository delivers its first connected Unity MVP plus local progression extensions. It does not claim all requested content is complete.

| Area | Current implementation | Remaining production work |
| --- | --- | --- |
| Core runner | Connected three-lane loop, jump/slide, hazards, coins, score, pooling, pause, revive | Unity/device validation, detailed balance, tutorial iteration |
| World | One neon urban location with raised sidewalks, window bands, cargo and sky bridges | Eight authored locations, location unlocks, day/night modes, rain/weather, mountains/coast, rooftops and tunnels |
| Art | Original primitive city/courier models, shared URP materials | Realistic assets, textures, baked lighting, reflection probes, authored LODs and occlusion strategy |
| Animation | Procedural run limbs, jump/slide poses | Full idle/stumble/hit/victory/revive clips, production rig and animation transitions |
| Hazards | Barriers, gates, cargo boxes/pods, pipes; patrol drone after mistakes | Independent moving vehicles, road holes, rotating machinery, pedestrians, richer chase behavior |
| Powers | Nine functional timed effects, duration upgrades, sound/HUD feedback | Distinct pickup silhouettes, unique VFX, air-dash animation and comprehensive balance |
| Progression | Wallet, XP levels, 3 color-style couriers, 2 cosmetics, daily/weekly/career rewards | Outfit/shoe/backpack/accessory catalogs, level-up rewards, chests and content unlock tracks |
| Interface | Home, HUD, pause/results, characters, shop, missions/achievements, profile, daily rewards, settings | Art pass, screen-reader/accessibility review, localization, animated transitions |
| Audio | Original generated loop, footsteps and event tones | Environment-specific compositions, ambient city mix, distinct production SFX |
| Camera | Stable follow and boost FOV | Hit shake and authored cinematic transitions |
| Performance | Pooled content, combined scenery, shared instancing-capable materials, 30/60 targets | Measured device tuning, automatic graphics adaptation, texture compression and thermal profiling |
| Services | Recoverable local JSON, local personal best, service interfaces | Authenticated cloud saves, real online rankings, offline sync and anti-cheat |
| Release | Android/iOS build menu and platform defaults | Engine validation, signed builds, icons, store materials and release QA |

No purchases use real money. Revive is free once per run. Couriers and cosmetic items have identical base movement and collision rules. Duration upgrades are earned using gameplay tokens.
