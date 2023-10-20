using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using miniRAID.Actions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

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

    /*
     * This agent will repeatedly use regular attack from equipped main weapon, towards current max aggro enemy.
     * If the regular attack is illegal (e.g., out of range, etc.), it will move towards current target instead.
     * It will never stop until the mob run out of AP, cannot do anything, or as specified in the variable `maxActionPerTurn`.
     */
    public class AggroAgentBase : MobAgentBase
    {
        protected Dictionary<MobData, float> aggroList;
        public float maxAggro => aggroList.Max(kv => kv.Value);
        public AggroAgentBaseSO aggroAgentData => (AggroAgentBaseSO)data;

        public MobData currentTarget;
        public bool useAggro = true;

        public AggroAgentBase(MobData parent, AggroAgentBaseSO data) : base(parent, data)
        {
        }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnDamageReceived += Mob_OnReceiveDamageFinal;
            mob.OnActionPostcast += Mob_OnActionPostcast;
            aggroList = new Dictionary<MobData, float>();
        }

        public override void OnRemove(MobData mob)
        {
            base.OnRemove(mob);

            mob.OnDamageReceived -= Mob_OnReceiveDamageFinal;
            mob.OnActionPostcast -= Mob_OnActionPostcast;

            foreach (MobData targetMob in aggroList.Keys)
            {
                targetMob.OnHealReceived -= TargetMob_OnReceiveHealFinal;
            }
        }
        
        public void SetRandomAggro(float magnitude)
        {
            aggroList.Clear();
            foreach (var mob in Globals.backend.GetAllMobs())
            {
                if (Consts.ApplyMask(Consts.EnemyMask(parentMob.unitGroup), mob.unitGroup))
                {
                    aggroList.Add(mob, Globals.cc.rng.NextFloat() * magnitude);
                }
            }
        }

        private void Mob_OnReceiveDamageFinal(MobData mob, Consts.DamageHeal_Result info)
        {
            if (info.source.unitGroup == mob.unitGroup) { return; }
            AddToAggro(info.source, info.value * info.source.aggroMul);
        }

        private void TargetMob_OnReceiveHealFinal(MobData mob, Consts.DamageHeal_Result info)
        {
            // TODO: Why?
            if (parentMob == null || info.source == null) { return; }
            
            if (info.source.unitGroup == parentMob.unitGroup) { return; }
            AddToAggro(info.source, info.value * info.source.aggroMul * Consts.HealAggroMul);
        }

        protected void AddToAggro(MobData mob, float aggro)
        {
            // Add to aggrolist if not exist
            if (!aggroList.ContainsKey(mob))
            {
                aggroList.Add(mob, 0);
                mob.OnHealReceived += TargetMob_OnReceiveHealFinal;
                // TODO: Buffs
            }

            aggroList[mob] += aggro;
            
            // Check if aggro exceeded the threshold
            if (mob != currentTarget)
            {
                if (aggroList[mob] >= maxAggro)
                {
                    Globals.popupMgr.Instance.Popup(">TARGET<", Globals.backend.GridToWorldPosCentered(mob.Position));
                    Globals.popupMgr.Instance.Popup("ATTACKING YOU", Globals.backend.GridToWorldPosCentered(parentMob.Position));
                }
                else if (aggroList[mob] >= maxAggro * Settings.highAggroThreshold)
                {
                    Globals.popupMgr.Instance.Popup("!", Globals.backend.GridToWorldPosCentered(mob.Position));
                    Globals.popupMgr.Instance.Popup("HIGH AGGRO", Globals.backend.GridToWorldPosCentered(parentMob.Position));
                }
            }
            
            UpdateAggro();
        }

        protected void RemoveFromAggro(MobData mob)
        {
            if (aggroList.ContainsKey(mob))
            {
                aggroList.Remove(mob);
                mob.OnHealReceived -= TargetMob_OnReceiveHealFinal;
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
                MobData target = null;
                float maxAggro = 0;

                List<MobData> shouldRemove = new List<MobData>();

                foreach (var entry in aggroList)
                {
                    // Remove from aggroList if aggro too small
                    // TODO: Check if dead or out of range
                    if (entry.Key == null || entry.Key.isDead)
                    {
                        shouldRemove.Add(entry.Key);
                        continue;
                    }
                    
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

        public IEnumerator Act(MobData mob)
        {
            actionCounter = 0;
            GridPath path = null;
            
            foreach (var entry in aggroList.Keys.ToList())
            {
                // Decrease aggro by some constant per turn
                aggroList[entry] *= 0.9f;
            }
            UpdateAggro();

            shouldStop = false;
            MobData target = currentTarget;

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
                    RuntimeAction pickedSpell = mob.mainWeapon.GetRegularAttackSpell();
                    var sTarget = new Spells.SpellTarget(target.Position);

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
                        path ??= Globals.backend.FindPathTo(mob.Position, Globals.backend.FindNearestEmptyGrid(target.Position, mob.gridBody), mob.movementType, aggroAgentData.eyesight);
                        
                        // TODO: FIXME: This is a dirty patch so the mob won't get stuck when it cannot find a valid path.
                        // path ??= Globals.backend.FindPathTo(mob.Position, Globals.backend.FindNearestEmptyGrid(target.Position, mob.gridBody), MobData.MovementType.Fly, aggroAgentData.eyesight);

                        // The path needs to be at least 1 grids long
                        if (path != null && path.path.Count >= 2)
                        {
                            //throw new System.NotImplementedException();
                            // TODO: Make agent use coroutine actions.
                            pickedAction = mob.GetActionFromSO<Movement>();
                            yield return new JumpIn(mob.DoActionWithDefaultCosts(
                                pickedAction,
                                new Spells.SpellTarget(path.path[0])
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

            yield break;
        }

        protected override IEnumerator OnAgentWakeUp(MobData mob)
        {
            if(!mob.isControllable) { yield break; }
            
            yield return new JumpIn(Act(mob));
        }

        public struct AggroInfo
        {
            [DisplayAsString]
            public string nickname;

            [DisplayAsString]
            public float aggro;
        }
        
        public List<AggroInfo> GetAggroListUtil()
        {
            return aggroList.Select(kv => new KeyValuePair<string, float>(kv.Key.nickname, kv.Value))
                .OrderByDescending(kv => kv.Value)
                .Select(kv => new AggroInfo() { nickname = kv.Key, aggro = kv.Value })
                .ToList();
        }

        public void SetAsMaxAggro(MobData target, float margin)
        {
            aggroList[target] = 0;
            AddToAggro(target, maxAggro + margin);
        }
    }
}
