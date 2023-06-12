using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace miniRAID.UI
{
    public class GridShapeCursor : MonoBehaviour
    {
        public GameObject gridMask;
        Transform gridsPivot;

        private void Awake()
        {
            gridsPivot = new GameObject().transform;
            gridsPivot.parent = transform;

            gridsPivot.localPosition = Vector3.zero;
            gridsPivot.localRotation = Quaternion.identity;
            gridsPivot.localScale = Vector3.one;

            this.cursorShape = new GridShape(new Vector3Int(0, 0));
        }

        public Vector3Int position
        {
            get => Globals.backend.GetGridPos(
                new Vector2(transform.position.x, transform.position.y)
            );

            set
            {
                //this.cursorShape.position = position;
                transform.position = Globals.backend.GridToWorldPos(value);
            }
        }

        GridShape _cursorShape;
        public GridShape cursorShape
        {
            get => _cursorShape;

            set
            {
                _cursorShape = value;

                // temporarily remove position
                Vector3Int _pos = _cursorShape.position;
                _cursorShape.position = new Vector3Int(0, 0);

                var result = _cursorShape.ApplyTransform();

                _cursorShape.position = _pos;

                // Update masks
                foreach (Transform child in gridsPivot)
                {
                    Destroy(child.gameObject);
                }

                foreach (var p in result)
                {
                    Instantiate(
                        gridMask,
                        gridsPivot.position + new Vector3(p.x, p.y, 0),
                        Quaternion.identity,
                        gridsPivot
                    );
                }
            }
        }


    }
}
