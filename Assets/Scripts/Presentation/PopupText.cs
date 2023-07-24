using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class PopupText : MonoBehaviour
{
    TMPro.TextMeshPro tmp;
    TMPro.TextMeshProUGUI tmpui;

    public Vector3 speed = Vector3.up * 2.0f;
    public float lifespan = 1.0f;
    public float fadeStart = 0.0f;

    float timer;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, lifespan);

        tmp = GetComponent<TMPro.TextMeshPro>();
        tmpui = GetComponent<TMPro.TextMeshProUGUI>();

        tmpui.transform.localScale = Vector3.one * 1.2f;

        var seq = DOTween.Sequence();

        seq.Append(tmpui.transform.DOScale(Vector3.one, 0.15f));
        seq.Join(tmpui.DOColor(new Color(0.984f, 0.694f, 0.380f), 0.15f));
        //seq.Append()
        seq.Play();

        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //timer += Time.deltaTime;

        //transform.Translate(speed * Time.deltaTime);
        
        //if (tmp != null)
        //{
        //    tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, Mathf.Min(1.0f, (lifespan - timer) / (lifespan - fadeStart))); ;
        //}

        //if(tmpui != null)
        //{
        //    tmpui.color = new Color(tmpui.color.r, tmpui.color.g, tmpui.color.b, Mathf.Min(1.0f, (lifespan - timer) / (lifespan - fadeStart))); ;
        //}
    }
}
