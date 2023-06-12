using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace miniRAID.Spells
{
    [CreateAssetMenu(menuName = "Spells/SingleTargetProjectile")]
    public class SingleTargetProjectile : Spell
    {
        [Header("Projectile Interaction")]
        public int damage;
        public Consts.Elements type;
        public List<miniRAID.Buff.BuffSO> buffsToTarget;

        [Header("Projectile Visuals")]
        public TestProjectile prefab;
        public Sprite sprite;
        public Color spriteColor = Color.white;

        public Sprite particle;
        public Color particleColor = Color.white;

        protected override void Awake()
        {
            base.Awake();

            // Change requester type here
            Requester = new UI.TargetRequester.BasicUnitsRequester();
            ((UI.TargetRequester.BasicUnitsRequester)Requester).range = range;
            ((UI.TargetRequester.BasicUnitsRequester)Requester).MinUnits = targetCount;
            ((UI.TargetRequester.BasicUnitsRequester)Requester).MaxUnits = targetCount;
        }

        public override bool CheckWithTargets(MobData mob, SpellTarget target)
        {
            bool baseResult = base.CheckWithTargets(mob, target);

            // Check if the spell is castable here

            return baseResult;
        }

        protected override IEnumerator Coroutine(MobData mob, SpellTarget target)
        {
            // Implement your spell here
            var spell = SpellEntity.Clone(prefab);

            // Set custom materials
            spell.Init(sprite, particle, spriteColor, particleColor);

            spell.OnHit += (s, t) =>
            {
                var gPos = t.targetPos[0];
                var targetMob = Databackend.GetSingleton().getMap(gPos.x, gPos.y, gPos.z).mob;
                //TODO: FIXME:
                //Databackend.GetSingleton().DealDmgHeal(
                //    targetMob,
                //    new Consts.DamageHeal_FrontEndInput
                //    {
                //        source = mob,
                //        value = damage,
                //        type = type
                //    }
                //);

                foreach (var buff in buffsToTarget)
                {
                    targetMob.AddBuff(Instantiate(buff), mob);
                }
            };

            yield return spell.CastCoroutine(mob, target);

            if (autoDestroyEntity == true)
            {
                Destroy(spell.gameObject, destroyDelay);
            }
        }
    }
}
