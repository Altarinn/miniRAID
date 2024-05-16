using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using miniRAID.Actions;
using miniRAID.Spells;
using miniRAID.TurnSchedule;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace miniRAID.Agents
{
    [CreateAssetMenu(menuName = "Agents/AggroAgentBase")]
    public class AggroAgentBaseSO : MobTurnSliceBaseSO
    {
        [Tooltip("视野范围，不会向超出范围的敌对目标移动。目前视距受移动种类影响，如飞行单位视野穿山但步行单位看不到山对面。")]
        public int eyesight = 10;
        
        [Header("User Interface")] public Material decalIndicatorMaterial;

        public override MobTurnSlice Wrap(MobData mob, TurnSliceMetadata metadata)
        {
            return new AggroAgentBase(mob, this, metadata);
        }

        public override IEnumerator Turn(TurnSlice slice, CombatSchedulerCoroutine coroutine)
        {
            throw new NotImplementedException(
                "Please implement Turn() in AggroAgentBase (TurnSlice) instead of AggroAgentBaseSO (TurnSliceSO).");
        }
    }

    /*
     * This agent will repeatedly use regular attack from equipped main weapon, towards current max aggro enemy.
     * If the regular attack is illegal (e.g., out of range, etc.), it will move towards current target instead.
     * It will never stop until the mob run out of AP, cannot do anything, or as specified in the variable `maxActionPerTurn`.
     */
    public class AggroAgentBase : MobTurnSlice
    {
        public AggroAgentBaseSO aggroAgentData => (AggroAgentBaseSO)data;

        public AggroCollector mobAggro;

        public AggroAgentBase(MobData mob, AggroAgentBaseSO data, TurnSliceMetadata metadata) : base(mob, data, metadata)
        {
            mobAggro = mob.FindListener<AggroCollector>();
        }

        public void OnBeginSlice()
        {
            mob.OnActionPostcast += Mob_OnActionPostcast;
        }

        public void OnEndSlice()
        {
            mob.OnActionPostcast -= Mob_OnActionPostcast;
        }

        public IEnumerator Mob_OnActionPostcast(MobData mob, RuntimeAction ra, Spells.SpellTarget target)
        {
            if(ra == pickedAction)
            {
                shouldStop = false;
            }
            yield break;
        }

        // Used to check if we need to stop performing actions
        // Usually when we don't have enough resources, but don't need to move (within attack range) then we pass.
        // If the action is performed sucessfully, shouldStop will be set to false so we can try more actions with points available.
        bool shouldStop = true;
        RuntimeAction pickedAction;

        private int actionCounter = 0, maxActionPerTurn = 3;

        public override IEnumerator Turn()
        {
            if(!mob.isControllable) { yield break; }
            
            OnBeginSlice();
            
            actionCounter = 0;
            GridPath path = null;
            
            shouldStop = false;
            MobData target = mobAggro.CurrentTarget;

            if (target == null)
            {
                shouldStop = true;
            }
            
            var movementAction = (mob.GetActionFromSO<Movement>() as RuntimeAction<SingleCoordinateTarget>);

            while (!shouldStop)
            {
                shouldStop = true;

                // No target, break the loop
                // TODO: OnNoTarget()
                if (target == null) { break; }

                // Obtain some actions for later use
                // Currently only handles SingleMobTarget actions
                // GetRegularAttackSpell() may give different results after casting a regular attack
                // So we need to query it every time
                RuntimeAction<SingleMobTarget> pickedSpell = mob.mainWeapon.GetRegularAttackSpell()
                    as RuntimeAction<SingleMobTarget>;
                
                if (pickedSpell == null)
                {
                    Debug.LogWarning("AggroAgentBase cannot handle non-SingleMobTarget actions. Action ignored!!");
                    shouldStop = true;
                }
            
                // TODO: Find enemy and move
                // Behaviour: pick an attack => If in range then attack => Move towards to target by search a path otherwise

                // TODO: Pick advanced attack / OnPickAction()
                var sTarget = new SingleMobTarget(target);

                // TODO: Move, Add inRange check in CheckWithTargets, etc.
                if (pickedSpell.actionData.CheckWithTargets(mob, sTarget))
                {
                    // TODO: Make agent use coroutine actions
                    // TODO: Cost
                    pickedAction = pickedSpell;
                    yield return new JumpIn(mob.DoActionWithDefaultCosts(pickedSpell, sTarget));
                }
                else
                {
                    // Do we really need to re-calculate the path everytime?
                    // Will the map change during our action? could be possible though ...
                    // TODO: Cache the path in some way in case of performance problems
                    path ??= Globals.backend.FindPathTo(mob.Position, Globals.backend.FindNearestEmptyGrid(target.Position, mob.gridBody), mob.movementType, aggroAgentData.eyesight);
                    
                    // TODO: FIXME: This is a dirty patch so the mob won't get stuck when it cannot find a valid path.
                    // path ??= Globals.backend.FindPathTo(mob.Position, Globals.backend.FindNearestEmptyGrid(target.Position, mob.gridBody), MobData.MovementType.Fly, aggroAgentData.eyesight);

                    // The path needs to be at least 1 grids long
                    if (path != null && path.path.Count >= 2)
                    {
                        //throw new System.NotImplementedException();
                        // TODO: Make agent use coroutine actions.

                        // We don't know how to move and we cannot reach the target, we screwed
                        if (movementAction == null) { break; }

                        // If we know how to move then move
                        pickedAction = movementAction;
                        yield return new JumpIn(mob.DoActionWithDefaultCosts(
                            movementAction,
                            new SingleCoordinateTarget(path.path[0])
                        ));
                        path.Step();

                        // Move do not have AP costs
                        yield return new JumpIn(mob.TryAutoEndTurn());
                        if(mob.isActive == false)
                        {
                            shouldStop = true;
                        }
                    }
                }
                
                actionCounter += 1;
                if (actionCounter >= maxActionPerTurn)
                {
                    shouldStop = true;
                }
            }

            if (shouldStop && (!mob.enemyDebug))
            {
                yield return new JumpIn(mob.SetActive(false));
            }

            OnEndSlice();
            
            yield break;
        }
    }
}
