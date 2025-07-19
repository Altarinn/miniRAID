using System.Collections;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.ActionHelpers
{
    public interface IMultiTurnActionBehaviour<TSpellTarget> where TSpellTarget : SpellTarget
    {
        public IEnumerator DoAction(
            RuntimeAction<TSpellTarget> ract,
            int turn, SimpleMultiTurnActionProxy<TSpellTarget> dummy,
            MobData src, TSpellTarget target, object customData);
    }
    
    public class SimpleMultiTurnAction<TSpellTarget> where TSpellTarget : SpellTarget
    {
        private int totalTurns = 1;

        /// <summary>
        /// Turn starts from 1.
        /// </summary>
        public IMultiTurnActionBehaviour<TSpellTarget> behaviour;
        
        public SimpleMultiTurnAction(int turns)
        {
            totalTurns = turns;
        }

        public IEnumerator Do(
            IMultiTurnActionBehaviour<TSpellTarget> behaviour, 
            RuntimeAction<TSpellTarget> ract,
            MobData src, 
            TSpellTarget target, 
            bool startImmediately = false, 
            object customData = null)
        {
            var dummyData = ScriptableObject.CreateInstance<NullListenerSO>();
            var listener = new SimpleMultiTurnActionProxy<TSpellTarget>(
                src, ract, dummyData, totalTurns, target, behaviour, customData);
            
            src.AddListener(listener);

            if (startImmediately)
            {
                yield return new JumpIn(listener.Do(src));
            }
        }
    }

    public class SimpleMultiTurnActionProxy<TSpellTarget> : MobListener where TSpellTarget : SpellTarget
    {
        public int turn, maxTurn;
        
        [SerializeField] private TSpellTarget target;
        [SerializeField] private RuntimeAction<TSpellTarget> ract;
        public IMultiTurnActionBehaviour<TSpellTarget> behaviour;
        [SerializeField] private object customData;
        
        public SimpleMultiTurnActionProxy(
            MobData parent, 
            RuntimeAction<TSpellTarget> ract,
            MobListenerSO data,
            int maxTurn,
            TSpellTarget target,
            IMultiTurnActionBehaviour<TSpellTarget> behaviour, 
            object customData = null) : base(parent, data)
        {
            this.target = target;
            this.behaviour = behaviour;
            this.customData = customData;
            this.ract = ract;

            this.maxTurn = maxTurn;
            this.turn = 0;
        }

        public override void OnAttach(MobData mob)
        {
            base.OnAttach(mob);

            mob.OnWakeup.AddListener(Do);
        }

        public override void OnRemove(MobData mob)
        {
            mob.OnWakeup.RemoveListener(Do);
            
            base.OnRemove(mob);
        }

        public IEnumerator Do(MobData mob)
        {
            turn += 1;
            yield return new JumpIn(behaviour.DoAction(ract, turn, this, mob, target, customData));

            if (turn >= maxTurn)
            {
                Destroy();
            }
        }
    }
}