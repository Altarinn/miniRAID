# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

miniRAID is a Unity-based tactical RPG game written in C#. The project uses a grid-based combat system with turn-based mechanics, featuring a backend-frontend architecture for game state management.

## Build & Development Commands

### Unity Project Setup
- **Unity Version**: This project uses Unity (check ProjectSettings/ProjectVersion.txt for exact version)
- **Open in Unity**: Open the project folder in Unity Hub
- **Build**: Use Unity's Build Settings (File > Build Settings) to build for target platforms

### Dependencies
- **Odin Inspector & Serializer**: Commercial Unity inspector enhancement and serialization system (has keyless claim option)
- **NuGet for Unity**: Package manager for .NET libraries
- **DOTween**: Animation library
- **XLua**: Lua scripting integration (legacy, being phased out - classes remain but Lua functionality unused)
- **Addressables**: Unity's asset management system
- **Localization**: Unity's localization package

### Common Development Tasks
- **Play Scene**: Open `Assets/Scenes/CombatBase.unity` and press Play in Unity
- **Debug Combat**: Use the Combat Monitor (`miniRAID` menu in editor) to track damage/healing
- **Monitor Mobs**: Use Mob Monitor to track aggro values
- **Test Builds**: Build for WebGL (configured in project)

## Core Architecture

### Three-Level Architecture
The game employs a sophisticated three-level architecture pattern that separates data definition, runtime behavior, and visual presentation:

1. **ScriptableObject Layer**: Static data definitions and templates (ActionDataSO, BuffSO, MobDescriptorSO)
2. **Runtime State Layer (BackendState)**: Dynamic behavior and game logic (RuntimeAction, Buff, MobData)  
3. **Renderer Layer (IStateRenderer)**: Visual presentation and UI (MobRenderer, Indicators)

**Key Benefits:**
- Clean separation of concerns between data, logic, and presentation
- Robust Odin Serializer support (backend state persists, SOs loaded via addressable paths, renderers recreate on load)
- Flexible rendering system with conditional visual feedback
- Event-driven state synchronization between backend and frontend

**See `Manual/Claude/Three-Level-Architecture.md` for detailed technical documentation.**

### Backend System
The game uses a clean separation between backend logic and frontend presentation:

- **`Databackend`** (`Assets/Scripts/Backend/Databackend.cs`): Central data store for map information, mob positions, and game state
- **`MobData`** (`Assets/Scripts/Backend/MobData/MobData.cs`): Core unit class representing characters, enemies, and their stats
  - **Position System**: Uses `Vector3` for floating-point positions (supports sub-grid movement)
  - **Collider Integration**: Each mob has an `IGridCollider` for collision detection and grid overlap queries
- **`CombatSchedulerCoroutine`**: Orchestrates turn-based combat flow using Unity coroutines
- **`MobListener`**: Event-driven system for buffs, equipment, skills, and other mob effects

### BackendState System
Core infrastructure for runtime state management:

- **`BackendState`** (`Assets/Scripts/Backend/BackendCore/BackendState.cs`): Base class for all persistent game state
  - **GUID Identity**: Each state has unique identifier for serialization
  - **Renderer Integration**: `IStateRenderer` interface connects backend state to visual presentation
  - **Lifecycle Management**: Registration with backend, automatic cleanup
- **`IRenderableState` Interface**: States that need visual representation implement this for automatic renderer creation
- **Odin Serialization Strategy**: ScriptableObject addressable paths and essential state serialize; SOs loaded from disk, calculated values and renderers recreate on load

### Collision System
- **`IGridCollider` Interface** (`Assets/Scripts/Backend/BackendCore/Colliders/GridColliders.cs`): Defines collision behavior
  - `Overlaps(IGridCollider other)`: Collider-to-collider collision detection
  - `OverlapsGrid(Vector3Int gridCell)`: Check if collider overlaps specific grid cell (for dynamic mob lookup)
  - `SetPosition(Vector3)` / `SetDirection()`: Transform control
- **`EnumerateGridCollider`** (`Assets/Scripts/Backend/BackendCore/Colliders/EnumerateGridCollider.cs`): 
  - Explicit grid cell enumeration using `HashSet<Vector3Int>`
  - Supports transforms, rotations, and floating-point positioning
  - Merged from legacy `GridShape` class
  - Uses efficient `HashSet<T>.Overlaps()` for collision detection
- **Future Extensions**: `SphereGridCollider`, `BoxGridCollider` with mathematical overlap computation

### Turn-Based Combat System
- **Turn Slices**: Modular turn components that can be combined to create complex turn structures
- **Action System**: Skill/spell system using `ActionDataSO` ScriptableObjects
- **Grid System**: 3D grid-based positioning with collision detection

### ScriptableObject Architecture
The project heavily uses Unity's ScriptableObject system for data-driven design:

- **Actions**: Skills and spells (`Assets/Scripts/Backend/SOCore/Action/ActionDataSO.cs`)
  - **Generic Type System**: `ActionDataSO<TSpellTarget>` for type-safe target handling
  - **Level-Based Scaling**: `PowerGetter` and `LeveledStats` for designer-friendly scaling
  - **Dynamic Calculation**: `LuaBoundedGetter` classes (legacy naming, no Lua functionality) for cost calculation
- **Buffs**: Status effects (`Assets/Scripts/Buffs/BuffSO.cs`)
  - **Stacking System**: Complex rules for buff stacking, refreshing, and duplicate handling
  - **Snapshot Mechanics**: Choose between one-time or continuous power calculations
  - **DOT/HOT Effects**: Integrated overtime effect system
- **Mob Descriptors**: Character templates (`Assets/Scripts/Backend/MobData/DescriptorSO/`)
- **Weapons & Equipment**: Gear system (`Assets/Scripts/Weapons/`)

**Template-Runtime Pattern**: ScriptableObjects define immutable templates that get instantiated into runtime objects with calculated stats and dynamic behavior.

### Key Directories
- **`Assets/Scripts/Backend/`**: Core game logic, data structures, and combat system
- **`Assets/Scripts/Presentation/`**: Visual components, rendering, and UI presentation layer
- **`Assets/Scripts/Spells/`**: Action/spell system with target requesters and helpers
- **`Assets/Scripts/UI/`**: User interface controllers and state management
- **`Assets/GameContent/`**: ScriptableObject data assets (characters, skills, enemies)
- **`Assets/Scripts/Utils/`**: Utility classes, globals, and editor tools

### Coroutine System
The project uses a custom coroutine system (`SerialCoroutine`, `JumpIn`) for:
- Combat flow orchestration
- Animation sequencing
- Asynchronous spell effects
- Turn-based timing control

### Event System
Extensive use of C# events for:
- Mob stat calculations (`OnStatCalculation`, `OnBaseStatCalculation`)
- Combat events (`OnDamage`, `OnHeal`, `OnDeath`)
- Turn progression (`OnTurnStart`, `OnTurnEnd`)
- Action triggers (`OnActionPerform`, `OnCostQuery`)

## Working with the Codebase

### Adding New Skills/Actions
1. Use `Create > Script Templates > New Action Data SO` in Project window
2. Inherit from `ActionDataSO<TSpellTarget>` and implement `OnPerform` method
3. Use coroutines with `yield return new JumpIn(...)` for animations
4. Create ScriptableObject asset using `Create Action` menu
5. **Runtime Creation**: Actions automatically wrap into `RuntimeAction<TSpellTarget>` instances with calculated stats

### Adding New Buffs/Effects
1. Use `Create > Script Templates > New Buff SO` in Project window  
2. Implement both `BuffSO` (data) and `Buff` (runtime) classes
3. Subscribe to `MobData` events in `OnAttach` method
4. Create asset using `Create Buff` menu
5. **Renderer Integration**: Implement `IRenderableState` in `Buff` class for visual indicators

### Adding New Backend States
1. Inherit from `BackendState` for basic state management
2. Implement `IRenderableState` if visual representation is needed
3. Override `ConstructRenderer()` to create appropriate renderer
4. Use `[NonSerialized]` for calculated values and renderer references
5. **Registration**: Call `Register()` to add state to backend tracking

### Code Conventions
- Use `Globals.backend` for accessing the main data backend
- Use `Globals.cc` for coroutine context and animation settings
- Backend logic should be separate from presentation/rendering
- Extensive use of Unity's serialization attributes (`[SerializeField]`, `[OdinSerialize]`)
- Chinese comments indicate bilingual development team
- **Architecture Patterns**:
  - ScriptableObjects: Immutable templates, pure functions only
  - Runtime States: Event-driven, use `[NonSerialized]` for calculated values
  - Renderers: Lightweight, focus on presentation, handle null states gracefully
  - Always unsubscribe from events in `OnRemove()` methods

### Position and Collision Patterns
- **Mob Positions**: Use `MobData.Position` (Vector3) for floating-point coordinates
- **Grid Coordinates**: Use `Vector3Int` for discrete grid cell references
- **Collider Usage**: Implement `IGridCollider` for entities needing collision detection
- **Pure Shapes**: Use `HashSet<Vector3Int>` for spell areas that don't need positioning
- **Grid Overlap**: Use `collider.OverlapsGrid(gridCell)` for dynamic mob-to-grid queries
- **Transform Logic**: Use `Mathf.FloorToInt()` when converting float positions to grid coordinates

### Debugging Tools
- **Combat Monitor**: Track damage, healing, and RNG history during combat
- **Mob Monitor**: Monitor individual unit stats and aggro values
- **Grid Shape Drawer**: Visualize spell/skill areas of effect
- **Debug logs**: Saved to `miniRAID.combat.log` and `miniRAID.log`

## Important Files
- **`Assets/Scripts/Utils/Globals.cs`**: Central singleton manager and global references
- **`Assets/Scripts/Backend/Databackend.cs`**: Main game state and map data
- **`Assets/Scripts/Backend/MobData/MobData.events.cs`**: Event definitions for the mob system
- **`Assets/Scripts/Backend/BackendCore/`**: Core backend infrastructure
  - `BackendState.cs`: Base class for all persistent game state with renderer integration
  - `Colliders/GridColliders.cs`: `IGridCollider` interface definition
  - `Colliders/EnumerateGridCollider.cs`: Hash-set based collider implementation
- **`Assets/Scripts/Presentation/`**: Renderer implementations
  - `MobRenderer.cs`: Complex state renderer for mob visualization
  - `IndicatorRenderer.cs`: Batched renderer for multiple indicators
  - `MobListeners/`: Specialized indicator renderers (sprites, decals, grid shapes)
- **`Assets/Scripts/Backend/BackendCore/GridShape.cs`**: Legacy shape class (being phased out)
- **`Manual/`**: Technical documentation
  - `Claude/Three-Level-Architecture.md`: Detailed architecture documentation
  - `index.md`: Detailed Chinese documentation for game mechanics
- **`README.md`**: Project setup and dependencies information
- **`FLOATING_POINT_POSITION_REFACTOR.md`**: Current refactoring task documentation

## Documentation Writing Guidelines

### When Writing Technical Documentation
- **Target Audience**: Assume readers have programming knowledge but may be unfamiliar with project specifics
- **Structure**: Use hierarchical organization with clear section headers
- **Code Examples**: Include relevant code snippets with explanations
- **Cross-References**: Link to related files and concepts throughout the documentation
- **Patterns**: Document design patterns, architectural decisions, and best practices
- **Integration**: Show how different systems work together, not just individual components

### Documentation Types to Create
- **Architecture Overviews**: High-level system design and interaction patterns
- **Implementation Guides**: Step-by-step instructions for common development tasks  
- **API References**: Detailed interface documentation with usage examples
- **Design Decisions**: Rationale behind architectural choices and trade-offs
- **Migration Guides**: How to handle changes in architecture or APIs

### File Organization
- Place technical documentation in `Manual/Claude/` directory for Claude-generated docs
- Use descriptive filenames that reflect content scope
- Cross-reference from `CLAUDE.md` for discoverability  
- Update `CLAUDE.md` with new architectural insights when writing docs
- **Serialization Notes**: Remember that Odin Serializer handles SO references as addressable paths, not direct object serialization