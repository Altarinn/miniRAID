using System.Collections;
using UnityEngine;
using Sprite = UnityEngine.ProBuilder.Shapes.Sprite;

namespace miniRAID.TurnSchedule
{
    public abstract class TurnSliceSO : CustomIconScriptableObject
    {
        public Sprite barIcon;
        public Color mainColor;
        public string label;
        public bool showInUI = true;

        public TurnSlice Wrap()
        {
            return new TurnSlice(this);
        }

        public abstract IEnumerator Turn(CombatSchedulerCoroutine coroutine);
    }

    public class TurnSlice
    {
        public TurnSliceSO data;

        public bool ShowInUI => data.showInUI;
        public string Label => data.label;
        public Color MainColor => data.mainColor;
        public Sprite BarIcon => data.barIcon;
        
        protected CombatSchedulerCoroutine coroutine;

        public TurnSlice(TurnSliceSO data)
        {
            this.data = data;
        }
        
        public void RegisterTo(CombatSchedulerCoroutine coroutine)
        {
            this.coroutine = coroutine;
        }

        public virtual IEnumerator Turn() => data.Turn(coroutine);
    }
}