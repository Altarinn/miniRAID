using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public float randomRadius = 1.0f;

    public Camera mainCam;
    public Canvas canvas;
    public Transform pivot;

    public GameObject popupPrefab;
    RectTransform CanvasRect;

    // Start is called before the first frame update
    void Start()
    {
        CanvasRect = canvas.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Popup(string text, Vector3 position)
    {
        position += Random.insideUnitSphere * randomRadius;

        Vector3 screenPos = mainCam.WorldToViewportPoint(position);

        var pop = Instantiate(popupPrefab, Vector3.zero, Quaternion.identity, pivot).GetComponent<TMPro.TextMeshProUGUI>();

        pop.rectTransform.anchoredPosition = new Vector2(
            ((screenPos.x * CanvasRect.sizeDelta.x)),
            ((screenPos.y * CanvasRect.sizeDelta.y)));

        pop.text = text;
    }
}
