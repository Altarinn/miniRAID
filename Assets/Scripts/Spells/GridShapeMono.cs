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

        Collider2D mainCollider;

        private void Awake()
        {
            mainCollider = GetComponent<Collider2D>();
        }

        void GenerateShape()
        {
            _shape = new();

            Bounds bounds = mainCollider.bounds;
            
            Vector2Int min = new Vector2Int(
                Mathf.FloorToInt(bounds.min.x),
                Mathf.FloorToInt(bounds.min.y)
            );
            Vector2Int max = new Vector2Int(
                Mathf.CeilToInt(bounds.max.x),
                Mathf.CeilToInt(bounds.max.y)
            );

            for(int x = min.x; x <= max.x; x++)
            {
                for(int y = min.y; y <= max.y; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (mainCollider.OverlapPoint(pos))
                    {
                        _shape.AddGrid(pos);
                    }
                }
            }
        }
    }
}
