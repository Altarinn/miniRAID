using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniRAID
{
    public class MobGridProxy : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Vector3Int gP = Databackend.GetSingleton().GetGridPos(transform.position);
            Gizmos.color = new Color(0.3f, 0.5f, 0.3f, 0.3f);
            Gizmos.DrawCube(new Vector3(gP.x + 0.5f, gP.z + 0.5f, 0.0f) + Vector3.one * (1.0f / 64.0f), Vector3.one * 31.0f / 32.0f);
        }
    }
}
