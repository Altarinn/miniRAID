using miniRAID.Backend;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace miniRAID
{
    public abstract class FollowMobStateRenderer : MonoBehaviour, IStateRenderer
    {
        protected MobData follow;

        public virtual void Refresh()
        {
            transform.parent = follow?.mobRenderer?.transform;
            transform.localPosition = Vector3.zero;
        }

        public FollowMobStateRenderer Follow(MobData follow)
        {
            this.follow = follow;
            Refresh();
            return this;
        }

        public virtual void Destroy()
        {
            GameObject.Destroy(gameObject);
        }
    }
    
    public class DecalIndicator : FollowMobStateRenderer
    {
        public Material decalMaterial;

        private Transform follow;

        public static DecalIndicator Instantiate(Material decalMaterial, Vector3 pos)
        {
            DecalIndicator d = GameObject.Instantiate(
                Globals.prefabs.Instance.decalIndicator.gameObject, pos, Quaternion.identity)
                .AddComponent<DecalIndicator>();
            
            if (decalMaterial != null)
            {
                d.GetComponentInChildren<DecalProjector>().material = decalMaterial;
            }

            return d;
        }
    }
}