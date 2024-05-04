using UnityEngine;
using System.Collections;

namespace miniRAID.SpellComponents
{
    public class ProjectileComponent : MonoBehaviour
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

        public void Stop()
        {
            // GetComponent<Renderer>().enabled = false;
            sprite.enabled = false;
            if (trail != null)
            {
                trail.Stop();
            }
        }
    }
}
