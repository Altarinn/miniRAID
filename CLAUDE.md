# CLAUDE.md

This file provides guidance to Claude Code when working with this Unity-based tactical RPG codebase.

## Project Overview

miniRAID is a Unity C# tactical RPG with grid-based combat, featuring a three-level architecture:
1. **ScriptableObject Layer**: Data templates (ActionDataSO, BuffSO, MobDescriptorSO)  
2. **Runtime State Layer**: Game logic (RuntimeAction, Buff, MobData)
3. **Renderer Layer**: Visual presentation (MobRenderer, Indicators)

## Key Systems

### Core Components
- **Databackend** (`Assets/Scripts/Backend/Databackend.cs`): Central game state
- **MobData** (`Assets/Scripts/Backend/MobData/MobData.cs`): Units with Vector3 positions
- **MapSystem** (`Assets/Scripts/Backend/Map/MapSystem.cs`): 32³ chunk-based 3D terrain
- **BackendState** (`Assets/Scripts/Backend/BackendCore/BackendState.cs`): Base for all persistent state

### Architecture Patterns
- **Template-Runtime**: SOs are immutable templates → runtime objects with calculated stats
- **Event-Driven**: Extensive C# events for combat, stats, turns, actions
- **Separation**: Backend logic separate from presentation/rendering
- **Serialization**: Odin Serializer with SO addressable paths, renderers recreate on load

## Development

### Version Control
- You (claude) works in your own, seperated repo.
- Create a branch when implement things. Commit before if there are unstaged changes.
- When done, ask the user to check the code and let the user merge into their branch.

### General
- Unity is not available, so there are no way for claude to test and debug. Therefore, only need to write the code.
- Double-check your code until you are confident that it will work; DO NOT create extra "safe-net" code due to this.
- Write only C# scripts, sometimes UXML things, DO NOT create assets and meta files.
- Implement the core logic and tell the user what they should do to use the code. e.g., create assets.

### Common Tasks
1. **New Actions**: Inherit `ActionDataSO<T>`, implement `OnPerform`, create asset
2. **New Buffs**: Implement `BuffSO` + `Buff` classes, subscribe to MobData events  
3. **Backend States**: Inherit `BackendState`, implement `IRenderableState` if visual
4. **Map Editing**: Use `miniRAID > Map Editor` with 3D raycast selection

### Code Conventions
- `Globals.backend` for main data access, `Globals.cc` for coroutines
- Use `[NonSerialized]` for calculated values and renderer references
- Always unsubscribe from events in `OnRemove()` methods
- Vector3 for mob positions, Vector3Int for grid coordinates
- Method naming: Follow existing patterns (`Wrap()`, `Create()`, `Apply()`, etc.)
- Self-interaction prevention: Check `info.source == mob` in damage/heal listeners
- Use `Destroy()` for buff cleanup, not `RemoveFromMob()`

### Common Pitfalls & Solutions
- **Infinite self-damage loops**: Always check `if (info.source == mob) yield break;` in damage listeners
- **Memory leaks**: Track associated objects in collections for proper cleanup (`List<TurnSliceBuff>`)
- **UI inconsistency**: Hide terminated states immediately (`ShowInUI => showInUI && (!muted)`)
- **Method naming**: Use established patterns - `WrapTurnSliceBuff()` not `CreateTurnSliceBuff()`

## Key Files
- **`Assets/Scripts/Utils/Globals.cs`**: Central singleton manager
- **`Assets/Scripts/Backend/BackendCore/`**: Core infrastructure
- **`Assets/Scripts/Backend/Map/`**: 3D chunk system with MapRenderer/MapEditor
- **`Assets/Scripts/Presentation/`**: Renderer implementations
- **`Manual/Claude/Three-Level-Architecture.md`**: Detailed architecture docs

## Important Notes
- Backend logic must be separate from presentation
- Use `IGridCollider` for collision detection
- Movement system integrates with map pathfinding
- Always test with Combat/Mob monitors for debugging

# Motto
Remember, as an experienced engineer, think harder before you code. Follow these steps:
1. Make a plan first. Read through the codebase and understand the flow of related logics.
2. Do not proceed. First, discuss the plan with the user.
3. After discussion, Implement based on the plan.
4. Ask the user to check the code before commiting.

Follow the code style of existing code, and find examples in the existing codebase.
Most of the time, you can find some examples to help you write cleaner, such as actions and buffs.

**USE Git and CREATE A NEW BRANCH when you implementing things!**
Leave a shorter commit message if implementation only contains some new actions / buffs etc.
Do not initialize serialized fields in constructor. Instead, tell the user to set them up in Inspector.
