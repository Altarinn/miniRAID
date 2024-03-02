using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace miniRAID.TurnSchedule
{
    public class CommonPlayerTurnSlice : TurnSlice
    {
        HashSet<MobData> awaitForActions = new HashSet<MobData>();
        public string message = null;

        public Consts.UnitGroup group = Consts.UnitGroup.Player;
        
        public override IEnumerator Turn()
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
                yield return new WaitForSeconds(0.5f);
                Globals.ui.Instance.combatView.HideCenterTitle();
            }
            
            yield return new WaitForSeconds(0.3f);
            
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
    }
}