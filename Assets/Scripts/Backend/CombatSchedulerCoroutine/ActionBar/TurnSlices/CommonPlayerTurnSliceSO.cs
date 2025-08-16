using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace miniRAID.TurnSchedule
{
    public class CommonPlayerTurnSliceSO : TurnSliceSO
    {
        HashSet<MobData> awaitForActions = new HashSet<MobData>();
        public string message = null;

        public Consts.UnitGroup group = Consts.UnitGroup.Player;
        
        public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
        {
            // yield break;
            Globals.logger?.Log($"[csc] TURN START: {group.ToString()}");
            
            // Enter phase
            awaitForActions.Clear();
            // currentPhase = group;
            
            // Add them to AwaitForActions first
            foreach (MobData mob in Databackend.GetSingleton().allMobs)
            {
                // mob.OnNewPhase();
                if (mob.unitGroup == group)
                {
                    awaitForActions.Add(mob);
                }
            }
            
            if (message != null)
            {
                Globals.ui.Instance.combatView.ShowCenterTitle(message);
                // Globals.ui.Instance.combatView.schedulerPlaceholder.text = message;
                yield return new WaitForSeconds(0.4f);
                Globals.ui.Instance.combatView.HideCenterTitle();
            }
            
            // yield return new JumpIn(NotifyNewTurn(awaitForActions));
            
            // Player can control
            if(group == Consts.UnitGroup.Player)
            {
                yield return new JumpIn(coroutine.UIWaitPlayerInput());
                yield return new JumpIn(NotifyNewTurn(Databackend.GetSingleton().allMobs));
            }
            else if(group == Consts.UnitGroup.Ally || group == Consts.UnitGroup.Enemy)
            {
                // TODO: Priorities?
                //foreach (Mob mob in awaitForActions)
                //{
                //    yield return new JumpIn(mob)
                //}
            }
            else
            {
                
            }
            
            // TODO: Do something to end the phase!
            // while (awaitForActions.Any(m => m.isControllable))
            while(coroutine.turnEnd == false)
            {
                yield return null;
            }
            
            // End phase. Do something?
        }
        
        public IEnumerator NotifyNewTurn(IEnumerable<MobData> mobs)
        {
            // Trigger all OnNextTurn events
            // AwaitForActions may be modified during following processes
            foreach (var mob in mobs.ToList())
            {
                yield return new JumpIn(mob._OnNextTurn());
            }
        }
        
        public override TurnSlice Wrap(TurnSliceMetadata metadata)
        {
            return new LockedPlayerTurnSlice(this, metadata);
        }
    }
    
    public class LockedPlayerTurnSlice : TurnSlice
    {
        private MobData lockedToPlayer = null;
        private bool isLocked = false;
        
        private Consts.UnitGroup group => ((CommonPlayerTurnSliceSO)data).group;
        
        public LockedPlayerTurnSlice(AbstractTurnSliceSO data, TurnSliceMetadata metadata) : base(data, metadata)
        {
        }
        
        public override IEnumerator Turn()
        {
            // Reset locking state for new turn slice
            ResetLockState();
            
            // Subscribe to global action events to handle turn slice locking
            Globals.backend.onGlobalActionPostcast.AddListener(OnGlobalActionPostcast);
            
            try
            {
                // Call the original turn logic
                yield return new JumpIn(base.Turn());
            }
            finally
            {
                // Always unsubscribe from events when turn slice ends
                Globals.backend.onGlobalActionPostcast.RemoveListener(OnGlobalActionPostcast);
            }
        }
        
        // Turn slice locking management methods
        public void LockToPlayer(MobData player)
        {
            if (player?.unitGroup == group)
            {
                lockedToPlayer = player;
                isLocked = true;
                Globals.logger?.Log($"[TurnSlice] Locked to player: {player.nickname}");
            }
        }
        
        public void ResetLockState()
        {
            lockedToPlayer = null;
            isLocked = false;
            Globals.logger?.Log($"[TurnSlice] Lock state reset");
        }
        
        public bool IsLocked => isLocked;
        public MobData LockedPlayer => lockedToPlayer;
        
        public bool CanPlayerAct(MobData player)
        {
            if (player.unitGroup != group) return false;
            if (!isLocked) return true;
            return player == lockedToPlayer;
        }
        
        
        private IEnumerator OnGlobalActionPostcast(MobData mob, RuntimeAction action, Spells.SpellTarget target)
        {
            // Only lock if this is during our player turn slice and it's a player action for our group
            if (mob.unitGroup == group && !isLocked)
            {
                // Lock the turn slice to this player when they perform their first action
                LockToPlayer(mob);
            }
            yield break;
        }
    }
}