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
                  from.data,
                  dNumber.CreateComposite(from.power.Value, "copied"),
                  dNumber.CreateComposite(from.auxPower.Value, "copied")
              )
        {
            // Don't create active mobs, entity & register to backend
        }

        public void Extend(Vector2Int pos)
        {
            Globals.backend.AddFxAt(this, pos);
            entity.AddGrid(pos);
        }

        public void Fx_OnNextTurn(Mob mob)
        {
            timeRemain--;
            if(timeRemain <= 0)
            {
                Globals.backend.RemoveFx(this);
            }
        }

        public void RegisterMob(Mob mob)
        {
            if (activeMobs.ContainsKey(mob.data) || !Consts.ApplyMask(mask, mob.data.unitGroup))
            {
                return;
            }
            var copied = new GridEffect(this);
            mob.ReceiveBuff(copied);
            activeMobs.Add(mob.data, copied);
        }

        public void RemoveMob(Mob mob)
        {
            if (activeMobs.ContainsKey(mob.data))
            {
                mob.RemoveListener(activeMobs[mob.data]);
                activeMobs.Remove(mob.data);
            }
        }
    }
}
