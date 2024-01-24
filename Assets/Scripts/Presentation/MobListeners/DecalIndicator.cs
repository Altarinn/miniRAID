using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace miniRAID
{
    public class DecalIndicator : IMobListenerIndicator
    {
        public Material decalMaterial;
        public Vector3 position;

        BossTargetIndicator obj;

        public DecalIndicator(Material decalMaterial, Vector3 pos)
        {
            this.decalMaterial = decalMaterial;
            this.position = pos;
        }
        
        public void Instantiate()
        {
            obj = GameObject.Instantiate(Globals.prefabs.Instance.decalIndicator.gameObject, position, Quaternion.identity).GetComponent<BossTargetIndicator>();
            
            if (this.decalMaterial != null)
            {
                obj.GetComponentInChildren<DecalProjector>().material = decalMaterial;
            }
        }

        public void Update()
        {
            
        }

        public void Destroy()
        {
            GameObject.Destroy(obj);
        }

        public DecalIndicator Follow(Transform follow)
        {
            obj.Follow(follow);
            return this;
        }
    }
}