using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRay : MonoBehaviour
{
    public void SetRay(Vector3 start, Vector3 end, float width, Texture2D texture)
    {
        transform.position = start;

        Vector3 z = (end - start).normalized;
        Vector3 up = (Camera.main.transform.position - start).normalized;
        
        transform.rotation = Quaternion.LookRotation(z, up);

        transform.localScale = new Vector3(width, 1, Vector3.Distance(start, end));
        
        var mat = GetComponentInChildren<MeshRenderer>().material;
        mat.mainTexture = texture;
        mat.mainTextureScale = new Vector2(1, Vector3.Distance(start, end));
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
