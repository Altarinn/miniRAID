using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID
{
    [RequireComponent(typeof(Collider2D))]
    public class GridShapeMono : MonoBehaviour
    {
        public bool rotationLocked = false;

        GridShape _shape;
        public GridShape Shape
        {
            get
            {
                if(rotationLocked)
                {
                    if(_shape == null)
                    {
                        GenerateShape();
                    }
                    return _shape;
                }
                else
                {
                    GenerateShape();
                    return _shape;
                }
            }
        }

        Collider mainCollider;

        private void Awake()
        {
            mainCollider = GetComponent<Collider>();
        }

        void GenerateShape()
        {
            _shape = new();

            Bounds bounds = mainCollider.bounds;
            
            Vector3Int min = new Vector3Int(
                Mathf.FloorToInt(bounds.min.x),
                Mathf.FloorToInt(bounds.min.y)
            );
            Vector3Int max = new Vector3Int(
                Mathf.CeilToInt(bounds.max.x),
                Mathf.CeilToInt(bounds.max.y)
            );

            for(int x = min.x; x <= max.x; x++)
            {
                for(int y = min.y; y <= max.y; y++)
                {
                    var pos = new Vector3Int(x, y);
                    if (Consts.IsPointWithinCollider(mainCollider, pos))
                    {
                        _shape.AddGrid(pos);
                    }
                }
            }
        }
    }
}
