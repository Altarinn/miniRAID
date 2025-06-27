using System.Collections;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.ActionHelpers
{
    public class SimpleMultiTurnAction<TSpellTarget> where TSpellTarget : SpellTarget
    {
        private int totalTurns = 1;

        /// <summary>
        /// Turn starts from 1.
        /// </summary>
        public delegate IEnumerator MultiTurnAction(
            int turn, SimpleMultiTurnActionProxy<TSpellTarget> dummy, 
            MobData src, TSpellTarget target, object customData);

        public SimpleMultiTurnAction(int turns)
        {
            totalTurns = turns;
        }

        public IEnumerator Do(
            MultiTurnAction action, 
            MobData src, 
            TSpellTarget target, 
            bool startImmediately = false, 
            object customData = null)
        {
            var dummyData = ScriptableObject.CreateInstance<NullListenerSO>();
            var listener = new SimpleMultiTurnActionProxy<TSpellTarget>(
                src, dummyData, totalTurns, target, action, customData);
            
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
        
        private TSpellTarget target;
        public SimpleMultiTurnAction<TSpellTarget>.MultiTurnAction action;
        private object customData;
        
        public SimpleMultiTurnActionProxy(
            MobData parent, 
            MobListenerSO data,
            int maxTurn,
            TSpellTarget target,
            SimpleMultiTurnAction<TSpellTarget>.MultiTurnAction action, 
            object customData = null) : base(parent, data)
        {
            this.target = target;
            this.action = action;
            this.customData = customData;

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
            yield return new JumpIn(action.Invoke(turn, this, mob, target, customData));

            if (turn >= maxTurn)
            {
                Destroy();
            }
        }
    }
}