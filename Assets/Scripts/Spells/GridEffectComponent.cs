using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.Buff
{
    public class GridEffectComponent : MonoBehaviour
    {
        public GameObject gridFxPrefab;

        public void AddGrid(Vector2 position)
        {
            Instantiate(
                gridFxPrefab,
                new Vector3(position.x, position.y, 0),
                Quaternion.identity,
                transform
            );
        }
    }
}
