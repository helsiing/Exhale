# Exhale — Project Context for AI Agents

## 1. Project Overview

Exhale is a **hexagonal city-building card game** in active development, built with Unity 6 (6000.3.8f1). The core loop is: draw hex-piece cards from a hand → click a tile on a hex grid board to "arm" it → click a card to launch it onto the armed tile → the piece spawns on the board and adjacent tiles unlock. The project targets **PC/Mac desktop** (mouse + keyboard controls). It uses the **Universal Render Pipeline (URP 17.3.0)** and is a personal/indie project with no publisher. Art uses the KayKit Medieval Hexagon asset pack as a base, with custom pieces layered on top.

---

## 2. Repository Layout

```
Exhale/Exhale/                  ← Unity project root
├── Assets/
│   ├── Art/
│   │   ├── Cards/              ← Card sprite art
│   │   ├── KayKit/             ← Third-party hex tile/building art pack (URP materials)
│   │   └── Pieces/             ← Custom piece models (Buildings, Nature, Path, River)
│   ├── Data/
│   │   ├── Board/              ← BoardConfig ScriptableObjects
│   │   ├── Pieces/Templates/   ← HexPieceTemplate SO assets (Buildings/, Ground/)
│   │   └── Yields/Items/       ← YieldTemplate SO assets
│   ├── Plugins/
│   │   ├── DOTween/            ← DOTween tweening library
│   │   ├── EventBus/           ← Unused/experimental EventBus plugin
│   │   ├── NuGet/              ← NuGet-sourced DLLs (SignalR client, R3, System.*)
│   │   ├── ServiceLocator/     ← Custom Service Locator framework (key plugin)
│   │   ├── Sirenix/            ← Odin Inspector + Odin Validator
│   │   └── StateMachine/       ← Simple StateMachine plugin (StateMachine, State, TransitionSequencer)
│   ├── Prefabs/
│   │   ├── Board/Pieces/       ← Board piece prefabs (Buildings/, Ground/)
│   │   └── Cards/              ← Card prefabs (Buildings/, Ground/)
│   ├── Resources/
│   │   └── Services/           ← ServiceLocator prefab (loaded at runtime via Resources.Load)
│   ├── Scenes/
│   │   ├── Boot.unity          ← Entry point scene
│   │   ├── Logic.unity         ← Logic/services scene
│   │   ├── Game.unity          ← Main gameplay scene
│   │   ├── Environment.unity   ← Visual environment scene
│   │   ├── Exhale.unity        ← Primary scene (may combine the above)
│   │   └── Exhale/Exhale (ECS).unity  ← ECS subscene (board entities)
│   ├── Scripts/                ← All game C# code (see §4)
│   ├── Settings/               ← URP renderer/pipeline assets
│   └── TextMesh Pro/           ← TMP fonts and resources
├── Packages/manifest.json      ← Package dependencies
└── ProjectSettings/
    └── ProjectVersion.txt      ← Unity 6000.3.8f1
```

**Assembly definitions (`.csproj` roots):**
- `Exhale.Scripts` — main game scripts
- `Exhale.Utils` — utility helpers
- `Exhale.Plugins.ServiceLocator` — service locator framework

---

## 3. Architecture Patterns

### Hybrid DOTS + MonoBehaviour

The project uses a deliberate hybrid:
- **ECS (DOTS)** owns the board simulation: tile entities, piece spawning, physics raycasting, adjacency logic.
- **MonoBehaviour** owns UI: card hand view, camera controller.
- **Plain C# classes** (services) own cross-cutting game state: placement flow, inventory, data access.

The bridge between ECS and the MonoBehaviour world is the **Service Locator** and the **`PlacementService`** C# event bus.

### Service Locator

Custom plugin at `Assets/Plugins/ServiceLocator/`. Central singleton (`ServiceLocator`) loaded from `Resources/Services/ServiceLocator` prefab. Services are registered by `ServicesBootstrapper` (a `ServiceReporter<IService>` MonoBehaviour) on `Awake`.

Consumers never call `ServiceLocator.GetInstance()` directly — they declare a `ServiceReference<IService>` field and access `.Reference`, which lazy-caches the service and auto-invalidates when a service is unregistered.

```csharp
// Pattern used everywhere, in both MonoBehaviours and ECS SystemBases
private readonly ServiceReference<IPlacementService> placementService = new();
// ...
placementService.Reference?.TryArmTile(pos, entity);
```

### ScriptableObject Collections (BrunoMikoski)

All piece templates are `HexPieceTemplate` assets that extend `ScriptableObjectCollectionItem`. The collection `HexPieceTemplateCollection` gives global static access to all pieces via `HexPieceTemplateCollection.Values`. `DataService` wraps this as the canonical entry point.

### Trait Composition on HexPieceTemplate

Pieces use composition via a `List<PieceTrait>` (serialized with `[SerializeReference, SubclassSelector]`). Traits are plain-C# `[Serializable]` classes. Current traits:

| Trait | Purpose |
|---|---|
| `BoardObject` | Declares board prefab; required for any piece that can be placed on the grid |
| `CardObject` | Declares card hand prefab |
| `Building` | Gameplay data — placement requirements (adjacent pieces), cost (YieldTemplate amounts) |
| `Ground` | Ground tile type |
| `Thumbnail` | Prefab for thumbnail generation |
| `Saleable` | (stub) marks piece as purchasable |

Access pattern: `template.TryGetTrait<Building>(out var b)` / `template.HasTrait<BoardObject>()`.

---

## 4. Core Systems

### 4.1 Board (ECS)

**Key files:** `Assets/Scripts/ECS/`

The board is a flat hex grid. All tile entities live in the ECS subscene (`Exhale (ECS).unity`). Only one tile is initially enabled (the start tile); placing a piece on a tile enables its 6 axial neighbors.

**System execution order:**
```
PointerInputSystem         → writes PointerInputData singleton (ray origin/dir, click)
TileHighlightSystem        → raycasts, scale-bumps hovered tile
TileArmingSystem           → on click, arms tile via PlacementService; tags adjacent disabled tiles
CancelInputSystem          → Esc/RMB → PlacementService.Cancel()
PiecePlacementSystem       → subscribes to PlacementService.OnCardLanded, creates PieceCreationRequest entities
EnableAdjacentTilesSystem  → when TileData changes (IsOccupied), enables disabled neighbor tiles
PieceFactorySystem         → consumes PieceCreationRequest, instantiates the piece entity from prefab catalog
BoardInitializationSystem  → one-shot: spawns all tiles, fires BoardService.TriggerBoardInitialized()
```

**Key ECS components:**
- `TileData` — `int2 PositionIndex`, `bool IsOccupied`, `bool IsEnabled`
- `TileDataHighlight` — `IsHighlighted`, `IsArmed`, `IsAdjacencyPreview`
- `TileArmedTag`, `TileAdjacencyPreviewTag`, `TileHighlightColorOverride` — structural tags used for visual state
- `PieceCreationRequest` — transient entity: `int PieceId`, `int2 PositionIndex`
- `BoardDataComponent` — grid Width/Height/StartPosition/prefab entity reference
- `BoardPosition` — placed on spawned piece entities
- `PointerInputData` — singleton written each frame by `PointerInputSystem`
- `SpawnPiecesConfig` / `PieceEntityData` — baked catalog mapping `PieceId` → prefab entity

**Hex coordinate system:** offset coordinates, flat-top-ish. `BoardHelper.HexToWorldPosition` computes world pos: every odd row gets a +halfWidth x-offset.

### 4.2 Placement Flow (PlacementService)

**File:** `Assets/Scripts/Services/PlacementService.cs`

Central state machine bridging ECS and MonoBehaviour. Plain C# (no MonoBehaviour).

```
Idle ──hover──► TileHovered ──click tile──► TileArmed ──click card──► CardLaunching ──card lands──► Resolving ──► Idle
                                                         ◄── Esc/RMB ─────────────────────────────
```

Events: `OnTileHovered`, `OnTileArmed`, `OnTileDisarmed`, `OnCardLaunchStarted`, `OnCardLanded`

`OnCardLanded` is the critical cross-boundary event: `PiecePlacementSystem` (ECS) enqueues the placement; `GameHandView` (MonoBehaviour) draws the next card.

### 4.3 Card Hand (MonoBehaviour)

**Files:** `Assets/Scripts/Cards/`, `Assets/Scripts/Services/GameHandService.cs`

- `GameHandService` (MonoBehaviour service) — owns the hand list and available pool; random draw without deck depletion.
- `GameHandView` — renders cards along a Spline (Unity Splines package). Uses `LeanPivot` intermediate GameObjects to separate lean rotation (CardHandLeanController) from hover/position tweens (CardHandHoverBehavior).
- `CardView` — individual card; on `OnMouseDown` calls `placementService.Reference.TryLaunchCard(Template)`.
- `CardLaunchAnimator` — plays the card flight animation to the armed tile.
- `PlacementAudioController` — audio feedback on placement events.
- `CardHandFilter` — (stub) will filter valid cards based on armed tile.

### 4.4 Camera

**Files:** `Assets/Scripts/Camera/CameraController.cs`, `Assets/Scripts/Services/CameraService.cs`

`CameraController` (MonoBehaviour) handles pan (keyboard WASD / middle-mouse drag), rotate (Q/E keyboard / right-mouse drag), and zoom (scroll wheel → FOV change). Uses the new Input System via a generated `CameraControlActions` asset. `CameraService` exposes `CenterAtPosition(int2)` to non-MonoBehaviour callers; `BoardInitializationSystem` calls it after board setup.

### 4.5 Data / Templates

**Files:** `Assets/Scripts/Data/`

- `HexPieceTemplate` — `ScriptableObjectCollectionItem` with `List<PieceTrait>`. Piece identity for ECS is a deterministic GUID-derived int (`GetId()`).
- `HexPieceTemplateCollection` — BrunoMikoski collection; global `Values` accessor.
- `YieldTemplate` — resource type SO (wood, stone, etc.); lives in `YieldTemplateCollection`.
- `BoardConfig` — ScriptableObject wrapping `BoardDataComponent` (Width, Height, StartPosition), `BoardStartType`, and the empty tile prefab reference.

---

## 5. Data & Event Flow

```
ScriptableObjectCollections (HexPieceTemplateCollection, YieldTemplateCollection)
    └── DataService.GetHexTilesAvailable()
            └── GameHandService (draws random cards into hand list)
                    └── GameHandView (renders hand along spline)
                            └── CardView.OnMouseDown()
                                    └── PlacementService.TryLaunchCard()
                                            │
                                            ├── (event) OnCardLanded
                                            │       ├── PiecePlacementSystem (ECS) → creates PieceCreationRequest entity
                                            │       │       └── PieceFactorySystem (ECS) → instantiates piece entity
                                            │       │               └── EnableAdjacentTilesSystem (ECS) → unlocks neighbors
                                            │       └── GameHandView → draws next card
                                            │
                                            └── (event) OnCardLaunchStarted
                                                    └── CardLaunchAnimator → plays flight tween

TileArmingSystem (ECS, on click) → PlacementService.TryArmTile()
    └── (event) OnTileArmed → CardHandFilter (validates which cards are playable)

BoardInitializationSystem (ECS, one-shot) → BoardService.TriggerBoardInitialized()
    └── CameraService.CenterAtPosition()
```

**Key rule:** ECS systems do NOT directly touch MonoBehaviours. They communicate outward only via `PlacementService` (a plain C# service) or via `BoardService`. MonoBehaviours subscribe to service events and never call ECS APIs directly.

---

## 6. Third-Party Packages

| Package | Version | Role |
|---|---|---|
| `com.unity.entities` | 1.4.2 | DOTS/ECS — board simulation, tile entities |
| `com.unity.physics` | 1.4.2 | Physics raycasting for tile hit detection |
| `com.unity.entities.graphics` | 1.4.15 | ECS rendering (entities render pipeline) |
| `com.unity.render-pipelines.universal` | 17.3.0 | URP — renderer, materials, post-processing |
| `com.brunomikoski.scriptableobjectcollection` | 2.5.0 | Global SO collections (`HexPieceTemplateCollection`, `YieldTemplateCollection`) |
| DOTween (in Plugins/) | — | Tweening for cards, camera (`DOMove`, `DOColor`, `DOLocalRotateQuaternion`) |
| Odin Inspector + Validator (Sirenix, in Plugins/) | — | Editor tooling; `[InlineProperty]`, `[BoxGroup]`, `[Title]` on data classes |
| `com.lbg.subclassselector` | git | `[SubclassSelector]` attribute — polymorphic trait list in Inspector |
| `com.cysharp.unitask` | git | UniTask async utilities (available, limited use visible) |
| `com.unity.inputsystem` | 1.18.0 | New Input System — camera and pointer input |
| `com.unity.splines` | 2.8.2 | Spline along which card hand is laid out |
| `com.unity.cinemachine` | 3.1.6 | Present; not visibly wired in current code |
| `com.unity.timeline` | 1.8.10 | Present; not visibly used in current scripts |
| `com.unity.visualeffectgraph` | 17.3.0 | VFX graph (available) |
| SignalR client (NuGet, in Plugins/) | 8.0.15 | Future multiplayer networking — not yet used in game code |
| R3 (NuGet, in Plugins/) | 1.3.0 | Reactive programming library — present, use not visible in current scripts |
| `com.coplaydev.unity-mcp` | git | MCP plugin enabling AI agent control of the Unity Editor |

---

## 7. Code Conventions

- **Namespaces** mirror the folder under `Scripts/`: `Exhale.Scripts.Services`, `Exhale.Scripts.Data`, `Exhale.ECS.Systems`, `Exhale.ECS.Components`, `Exhale.ECS.Authoring`, `Exhale.Plugins.ServiceLocators`, `Exhale.Cards.UI`, `Exhale.Board`, `Exhale.Utils`.
- **Services**: always defined as `interface IXxxService : IService` + `class XxxService : IXxxService`. Services implement `IDisposable` via `Dispose()`.
- **ServiceReference**: the universal pattern to consume a service — field `private readonly ServiceReference<IXxx> xxx = new();`, accessed via `xxx.Reference`.
- **ECS**: Burst-compiled pure systems use `ISystem` + `[BurstCompile]`. Managed systems (those needing ServiceReferences, event callbacks) use `SystemBase`.
- **ECS authoring**: `IComponentData` structs and their `Baker<T>` inner class live in the same file in `ECS/Authoring/`.
- **Data authoring**: Odin `[InlineProperty]`, `[BoxGroup]`, `[Title]` used on designer-facing ScriptableObjects and traits.
- **Traits**: subclasses have `[SubclassPath("Category", "Display Name")]` for the Inspector dropdown.
- **Hex positions**: always `int2` (Unity.Mathematics). World positions use `BoardHelper.HexToWorldPosition(int2)`.
- **PieceId**: stable across play sessions — derived by XOR-folding the raw GUID longs (not `GetHashCode()`, which is domain-randomized).

---

## 8. Known Issues & Tech Debt

1. **`TileHighlightSystem` bug** (`TileHighlightSystem.cs:76`): when checking if the highlight should update, it reads `SystemAPI.GetComponent<TileDataHighlight>(highlightedTile)` instead of `(tileEntity)`, causing highlight state to be checked against the wrong entity.

2. **`PlacementService.IsCardValidForTile` is a placeholder** — always returns `true` for any `BoardObject` card. Terrain constraints and cost checks (commented as "Step 8 CardHandFilter") are not yet implemented.

3. **`Building.ValidateConfig` incomplete** — `//TODO: Validate cost data` at line 168.

4. **`BoardStartType.AtPosition` commented out** in `BoardAuthoring.cs` baker switch — dead enum value.

5. **Debug.Log spam in release paths** — `PlacementService`, `PiecePlacementSystem`, `PieceFactorySystem`, `BoardInitializationSystem` all have `Debug.Log` on every placement event.

6. **`InventoryService` read-only gap** — only has `AddYield()`; no method to query current inventory. Blocks any UI that needs to display resources.

7. **`GameHandService` draw is unweighted random** — `DrawNextCard` draws any card from the full available pool every time; no deck/discard/weight system yet.

8. **`CameraController.ClampCameraPosition()`** is commented out — camera has no position bounds.

9. **SignalR + R3 dependencies imported but unused** — large DLL weight for functionality not yet built.

10. **`ECS.Systems` namespace inconsistency** — `TileArmingSystem`, `PiecePlacementSystem`, `CancelInputSystem` are in `namespace ECS.Systems` (no `Exhale.` prefix), while others use `Exhale.ECS.Systems`. This may cause future confusion.

---

## 9. Onboarding Tips for Agents

1. **The placement flow is the spine of the game.** Before touching any feature involving the board, cards, or pieces, read `PlacementService.cs` first. Every interaction — tile arm, card launch, piece spawn — routes through it.

2. **ECS and MonoBehaviours communicate only via services.** ECS systems hold `ServiceReference<IXxx>` fields and subscribe to events. MonoBehaviours never call ECS APIs. Do not break this contract.

3. **`HexPieceTemplate` is the universal piece descriptor.** It is a ScriptableObject, not an ECS component. When ECS needs to reference a template, it uses the deterministic `int` from `GetId()` (GUID-derived, not `GetHashCode()`). The `PieceFactorySystem` matches `PieceId` to the baked entity catalog.

4. **The board hex grid uses offset coordinates.** Odd rows are shifted +0.5 * hexWidth on X. Neighbour positions use axial offsets (`±1,0`, `0,±1`, `+1,-1`, `-1,+1`). `EnableAdjacentTilesSystem` and `TileArmingSystem` both hard-code these six offsets — they must match.

5. **Tiles start `Disabled`.** Only the start tile is enabled at boot. `EnableAdjacentTilesSystem` enables neighbors when `IsOccupied` changes. A tile must be `IsEnabled && !IsOccupied` to be armable. Check these flags before reasoning about why a click has no effect.

6. **Service registration order matters.** `ServicesBootstrapper.RegisterServices()` runs on `Awake`. ECS worlds start before or concurrently. ECS systems use lazy subscription (`if (!subscribed && placementService.Reference != null)`) to handle the race. Follow this pattern in new ECS systems.

7. **Traits are queried, not cast.** Always use `template.TryGetTrait<T>(out var t)` or `template.HasTrait<T>()`. Do not cast `Traits[i]` directly.

8. **`ServiceReference<T>` is safe to use in `OnDestroy` / `OnDisable` only via `HasCachedReference` + `CachedReference`.** Never call `.Reference` in those lifecycle methods — use the cached variant.

9. **URP + Entities Graphics.** The project uses `com.unity.entities.graphics`. Piece prefabs baked into ECS use `RenderMeshArray` / `MaterialMeshInfo`. If adding new visual ECS entities, use `RenderMeshUtility.AddComponents` in the baker.

10. **The `Resources/Services/ServiceLocator` prefab is the bootstrap anchor.** It carries `ServicesBootstrapper` which wires all service instances. If a service is missing at runtime, check that prefab and `ServicesBootstrapper.RegisterServices()` first.
