using System;
using System.Collections;
using DG.Tweening;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.ActionHelpers
{
    public class Projectile
    {
        public GameObject projectilePrefab;
        public Sprite projectileSprite, projectileTrail;
        public float flyTime = 1.0f;

        public Projectile()
        {
        }

        public Projectile(
            GameObject prefab, Sprite projectile, Sprite trail)
        {
            projectilePrefab = prefab;
            projectileSprite = projectile;
            projectileTrail = trail;
        }
        
        public IEnumerator WaitForShootAt(
            MobData mob, 
            Vector2Int target)
        {
            if (Globals.cc.animation && mob.mobRenderer != null)
            {
                GameObject obj = GameObject.Instantiate(projectilePrefab, mob.mobRenderer.transform.position + Vector3.back, Quaternion.identity);
            
                obj.GetComponent<TestProjectile>().Init(projectileSprite, projectileTrail, Color.white, Color.white);

                Vector3 dest = Globals.backend.GridToWorldPos(target) + new Vector2(.5f, .5f);
                dest.z = obj.transform.position.z;

                yield return obj.transform
                    .DOMove(dest, flyTime)
                    .SetEase(Ease.Linear)
                    .WaitForCompletion(true);
                obj.GetComponent<TestProjectile>().Stop();

                GameObject.Destroy(obj, Settings.fxTimeout);
            }
        }
    }
}