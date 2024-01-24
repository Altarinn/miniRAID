using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public class PopupTextInstance
    {
        public TMPro.TextMeshProUGUI tmp;
        
        public Rect screenRect;
        public Vector3 originPos;
    }
    
    public float randomRadius = 1.0f;

    public Camera mainCam;
    public Canvas canvas;
    public Transform pivot;

    public GameObject popupPrefabRegular, popupPrefabNumber;
    RectTransform CanvasRect;

    public float relaxSpeed = 10.0f;
    public int initialRelax = 0;
    public bool followCamera = false;
    public float paddingPx = 5.0f;

    private HashSet<PopupTextInstance> _popupTextInstances = new HashSet<PopupTextInstance>();

    public enum PopupType
    {
        Regular,
        Number
    }

    // Start is called before the first frame update
    void Awake()
    {
        CanvasRect = canvas.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        // Popup(
        //     "TEST_STR", 
        //     new Vector3(
        //         Random.Range(0.0f, 15.0f), 
        //         Random.Range(0.0f, 5.0f), 
        //         Random.Range(0.0f, 15.0f)), 
        //     Color.white);
        
        var list = _popupTextInstances.ToArray();
        foreach (var text in list)
        {
            RelaxPopupText(text);
        }
    }

    public Vector2 ResolveCollision(PopupTextInstance text)
    {
        Vector2 velocity = Vector2.zero;
        foreach (var target in _popupTextInstances)
        {
            if (target == text){ continue; }
            if (target.screenRect.Overlaps(text.screenRect))
            {
                var diff = (text.screenRect.position - target.screenRect.position);
                velocity += diff.normalized * Mathf.Min(10.0f, 1.0f / (1e-5f + diff.magnitude));
            }
        }

        return velocity.normalized;
    }

    public void RelaxPopupText(PopupTextInstance text)
    {
        if (text.tmp == null || text.tmp.enabled == false)
        {
            _popupTextInstances.Remove(text);
            return;
        }

        Vector2 velocity = ResolveCollision(text);
        bool hit = velocity.sqrMagnitude > 1e-5;

        if (hit)
        {
            text.screenRect.position += velocity * (relaxSpeed * Time.deltaTime);
            // ClampToScreen(ref text.screenRect);
        }
        else if (followCamera)
        {
            var posCache = text.screenRect.position;
            
            // Towards camera
            velocity = mainCam.WorldToViewportPoint(text.originPos) * CanvasRect.sizeDelta - text.screenRect.position;
            text.screenRect.position += velocity * (relaxSpeed * Time.deltaTime);
            
            // Check if have collision now
            Vector2 newVel = ResolveCollision(text);
            if (newVel.sqrMagnitude > 1e-5) // We have collision after moving towards camera
            {
                // Cancel movement
                text.screenRect.position = posCache;
            }
        }

        text.tmp.rectTransform.anchoredPosition = text.screenRect.position + text.screenRect.size / 2.0f;
    }

    public void ClampToScreen(ref Rect r)
    {
        r.x = Mathf.Clamp(r.x, 0, Screen.width - r.width);
        r.y = Mathf.Clamp(r.y, 0, Screen.height - r.height);
    }

    public GameObject GetPrefab(PopupType type)
    {
        switch (type)
        {
            case PopupType.Regular:
                return popupPrefabRegular;
            case PopupType.Number:
                return popupPrefabNumber;
            default:
                return popupPrefabRegular;
        }
    }

    public void Popup(string text, Vector3 position, Color color, PopupType type = PopupType.Regular)
    {
        position += Random.insideUnitSphere * randomRadius;

        Vector3 screenPos = mainCam.WorldToViewportPoint(position) * CanvasRect.sizeDelta;
        
        var pop = Instantiate(GetPrefab(type), Vector3.zero, Quaternion.identity, pivot).GetComponent<TMPro.TextMeshProUGUI>();

        pop.rectTransform.anchoredPosition = screenPos;

        pop.color = color;
        pop.text = text;
        
        pop.ForceMeshUpdate();

        PopupTextInstance textInst = new PopupTextInstance();
        textInst.tmp = pop;
        textInst.originPos = position;
        textInst.screenRect = new Rect(
            screenPos - pop.textBounds.extents - paddingPx * Vector3.one,
            (pop.textBounds.extents + paddingPx * Vector3.one) * 2.0f);

        _popupTextInstances.Add(textInst);

        for (int i = 0; i < initialRelax; i++)
        {
            RelaxPopupText(textInst);
        }
    }
}
