using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace miniRAID.Spells
{
    //[ParameterDefaultName("target")]

    //public class SpellTarget
    //{
    //    public List<Vector2Int> targetPos = new List<Vector2Int>();

    //    public SpellTarget() { }

    //    public SpellTarget(Vector2Int point)
    //    {
    //        targetPos.Add(point);
    //    }

    //    public SpellTarget(IEnumerable<Vector2Int> point)
    //    {
    //        targetPos.AddRange(point);
    //    }
    //}

    public abstract class SpellEntity : MonoBehaviour
    {
        // Static members

        public static List<SpellEntity> activeSpellEntities = new List<SpellEntity>();

        public static bool IsAnimationFinished
        {
            get
            {
                return activeSpellEntities.Count <= 0;
            }
        }

        public static T Clone<T>(T prefab) where T : SpellEntity
        {
            T spell = Instantiate(prefab.gameObject).GetComponent<T>();
            return spell;
        }

        // Non-static members

        public delegate void RegularEntityEvent(SpellEntity ent);
        public delegate void HitEntityEvent(SpellEntity ent, SpellTarget target);

        public event RegularEntityEvent OnUpdate;
        public event HitEntityEvent OnHit;
        public event RegularEntityEvent OnFinished;

        public virtual void Cast(Mob source, SpellTarget target, Action.OnActionCorotineFinished callback)
        {
            Globals.actionHost.Instance.HostActionCoroutine(CastCoroutine(source, target), callback);
        }

        protected virtual void Update()
        {
            OnUpdate?.Invoke(this);
        }

        public IEnumerator CastCoroutine(Mob source, SpellTarget target)
        {
            yield return Coroutine(source, target);

            OnFinished?.Invoke(this);
        }

        /// <summary>
        /// Can be implemented by child classes.
        /// </summary>
        /// <param name="source">Source of this spell</param>
        /// <param name="target">Target of this spell</param>
        /// <returns></returns>
        public abstract IEnumerator Coroutine(Mob source, SpellTarget target);

        protected virtual void Hit(SpellTarget target)
        {
            OnHit?.Invoke(this, target);
        }
    }
}
