using miniRAID.Backend;
using UnityEngine;

namespace miniRAID
{
    public class SimpleSpriteIndicator : FollowMobStateRenderer
    {
        public static SimpleSpriteIndicator Instantiate(Sprite sprite, Vector3 pos, int sortOrder = 0)
        {
            SimpleSpriteIndicator s = GameObject.Instantiate(Globals.prefabs.Instance.spriteIndicator, pos, Quaternion.identity)
                .AddComponent<SimpleSpriteIndicator>();
            if (sprite != null)
            {
                s.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
                s.GetComponentInChildren<SpriteRenderer>().sortingOrder = sortOrder;
            }

            return s;
        }
    }
}