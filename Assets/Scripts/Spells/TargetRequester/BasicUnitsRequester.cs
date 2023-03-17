using UnityEngine;
using System.Collections;
using miniRAID.Spells;

namespace miniRAID.UI.TargetRequester
{
    public class BasicUnitsRequester : TargetRequesterBase
    {
        public int MaxUnits = 1;
        public int MinUnits = 1;
        public int range = 3;

        public GridOverlay.Types enemyType = GridOverlay.Types.ATTACK;
        public GridOverlay.Types allyType = GridOverlay.Types.HEAL;

        public bool toEnemies = true;
        public bool toAllies = false;

        public override RequestStage Next(Vector2Int coord, bool notFirst = true)
        {
            if(currentStageCompleted >= MaxUnits)
            {
                Decided();
                return null;
            }

            if (currentStageCompleted >= MinUnits)
            {
                // TODO: show confirm screen
            }

            RequestStage stage = new RequestStage();
            stage.type = RequestType.Target;
            
            // Handle grids with enemy
            if (toEnemies)
            {
                int mask = Consts.EnemyMask(mob.data.unitGroup);

                var validGrids = Globals.backend.GetGridsWithMob(
                    (Databackend.IsMobValidFunc)((Mob mob) => Consts.ApplyMask(mask, mob.data.unitGroup)),
                    (Databackend.IsGridValidFunc)((Vector2Int pos, GridData grid) => ((!choice.Contains(pos)) && (Consts.Distance(mob.data.Position, pos) <= range))));

                foreach (var pos in validGrids)
                {
                    stage.map.Add(pos, enemyType);
                }
            }

            // Handle grids with ally
            if(toAllies)
            {
                int mask = Consts.AllyMask(mob.data.unitGroup);

                var validGrids = Globals.backend.GetGridsWithMob(
                    (Databackend.IsMobValidFunc)((Mob mob) => Consts.ApplyMask(mask, mob.data.unitGroup)),
                    (Databackend.IsGridValidFunc)((Vector2Int pos, GridData grid) => ((!choice.Contains(pos)) && (Consts.Distance(mob.data.Position, pos) <= range))));

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
            Finish(new Spells.SpellTarget(choice));
        }

        public override bool CheckTargets(Mob mob, SpellTarget target)
        {
            foreach (var pos in target.targetPos)
            {
                if(Globals.backend.Distance(mob.data.Position, pos) > range)
                {
                    return false;
                }
            }
            return true;
        }

        public override void PointAtGrid(Vector2Int gridPos)
        {
            base.PointAtGrid(gridPos);

            ui.combatView.HideBattlePreview();

            var pointedMob = Globals.backend.getMap(gridPos.x, gridPos.y)?.mob;
            if (pointedMob == null) { return; }

            if(toEnemies && Consts.ApplyMask(Consts.EnemyMask(mob.data.unitGroup), pointedMob.data.unitGroup))
            {
                ui.combatView.ShowBattlePreview("test");
            }

            if(toAllies && Consts.ApplyMask(Consts.AllyMask(mob.data.unitGroup), pointedMob.data.unitGroup))
            {
                ui.combatView.ShowBattlePreview("test");
            }
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            ui.combatView.HideBattlePreview();
        }
    }
}
