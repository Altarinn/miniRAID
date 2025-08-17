using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Collections.Generic;
using miniRAID.Backend;
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

        [PropertyOrder(-1)] public int GridFxTimeMax = 0;

        public GridEffect LeveledWrapFx(MobData parent, int level, IGridCollider shape)
        {
            var buff = (Buff)base.LeveledWrap(parent, level);
            return new GridEffect(parent, this, buff, (IGridCollider)shape.Clone());
        }
    }

    // Contains a buff inside.
    // GridEffect is a collider in scene, registers buff (copy) to all mobs entering it.
    public class GridEffect : MobListener, IRenderableState, IColliderState
    {
        [OdinSerialize] Dictionary<MobData, Buff> activeMobs;
        GridEffectComponent entity => (GridEffectComponent)renderer;

        public int mask;
        public Buff cachedBuff;
        public GridEffectSO gridData => (GridEffectSO)data;

        bool isFx => activeMobs != null;
        public IGridCollider Collider { get => _collider; set => _collider = value; }
        [OdinSerialize] private IGridCollider _collider;

        [SerializeField] private int timeRemain;

        public GridEffect(MobData source, GridEffectSO data, Buff rBuff, IGridCollider shape) : base(source, data)
        {
            mask = 0;

            if (data.toAllies) { mask |= Consts.AllyMask(source.unitGroup); }
            if (data.toEnemies) { mask |= Consts.EnemyMask(source.unitGroup); }

            this.Collider = shape;
            cachedBuff = rBuff;

            timeRemain = -1;
            if (data.GridFxTimeMax > 0)
            {
                timeRemain = data.GridFxTimeMax;
            }
            
            activeMobs = new Dictionary<MobData, Buff>();
            Globals.backend.AddFx(this);
        }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            
            mob.OnNextTurn.AddListener(Fx_OnNextTurn);
        }

        public override void OnRemove(MobData mob)
        {
            mob.OnNextTurn.RemoveListener(Fx_OnNextTurn);
            
            base.OnRemove(mob);
        }

        public IEnumerator Fx_OnNextTurn(MobData mob)
        {
            if (timeRemain > 0)
            {
                timeRemain--;
                if(timeRemain <= 0)
                {
                    Globals.backend.RemoveFx(this);
                }
            }

            yield break;
        }

        public void RegisterMob(MobData mob)
        {
            if (activeMobs.ContainsKey(mob) || !Consts.ApplyMask(mask, mob.unitGroup))
            {
                return;
            }
            var copied = new Buff(this.cachedBuff);
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

        public void ConstructRenderer()
        {
            renderer = GameObject.Instantiate(
                gridData.prefab.gameObject, 
                Globals.backend.BackendToRenderPosCenteredGrounded(Collider.Position), Quaternion.identity).GetComponent<GridEffectComponent>();

            UpdateRenderer();
        }

        public void UpdateRenderer()
        {
            // TODO: Performance heavy?
            entity?.SetShape(new HashSet<Vector3Int>(Collider));
        }

        public void OnEnterCollider(BackendState other)
        {
            if (other is MobData)
            {
                RegisterMob(other as MobData);
            }
        }

        public void OnExitCollider(BackendState other)
        {
            if (other is MobData)
            {
                RemoveMob(other as MobData);
            }
        }
    }
}
