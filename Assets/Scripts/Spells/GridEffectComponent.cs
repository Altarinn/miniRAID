using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.Buff
{
    public class GridEffectComponent : MonoBehaviour
    {
        public GameObject gridFxPrefab;

        public void AddGrid(Vector3 position)
        {
            Instantiate(
                gridFxPrefab,
                new Vector3(position.x, position.y, position.z),
                Quaternion.identity,
                transform
            );
        }
    }
}
