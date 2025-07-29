using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Collections.Generic;
using Sirenix.Serialization;

namespace miniRAID.Buff
{
    public class GridEffectSO : BuffSO
    {
        [PropertyOrder(-1)]
        public GridEffectComponent prefab;

        [PropertyOrder(-1)]
        public bool toAllies = false;

        [PropertyOrder(-1)]
        public bool toEnemies = true;

        public MobListener LeveledWrapFx(MobData parent, int level, GridShape shape)
        {
            return new GridEffect(parent, this, shape);
        }
    }

    public class GridEffect : Buff
    {
        [OdinSerialize] Dictionary<MobData, GridEffect> activeMobs;
        GridEffectComponent entity => (GridEffectComponent)renderer;
        public GridEffectSO gridData => (GridEffectSO)data;

        public GridShape grids;

        public int mask;

        bool isFx => activeMobs != null;

        public GridEffect(MobData source, GridEffectSO data, GridShape shape) : base(source, data)
        {
            mask = 0;

            if (data.toAllies) { mask |= Consts.AllyMask(source.unitGroup); }
            if (data.toEnemies) { mask |= Consts.EnemyMask(source.unitGroup); }

            this.data = data;
            this.grids = shape;
            this.grids.ApplyTransformInplace();
            
            activeMobs = new Dictionary<MobData, GridEffect>();
            Globals.backend.AddFx(this);
        }

        // This is used to copy itself to Mob.
        // Not actually "Clone constructor".
        public GridEffect(GridEffect from)
            : base(
                  from.source,
                  from.gridData
              )
        {
            // Don't create active mobs, entity & register to backend
            power = dNumber.CreateComposite(from.power.Value, "copied");
            auxPower = dNumber.CreateComposite(from.auxPower.Value, "copied");
            hit = dNumber.CreateComposite(from.hit.Value, "copied");
            crit = dNumber.CreateComposite(from.crit.Value, "copied");

            level = from.level;
        }

        public void Extend(Vector3Int pos)
        {
            if (!Globals.backend.InMap(pos))
            {
                return;
            }
            
            grids.AddGrid(pos);
            Globals.backend.AddFxAt(this, pos);
            // entity.AddGrid(Globals.backend.GridToWorldPos(pos));
        }

        public void Fx_OnNextTurn(MobData mob)
        {
            timeRemain--;
            if(timeRemain <= 0)
            {
                Globals.backend.RemoveFx(this);
            }
        }

        public void RegisterMob(MobData mob)
        {
            if (activeMobs.ContainsKey(mob) || !Consts.ApplyMask(mask, mob.unitGroup))
            {
                return;
            }
            var copied = new GridEffect(this);
            mob.AddBuff(copied);
            activeMobs.Add(mob, copied);
        }

        public void RemoveMob(MobData mob)
        {
            if (activeMobs.ContainsKey(mob))
            {
                // TODO: Should remove 1 stack of the buff, perhaps?
                mob.RemoveBuffOnce(activeMobs[mob]);
                activeMobs.Remove(mob);
            }
        }

        public override void ConstructRenderer()
        {
            renderer = GameObject.Instantiate(
                gridData.prefab.gameObject, 
                Globals.backend.GridToWorldPosCenteredGrounded(source.Position), Quaternion.identity).GetComponent<GridEffectComponent>();

            UpdateRenderer();
        }

        public override void UpdateRenderer()
        {
            entity.SetShape(grids?.shape);
        }
    }
}
