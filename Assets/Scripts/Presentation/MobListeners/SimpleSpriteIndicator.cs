using UnityEngine;

namespace miniRAID
{
    public class SimpleSpriteIndicator : IMobListenerIndicator
    {
        public Sprite sprite;
        public int sortingOrder;
        public Vector3 position;

        GameObject obj;
        
        public SimpleSpriteIndicator(Sprite sprite, Vector3 pos, int sortOrder = 0)
        {
            this.sprite = sprite;
            this.position = pos;
            this.sortingOrder = sortOrder;
        }
        
        public void Instantiate()
        {
            obj = GameObject.Instantiate(Globals.prefabs.Instance.spriteIndicator, position, Quaternion.identity);
            if (this.sprite != null)
            {
                obj.GetComponentInChildren<SpriteRenderer>().sprite = this.sprite;
                obj.GetComponentInChildren<SpriteRenderer>().sortingOrder = this.sortingOrder;
            }
        }

        public void Update()
        {
        }

        public void Destroy()
        {
            GameObject.Destroy(obj);
        }

        public SimpleSpriteIndicator Follow(MobRenderer mob)
        {
            if (obj != null && mob != null)
            {
                obj.transform.parent = mob.transform;
            }

            return this;
        }

        public SimpleSpriteIndicator Move(Vector3 direction)
        {
            if (obj != null)
            {
                obj.transform.position += direction;
            }

            return this;
        }
    }
}