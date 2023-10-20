using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace miniRAID.ActionHelpers
{
    public class SimpleRayFx
    {
        public Texture2D image;
        public float width = 0.5f;

        public SimpleRay InstantiateRay(Vector3 start, Vector3 end)
        {
            SimpleRay ray = GameObject.Instantiate(Globals.prefabs.Instance.ray.gameObject).GetComponent<SimpleRay>();
            ray.SetRay(start, end, width, image);

            return ray;
        }
    }
    
    [ColoredBox("#ff7")]
    public class SimpleExplosionFx
    {
        public Sprite image;
        public float size = 200.0f;
        public float triggerTime = 0.3f;
        public float time = 0.5f;

        public IEnumerator Do(Vector3 position)
        {
            if (Globals.cc.animation)
            {
                GameObject obj = new GameObject("explosion");
                obj.transform.position = new Vector3(position.x + 0.5f, position.y + 0.5f, position.z + 0.5f);
                var sr = obj.AddComponent<SpriteRenderer>();
                sr.sprite = image;

                sr.DOColor(Color.clear, time);
                DOTween.Sequence()
                    .Append(obj.transform.DOScale(Vector3.one * size, time)
                        .OnComplete(() => {GameObject.Destroy(obj);}))
                    .SetLink(obj);
                
                yield return new WaitForSeconds(triggerTime);
            }
            
            yield return -1;
        }

        public IEnumerator Do(Vector3Int gridPosition) => Do(Globals.backend.GridToWorldPos(gridPosition));
    }
}