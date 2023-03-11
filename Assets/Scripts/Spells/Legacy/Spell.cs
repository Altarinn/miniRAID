using UnityEngine;
using System.Collections;

namespace miniRAID.Spells
{
    // TODO: show spell icon in editor
    [CreateAssetMenu(menuName = "Spells/BasicSpell")]
    public class Spell : Action
    {
        public int targetCount = 1;
        public int range = 5;

        public bool autoDestroyEntity = true;
        public float destroyDelay = 0.0f;

        //[Space]
        //public SpellEntity entityPrefab;

        public static Spell Dummy(string name)
        {
            Spell spell = new Spell();
            spell.ActionName = name;

            return spell;
        }

        protected virtual void Awake()
        {
            CooldownRemain = 0;

            Requester = new UI.TargetRequester.BasicUnitsRequester();
            ((UI.TargetRequester.BasicUnitsRequester)Requester).range = range;
            ((UI.TargetRequester.BasicUnitsRequester)Requester).MinUnits = targetCount;
            ((UI.TargetRequester.BasicUnitsRequester)Requester).MaxUnits = targetCount;

            //Requester = new UI.TargetRequester.FourDirectionalRequester();
            //(Requester as UI.TargetRequester.FourDirectionalRequester).shape = new Vector2Int[]
            //{
            //    new Vector2Int( 0, 0),
            //    new Vector2Int( 0, 1),
            //    new Vector2Int( 0, 2),
            //    new Vector2Int( 0, 3),
            //    new Vector2Int( 1, 0),
            //    new Vector2Int(-1, 0),
            //};
        }

        public override bool CheckWithTargets(Mob mob, SpellTarget target)
        {
            bool baseResult = base.CheckWithTargets(mob, target);

            bool isInRange = true;
            foreach (var item in target.targetPos)
            {
                isInRange &= Globals.backend.Distance(mob.data.Position, item) <= range;
            }

            return baseResult && isInRange;
        }

        protected override bool Do(Mob mob, SpellTarget target, OnActionCorotineFinished callback = null, bool cd = true, bool host = false)
        {
            return base.Do(mob, target, callback, cd, host);
        }

        
        protected override IEnumerator Coroutine(Mob mob, SpellTarget target)
        {
            yield break;
            /*
            SpellEntity spell = Instantiate(entityPrefab.gameObject, mob.transform.position, Quaternion.identity).GetComponent<SpellEntity>();

            yield return spell.CastCoroutine(mob, target);

            if (autoDestroyEntity == true)
            {
                Destroy(spell.gameObject, destroyDelay);
            }
            */
        }
}
}
