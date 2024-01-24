using System.Collections;
using System.Collections.Generic;
using miniRAID;
using UnityEngine;
using UnityEngine.Serialization;

public class BossTargetIndicator : MonoBehaviour
{
    private Transform follow;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Follow(Transform target)
    {
        follow = target;
    }

    // Update is called once per frame
    void Update()
    {
        if (follow != null)
        {
            transform.position = follow.position;
        }
    }
}
