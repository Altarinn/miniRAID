using UnityEngine;
using System.Collections;

namespace miniRAID.Spells
{
    public class TestProjectile : SpellEntity
    {
        public SpriteRenderer sprite;
        public ParticleSystem trail;

        public void Init(Sprite baseSprite, Sprite particleTex, Color spriteColor, Color particleColor)
        {
            if(baseSprite != null) { sprite.sprite = baseSprite; }
            sprite.color = spriteColor;

            var m = new Material(trail.GetComponent<Renderer>().material);
            m.color = particleColor;
            if(particleTex != null)
            {
                trail.textureSheetAnimation.SetSprite(0, particleTex);
            }
            trail.GetComponent<Renderer>().material = m;
        }

        public override IEnumerator Coroutine(MobData source, SpellTarget target)
        {
            transform.position = source.mobRenderer.transform.position;
            var gPos = target.targetPos[0];

            // Don't check this anymore
            // There's no mob in our target ... this should not happen though.
            //if (targetMob == null)
            //{
            //    // Stop
            //    Stop();
            //    yield break;
            //}

            Vector3 targetPos_real = new Vector3(gPos.x + 0.5f, gPos.y + 0.5f, transform.position.z);
            transform.rotation = Quaternion.FromToRotation(Vector3.left, (targetPos_real - transform.position));

            while ((transform.position - targetPos_real).magnitude >= 1e-3)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos_real, 12.0f * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPos_real;
            Hit(target);

            Stop();
            yield break;
        }

        public void Stop()
        {
            GetComponent<Renderer>().enabled = false;
            if (trail != null)
            {
                trail.Stop();
            }
        }
    }
}
