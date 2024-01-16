using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CharacterBillboard : MonoBehaviour
{
    public bool AllDirections = false;
    
    Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
    }
    void LateUpdate()
    {
        if(mainCamera == null)
        { transform.rotation = Quaternion.identity; return; }

        Vector3 newRotation = mainCamera.transform.eulerAngles;
        
        if (!AllDirections)
        {
            newRotation.x = 0;
            newRotation.z = 0;
        }

        transform.eulerAngles = newRotation;
    }
}
