using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;
using miniRAID.Actions;
using miniRAID.Spells;
using miniRAID.Weapon;

namespace miniRAID
{
    public class BattlefieldPray : BasicProjectile
    {
        public override RuntimeAction<SingleMobTarget> LeveledWrap(MobData source, int level)
        {
            return new WrappedChargedAction<SingleMobTarget>(
                source,
                (mob) => (mob.mainWeapon as HeavyWeapon)?.RchargedAttack,
                this,
                level);
        }
    }

    public class WrappedChargedAction<TSpellTargetBase> : RuntimeAction<TSpellTargetBase>
        where TSpellTargetBase : SingleMobTarget
    {
        private System.Func<MobData, RuntimeAction> _getChargedRact;
        public RuntimeAction ChargedAttack => _getChargedRact(parentMob);
        
        private bool beingRequested = false;
        
        public WrappedChargedAction(
            MobData source,
            System.Func<MobData, RuntimeAction> getChargedRAct,
            ActionDataSO<TSpellTargetBase> data, int level) : base(source, data, level)
        {
            _getChargedRact = getChargedRAct;
        }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);
            mob.OnActionPrecast.AddListener(TryAppendBaseAction);
        }

        public override void OnRemove(MobData mob)
        {
            mob.OnActionPrecast.RemoveListener(TryAppendBaseAction);
            base.OnRemove(mob);
        }

        public override IEnumerator RequestInUI(MobData mob)
        {
            if (ChargedAttack == null)
            {
                yield break;
            }
            
            beingRequested = true;
            yield return new JumpIn(ChargedAttack.RequestInUI(mob));
            beingRequested = false;
        }
        
        public IEnumerator TryAppendBaseAction(MobData source, RuntimeAction ract, SpellTarget target)
        {
            if (beingRequested && ract == ChargedAttack)
            {
                yield return new JumpIn(
                    source.DoActionWithDefaultCosts<TSpellTargetBase>(
                        this, 
                        new SingleMobTarget(source) as TSpellTargetBase)
                );
            }
        }
    }
}
