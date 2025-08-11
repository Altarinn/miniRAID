# Character Progression & Demo Development Strategy

*Generated 2025-01-10*

## Executive Summary

**Recommendation: Prioritize Demo First**, then implement inventory/skill tree systems. The current architecture is remarkably forward-compatible and will not require major refactoring when adding progression systems.

## Current System Analysis

### Strengths of Existing Architecture

1. **ScriptableObject Foundation**: All content (skills, equipment, buffs) is SO-based, making it perfect for inventory systems
2. **Runtime Wrapping Pattern**: Template SOs → Runtime instances means adding inventory won't break existing content
3. **Flexible Stat System**: `StatModifierSO` with enum-based targets handles complex equipment bonuses seamlessly
4. **Level-Aware Design**: Both actions and equipment support leveling, translating perfectly to skill trees
5. **Event-Driven Architecture**: Extensive use of C# events supports complex progression mechanics

### Technical Compatibility Assessment

- **Skills remain 100% valid**: `ActionDataSO<T>` system is inventory-agnostic
- **Equipment requires minimal changes**: Just need `MobData.AddEquipment()/RemoveEquipment()` methods
- **Current assets are already structured like inventory**: `GameContent/` folder functions as content database

## Recommended Implementation Strategy

### Phase 1: Demo Development (2-4 weeks)

#### Demo Structure
- **Multiple predefined combat encounters**: Tutorial, Normal, Hard difficulty stages
- **Pre-equipped characters**: Use `BaseMobDescriptorSO` with manually assigned equipment
- **Equipment Presets**: Create composition-based system for different character builds

#### Equipment Preset System (Demo-Only)
```csharp
[CreateAssetMenu(menuName = "Equipment Preset")]
public class EquipmentPresetSO : ScriptableObject 
{
    public string presetName;
    public List<Weapon.WeaponSO> weapons;
    public List<MobListenerSO> equipment;
    public List<ActionSOEntry> bonusSkills;
    
    // Use composition, not SO modification
    public MobData BuildCharacter(BaseMobDescriptorSO baseDescriptor) 
    {
        // Apply base template, then layer preset equipment/skills
        // WITHOUT modifying original SOs
    }
}
```

### Phase 2: Character Progression System (Future)

#### Two-Layer Save Architecture

**Layer 1: Campaign Progress (Persistent between combats)**
```csharp
[System.Serializable]
public class CampaignSaveData 
{
    public Dictionary<string, CharacterProgressionData> characters;
    public int currentStage;
    public Dictionary<string, bool> unlockedStages;
    public List<GlobalEventEffect> globalEvents; // Magic fountain, etc.
}

public class CharacterProgressionData 
{
    public string characterId;
    public int level;
    public List<SkillUnlock> skillUnlockHistory; // CHRONOLOGICALLY ORDERED!
    public List<string> equippedItems;
    public Dictionary<string, int> skillPoints;
    public List<GlobalEventEffect> globalEventEffects;
}
```

**Layer 2: Combat State (Temporary - Current System)**
- Keep existing `SaveDataSerializer` for mid-combat quicksave/load
- Gets cleared when combat ends

#### Character Building Pipeline
```csharp
public static class CharacterBuilder 
{
    public static MobData BuildCharacter(BaseMobDescriptorSO baseDescriptor, CharacterProgressionData progression)
    {
        // 1. Apply base template
        // 2. Apply progression data (level, unlocked skills)
        // 3. Apply global event effects
        // 4. Add equipped items
        // 5. Process skill tree replacements (IN CHRONOLOGICAL ORDER)
    }
}
```

## Global Events Implementation

### Magic Fountain Example
```csharp
public class GlobalEventManager 
{
    public void TriggerGlobalEvent(string eventId, GlobalEventEffect effect)
    {
        var campaign = CampaignSaveSystem.LoadCampaignProgress();
        
        // Apply to ALL characters in party
        foreach (var character in campaign.characters.Values) 
        {
            character.globalEventEffects.Add(effect);
        }
        
        CampaignSaveSystem.SaveCampaignProgress(campaign);
        
        // If in combat, apply immediately to runtime characters
        if (Globals.backend != null) 
        {
            ApplyGlobalEffectToRuntimeCharacters(effect);
        }
    }
}
```

Global events modify **campaign progression data**, not runtime characters directly.

## Map State Persistence (Open World)

### World State System
```csharp
[System.Serializable]
public class WorldStateData 
{
    public Dictionary<string, bool> doorStates; // doorId -> isOpen
    public Dictionary<string, bool> switchStates;
    public List<string> destroyedObjects;
    public Dictionary<Vector3Int, string> placedObjects;
}

public class WorldStateManager 
{
    public void OpenDoor(string doorId, Vector3Int doorPosition)
    {
        // 1. Update world state data
        // 2. Update map chunks (existing MapSystem)
        // 3. Update visuals (MapRenderer)
    }
}
```

Integrates with existing chunk-based `MapSystem` for persistent world changes.

## Critical Technical Decisions

### Skill Tree Replacement Mechanics
**ORDER MATTERS!** Skill replacements must be processed chronologically:

```csharp
public void ApplyProgressionToCharacter(MobData mob, CharacterProgressionData progression)
{
    // CRITICAL: Sort by unlock timestamp
    var orderedUnlocks = progression.skillUnlockHistory
        .OrderBy(unlock => unlock.timestamp)
        .ToList();
        
    foreach (var unlock in orderedUnlocks) 
    {
        // Add new skill, remove replaced skills
        // This is why chronological order is essential
    }
}
```

### Never Modify ScriptableObjects Directly
- Use **composition** and **builder patterns**
- ScriptableObjects are **immutable templates**
- All modifications happen at **runtime character level**

## Integration with Current Systems

### Serialization Strategy
- **Campaign Progress**: Separate system using Odin Serializer
- **Combat State**: Keep existing `SaveDataSerializer` unchanged
- **World State**: Extends existing `MapSystem` chunk persistence

### Migration Path
1. Current demo assets become "starter equipment"
2. Existing combat system remains unchanged
3. Progression system layers on top without breaking existing functionality

## Timeline Expectations

- **Demo (Phase 1)**: 2-4 weeks with Equipment Presets
- **Full Progression System (Phase 2)**: Major feature, timeline TBD
- **Open World Features**: Can be developed independently

## Risk Mitigation

✅ **Technical Risk**: Low - Architecture is highly compatible
✅ **Content Risk**: Low - Existing content remains valid
✅ **Timeline Risk**: Medium - Demo-first approach validates core gameplay before major systems investment

This strategy maximizes learning while minimizing technical debt, ensuring both short-term demo success and long-term architectural sustainability.