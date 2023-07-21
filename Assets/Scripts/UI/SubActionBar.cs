using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID.UI
{
    public struct ActionEntry
    {
        public string name;
    }

    public class SubActionBar : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI textPrefab;

        //// Start is called before the first frame update
        //void Start()
        //{

        //}

        //// Update is called once per frame
        //void Update()
        //{

        //}

        public void AppendEntry(ActionEntry entry)
        {
            Instantiate(textPrefab.gameObject, this.transform).GetComponent<TMPro.TextMeshProUGUI>().text = entry.name;
        }

        public void Clear()
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name != "Title")
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}
