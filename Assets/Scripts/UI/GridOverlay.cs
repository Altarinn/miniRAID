using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOverlay : MonoBehaviour
{
    public enum Types
    {
        MOVE = 0,
        ATTACK = 1,
        HEAL = 2,
        BUFF = 3,
        DEBUFF = 4,
        SUMMON = 5,
        SELECTED = 6,
        
        CUSTOM = 7,
        INCOMING_ATTACK = 8,
    }

    public delegate void OnOverlayChangedDelegate(GridOverlay overlay);
    public OnOverlayChangedDelegate onChange;

    public Dictionary<Vector3Int, Types> overlay = new Dictionary<Vector3Int, Types>();

    public GameObject overlayObj;
    public Sprite[] overlaySpriteOverride = new Sprite[Enum.GetNames(typeof(Types)).Length];

    bool shouldRedraw = false;

    private void Start()
    {
        
    }

    public void SetCustomSprite(Sprite sprite)
    {
        overlaySpriteOverride[7] = sprite;
    }

    private void LateUpdate()
    {
        if(shouldRedraw)
        {
            shouldRedraw = false;
            Draw();
        }
    }

    private void Draw()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in overlay)
        {
            // Instantiate a cell and set its sprite
            Instantiate(overlayObj, new Vector3(item.Key.x, item.Key.z, transform.position.z - 1), Quaternion.identity, transform).GetComponent<SpriteRenderer>().sprite = overlaySpriteOverride[(int)item.Value];
        }
    }

    public void RequestRedraw()
    {
        shouldRedraw = true;
    }
}
