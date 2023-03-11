using miniRAID.Spells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using XLua;

using Sirenix.OdinInspector;

namespace miniRAID
{
    public class Projectile : ActionOnPerformTemplate
    {
        [LuaCallCSharp]
        public struct ProjectileTerminationInfo
        {
            Vector2 position;
            bool hitTarget;
        }

        [InfoBox("Single target only")]
        [AssetsOnly]
        public GameObject projectilePrefab;
        public Sprite projectileSprite, projectileTrail;

        public float flyTime = 1.0f;

        [EventSlot]
        [Sirenix.Serialization.OdinSerialize]
        public LuaFunc<(GeneralCombatData, Mob, Vector2Int), IEnumerator> Termination;

        public override IEnumerator EasyEval(GeneralCombatData self, Mob mob, SpellTarget target)
        {
            GameObject obj = GameObject.Instantiate(projectilePrefab, mob.transform.position + Vector3.back, Quaternion.identity);

            //if(projectileSprite != null)
            //{
            //    obj.GetComponent<TestProjectile>().sprite.sprite = projectileSprite;
            //}
            //if(projectileTrail != null)
            //{
            //    obj.GetComponent<TestProjectile>().trail.textureSheetAnimation.SetSprite(0, projectileTrail);
            //}
            obj.GetComponent<TestProjectile>().Init(projectileSprite, projectileTrail, Color.white, Color.white);

            var gPos = target.targetPos[0];

            Vector3 dest = Globals.backend.GridToWorldPos(gPos) + new Vector2(.5f, .5f);
            dest.z = obj.transform.position.z;

            yield return obj.transform
                .DOMove(dest, flyTime)
                .SetEase(Ease.Linear)
                .WaitForCompletion(true);
            obj.GetComponent<TestProjectile>().Stop();

            GameObject.Destroy(obj, Settings.fxTimeout);

            yield return new JumpIn(Termination.Eval((self, mob, gPos)));
        }
    }
}
