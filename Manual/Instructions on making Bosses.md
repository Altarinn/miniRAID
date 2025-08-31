## Turn Scheduling
### Fixed schedule
e.g., Use X action every N turns.
![[Pasted image 20250831172057.png]]
1. Use `MobBasedTurnGeneratorWithClaimableSlice` as the main scheduler
2. Insert corresponding `MobTurnSlice`s into the schedule
	- `TargetedAgentBaseSO` - For aggro-based controls: movement, main weapon regular attack
		- Can enable `DoWakeUp` or disable `DoPass` on this slice
	- `AutoUseSingleMobActionTurnSliceSO` - Perform specific actions
![[Pasted image 20250831172331.png]]
3. Use `ClaimMobSliceRootAgentSO` as the root agent of boss mob
4. Place corresponding slices inside `Target Slices` list

### Preparable Actions
1. Create a script inherting `ClaimingPreparableTurnSliceSO`, preferably, name it as "XXXBrain". e.g.:
```csharp
public class AlphaWolfP1Brain : ClaimingPreparableTurnSliceSO  
{  
	// Action ingredients
    public SimpleUseFourDirectionalActionTurnSliceSO clawTurn;  
    public ActionSOEntry<ActionDataSO<FourDirectionalTarget>> clawActionEntry; 
    
    public override IEnumerator Turn(  
        TurnSlice _slice, CombatSchedulerCoroutine coroutine)  
    {
        var slice = (MobTurnSlice)_slice;  

		// Obtain runtimeAction from mob
        var clawAction =   
            slice.mob.GetAction(clawActionEntry.data) ??   
            slice.mob.AddAction(clawActionEntry);  

		// Claim turns to use the action
        TryClaimNextTurn(clawTurn, slice.mob, clawAction);  

        yield break;  
    }
}
```
- Targets will be picked in the constructor of corresponding turn slice. (In this case, inside `TryClaimNextTurn`'s `Wrap`.)