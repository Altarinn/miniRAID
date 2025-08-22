using UnityEngine;
using System.Collections;
using System.Linq;
using miniRAID.ActionHelpers;
using miniRAID.Backend.Numericals;
using miniRAID.Spells;

namespace miniRAID.UI.TargetRequester
{
    public class BasicUnitRequester : TargetRequesterBase<SingleMobTarget>
    {
        protected int MaxUnits = 1;
        protected int MinUnits = 1;

        [PathBell]
        public int range = 3;

        public GridOverlay.Types enemyType = GridOverlay.Types.ATTACK;
        public GridOverlay.Types allyType = GridOverlay.Types.HEAL;

        public bool toEnemies = true;
        public bool toAllies = false;

        public override RequestStage Next(Vector3Int coord, bool notFirst = true)
        {
            // TODO: Support multiple targets with MultiMobTarget
            // if(currentStageCompleted >= MaxUnits)
            if(currentStageCompleted >= 1)
            {
                Decided();
                return null;
            }

            // if (currentStageCompleted >= MinUnits)
            if (currentStageCompleted >= 1)
            {
                // TODO: show confirm screen
            }

            RequestStage stage = new RequestStage();
            stage.type = RequestType.Target;
            
            // Handle grids with enemy
            if (toEnemies)
            {
                int mask = Consts.EnemyMask(mob.unitGroup);

                var validGrids = Globals.backend.GetGridsWithMob(
                    (Databackend.IsMobValidFunc)((MobData mob) => Consts.ApplyMask(mask, mob.unitGroup)),
                    (Databackend.IsGridValidFunc)((Vector3Int pos, GridData grid) 
                        => ((!choice.Contains(pos)) 
                            && (Consts.RangedActionDistance(mob.Position, pos) <= range)
                            && Globals.backend.HasLineOfSight(mob.SpellCastPivot, pos + Vector3.one * 0.5f))
                    ));

                foreach (var pos in validGrids)
                {
                    stage.map.Add(pos, enemyType);
                }
            }

            // Handle grids with ally
            if(toAllies)
            {
                int mask = Consts.AllyMask(mob.unitGroup);

                var validGrids = Globals.backend.GetGridsWithMob(
                    (Databackend.IsMobValidFunc)((MobData mob) => Consts.ApplyMask(mask, mob.unitGroup)),
                    (Databackend.IsGridValidFunc)((Vector3Int pos, GridData grid) 
                        => ((!choice.Contains(pos)) 
                            && (Consts.RangedActionDistance(mob.Position, pos) <= range)
                            && Globals.backend.HasLineOfSight(mob.SpellCastPivot, pos + Vector3.one * 0.5f))
                    ));

                foreach (var pos in validGrids)
                {
                    stage.map.Add(pos, allyType);
                }
            }

            // Already selected grids
            foreach (var grid in choice)
            {
                stage.map.Add(grid, GridOverlay.Types.SELECTED);
            }

            return stage;
        }

        void Decided()
        {
            MobData mob = Essentials.MobAtGrid(choice.First());
            Finish(new SingleMobTarget(mob));
        }

        public override bool CheckTargets(MobData mob, SingleMobTarget target)
        {
            // foreach (var pos in target.targetPos)
            // {
            //     if(
            //         Globals.backend.Distance(mob.Position, pos) > range ||
            //         Globals.backend.GetMap(pos).mob == null ||
            //         (!toEnemies && Consts.ApplyMask(Consts.EnemyMask(mob.unitGroup), Globals.backend.GetMap(pos).mob.unitGroup)) ||
            //         (!toAllies && Consts.ApplyMask(Consts.AllyMask(mob.unitGroup), Globals.backend.GetMap(pos).mob.unitGroup)))
            //     {
            //         return false;
            //     }
            // }
            
            if(
                Consts.RangedActionDistance(mob.Position, target.Target.Position) > range ||
                (!toEnemies && Consts.ApplyMask(Consts.EnemyMask(mob.unitGroup), target.Target.unitGroup)) ||
                (!toAllies && Consts.ApplyMask(Consts.AllyMask(mob.unitGroup), target.Target.unitGroup)))
            {
                return false;
            }
            return true;
        }

        public override void PointAtGrid(Vector3Int gridPos)
        {
            base.PointAtGrid(gridPos);

            ui.combatView.HideBattlePreview();

            var pointedMob = Globals.backend.GetMap(gridPos)?.mob;
            if (pointedMob == null) { return; }

            // TODO: Formalize me and ask ract for proper info
            if(toEnemies && Consts.ApplyMask(Consts.EnemyMask(mob.unitGroup), pointedMob.unitGroup))
            {
                int expectedDamage =
                    Mathf.CeilToInt(Consts.GetDamage(ract.power, mob.level, pointedMob.defense, pointedMob.level));
                int expectedSpDamage =
                    Mathf.CeilToInt(Consts.GetDamage(ract.power, mob.level, pointedMob.spDefense, pointedMob.level));

                int hitrate = Mathf.CeilToInt(Consts.GetHitRate(ract.hit, pointedMob.dodge, pointedMob.level) * 100);
                int critrate =
                    Mathf.CeilToInt(Consts.GetCriticalRate(ract.crit, pointedMob.antiCrit, pointedMob.level) * 100);
                
                ui.combatView.ShowBattlePreview(
                    $"<mspace=0.75em>DMG</mspace>: {expectedDamage} or {expectedSpDamage} (Sp)\n" +
                    $"<mspace=0.75em>HIT</mspace>: {hitrate}\n" +
                    $"<mspace=0.75em>CRT</mspace>: {critrate}");
            }

            if(toAllies && Consts.ApplyMask(Consts.AllyMask(mob.unitGroup), pointedMob.unitGroup))
            {
                ui.combatView.ShowBattlePreview("Ally (Healing?)");
            }
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            ui.combatView.HideBattlePreview();
        }
    }
}
