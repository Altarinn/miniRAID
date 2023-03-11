using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopBar : MonoBehaviour
{
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public TMPro.TextMeshProUGUI title;
    public SubActionBar defBar, atkBar;

    public void SetAction(ActionEntry entry)
    {
        title.text = entry.name;
    }

    public void Clear()
    {
        title.text = "CLEARED";
        defBar.Clear();
        atkBar.Clear();
    }
}
