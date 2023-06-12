using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Collections.Generic;

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

        public MobListener WrapFx(MobData parent, Vector3 position)
        {
            return new GridEffect(parent, this, position);
        }
    }

    public class GridEffect : Buff
    {
        Dictionary<MobData, GridEffect> activeMobs;
        GridEffectComponent entity;
        public new GridEffectSO data;

        int mask;

        bool isFx => activeMobs != null;

        public GridEffect(MobData source, GridEffectSO data, Vector3 position) : base(source, data)
        {
            mask = 0;

            if (data.toAllies) { mask |= Consts.AllyMask(source.unitGroup); }
            if (data.toEnemies) { mask |= Consts.EnemyMask(source.unitGroup); }

            this.data = data;
            activeMobs = new Dictionary<MobData, GridEffect>();
            entity = GameObject.Instantiate(data.prefab.gameObject, position, Quaternion.identity).GetComponent<GridEffectComponent>();
            Globals.backend.AddFx(this);
        }

        public GridEffect(GridEffect from)
            : base(
                  from.source,
                  from.data
              )
        {
            // Don't create active mobs, entity & register to backend
            power = dNumber.CreateComposite(from.power.Value, "copied");
            auxPower = dNumber.CreateComposite(from.auxPower.Value, "copied");
            hit = dNumber.CreateComposite(from.hit.Value, "copied");
            crit = dNumber.CreateComposite(from.crit.Value, "copied");
        }

        public void Extend(Vector3Int pos)
        {
            Globals.backend.AddFxAt(this, pos);
            entity.AddGrid(Globals.backend.GridToWorldPos(pos));
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
                mob.RemoveListener(activeMobs[mob]);
                activeMobs.Remove(mob);
            }
        }
    }
}
