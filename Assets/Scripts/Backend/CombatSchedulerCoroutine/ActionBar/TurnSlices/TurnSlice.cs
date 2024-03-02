using System.Collections;
using UnityEngine;
using Sprite = UnityEngine.ProBuilder.Shapes.Sprite;

namespace miniRAID.TurnSchedule
{
    public abstract class TurnSlice : CustomIconScriptableObject
    {
        public Sprite barIcon;
        public Color mainColor;
        public string label;
        public bool showInUI = true;

        protected CombatSchedulerCoroutine coroutine;
        public void RegisterTo(CombatSchedulerCoroutine coroutine)
        {
            this.coroutine = coroutine;
        }

        public abstract IEnumerator Turn();
    }
}