using System;
using System.Collections;
using System.Collections.Generic;
using miniRAID.Backend;
using UnityEngine;

namespace miniRAID.Buff
{
    public class GridEffectComponent : MonoBehaviour, IStateRenderer
    {
        public GameObject gridFxPrefab;
        private HashSet<Vector3Int> shape = new();
        private HashSet<Vector3Int> incomingShape = new();
        private HashSet<Vector3Int> buffer = new();

        private void Awake()
        {
            shape = new();
        }

        public void AddGrid(Vector3 position)
        {
            Instantiate(
                gridFxPrefab,
                new Vector3(position.x, position.y, position.z),
                Quaternion.identity,
                transform
            );
        }

        public void SetShape(HashSet<Vector3Int> shape)
        {
            incomingShape = shape ?? new HashSet<Vector3Int>();
            Refresh();
        }

        public void Refresh()
        {
            buffer.Clear();
            buffer.UnionWith(incomingShape);
            buffer.ExceptWith(shape);
            foreach (var p in buffer)
            {
                AddGrid(Globals.backend.GridToBackendFloorPos(p));
            }
            
            shape.UnionWith(buffer);
        }

        public void Destroy()
        {
            GameObject.Destroy(gameObject);
        }
    }
}
