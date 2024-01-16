using System;
using System.Collections;
using DG.Tweening;
using miniRAID.Spells;
using UnityEngine;

namespace miniRAID.ActionHelpers
{
    [ColoredBox("#ffec6e")]
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
            Vector3Int target)
        {
            if (Globals.cc.animation && mob.mobRenderer != null)
            {
                // TODO: Projectile firing position pivot
                GameObject obj = GameObject.Instantiate(projectilePrefab, mob.mobRenderer.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            
                obj.GetComponent<TestProjectile>().Init(projectileSprite, projectileTrail, Color.white, Color.white);

                Vector3 dest = Globals.backend.GridToWorldPosCentered(target);
                // dest.z = obj.transform.position.z;

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