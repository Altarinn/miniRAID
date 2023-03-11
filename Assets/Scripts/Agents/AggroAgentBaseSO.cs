using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace miniRAID.Agents
{
    [CreateAssetMenu(menuName = "Agents/AggroAgentBase")]
    public class AggroAgentBaseSO : MobAgentBaseSO
    {
        [Tooltip("视野范围，不会向超出范围的敌对目标移动。目前视距受移动种类影响，如飞行单位视野穿山但步行单位看不到山对面。")]
        public int eyesight = 10;

        public override MobListener Wrap(MobData parent)
        {
            return new AggroAgentBase(parent, this);
        }
    }

    public class AggroAgentBase : MobAgentBase
    {
        protected Dictionary<Mob, float> aggroList;
        public new AggroAgentBaseSO data;

        public Mob currentTarget;
        public bool useAggro = true;

        public AggroAgentBase(MobData parent, AggroAgentBaseSO data) : base(parent, data)
        {
            this.data = data;
        }

        public override void OnAttach(Mob mob)
        {
            base.OnAttach(mob);

            mob.OnReceiveDamageFinal += Mob_OnReceiveDamageFinal;
            mob.OnActionPostcast += Mob_OnActionPostcast;
            aggroList = new Dictionary<Mob, float>();
        }

        private void Mob_OnReceiveDamageFinal(Mob mob, Consts.DamageHeal_Result info)
        {
            if (info.source.data.unitGroup == mob.data.unitGroup) { return; }
            AddToAggro(info.source, info.value * info.source.data.aggroMul);
        }

        private void TargetMob_OnReceiveHealFinal(Mob mob, Consts.DamageHeal_Result info)
        {
            if (info.source.data.unitGroup == mob.data.unitGroup) { return; }
            AddToAggro(info.source, info.value * info.source.data.aggroMul * 2.0f);
        }

        protected void AddToAggro(Mob mob, float aggro)
        {
            // Add to aggrolist if not exist
            if (!aggroList.ContainsKey(mob))
            {
                aggroList.Add(mob, 0);
                mob.OnReceiveHealFinal += TargetMob_OnReceiveHealFinal;
                // TODO: Buffs
            }

            aggroList[mob] += aggro;
            UpdateAggro();
        }

        protected void RemoveFromAggro(Mob mob)
        {
            if (aggroList.ContainsKey(mob))
            {
                aggroList.Remove(mob);
                mob.OnReceiveHealFinal -= TargetMob_OnReceiveHealFinal;
                // TODO: Buffs
            }
            UpdateAggro();
        }

        public void UpdateAggro()
        {
            if (useAggro)
            {
                // Find our target first.
                // Target may die due to previous action, so we may need to find a new one.
                Mob target = null;
                float maxAggro = 0;

                List<Mob> shouldRemove = new List<Mob>();

                foreach (var entry in aggroList)
                {
                    // Remove from aggroList if aggro too small
                    // TODO: Check if dead or out of range
                    if (aggroList[entry.Key] <= 1.0)
                    {
                        shouldRemove.Add(entry.Key);
                        continue;
                    }

                    if (target == null) { target = entry.Key; }
                    if (entry.Value > maxAggro)
                    {
                        target = entry.Key;
                        maxAggro = entry.Value;
                    }
                }

                foreach (var item in shouldRemove)
                {
                    RemoveFromAggro(item);
                }


                currentTarget = target;
            }
        }

        public IEnumerator Mob_OnActionPostcast(Mob mob, RuntimeAction ra, Spells.SpellTarget target)
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

        public IEnumerator Act(Mob mob)
        {
            foreach (var entry in aggroList.Keys.ToList())
            {
                // Decrease aggro by some constant per turn
                aggroList[entry] *= 0.9f;
            }
            UpdateAggro();

            shouldStop = false;
            Mob target = currentTarget;

            while (!shouldStop)
            {
                shouldStop = true;

                // No target, break the loop
                // TODO: OnNoTarget()
                if (target != null)
                {
                    // TODO: Find enemy and move
                    // Behaviour: pick an attack => If in range then attack => Move towards to target by search a path otherwise

                    // TODO: Pick advanced attack / OnPickAction()
                    RuntimeAction pickedSpell = mob.data.mainWeapon.GetRegularAttackSpell();
                    var sTarget = new Spells.SpellTarget(target.data.Position);

                    // TODO: Move, Add inRange check in CheckWithTargets, etc.
                    if (pickedSpell.data.CheckWithTargets(mob, sTarget))
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
                        GridPath path = Globals.backend.FindPathTo(mob.data.Position, Globals.backend.FindNearestEmptyGrid(target.data.Position, mob.gridBody), mob.data.movementType, data.eyesight);

                        // The path needs to be at least 2 grids long
                        if (path.path.Count >= 2)
                        {
                            //throw new System.NotImplementedException();
                            // TODO: Make agent use coroutine actions.
                            pickedAction = mob.GetAction("Move");
                            yield return new JumpIn(mob.DoActionWithDefaultCosts(
                                pickedAction,
                                new Spells.SpellTarget(path.path[1])
                            ));

                            // Move do not have AP costs
                            yield return new JumpIn(mob.TryAutoEndTurn());
                            if(mob.data.isActive == false)
                            {
                                shouldStop = true;
                            }
                        }
                    }
                }
            }

            if (shouldStop && (!mob.enemyDebug))
            {
                yield return new JumpIn(mob.SetActive(false));
            }

            yield break;
        }

        protected override IEnumerator OnAgentWakeUp(Mob mob)
        {
            yield return new JumpIn(Act(mob));
        }
    }
}
