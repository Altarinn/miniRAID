using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CanvasPixelPerfectScaler : MonoBehaviour
{
    public Vector2Int referenceResolution;
    public UnityEngine.UI.CanvasScaler scaler;

    private void Awake()
    {
        scaler = GetComponent<UnityEngine.UI.CanvasScaler>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2Int screenSize = new Vector2Int(Screen.width, Screen.height);
        int scaleRatio = Mathf.Min(screenSize.x / referenceResolution.x, screenSize.y / referenceResolution.y);

        scaler.scaleFactor = scaleRatio;
    }
}
