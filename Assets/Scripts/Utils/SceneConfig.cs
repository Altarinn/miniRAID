using UnityEngine;

namespace Utils
{
    public class SceneConfig : MonoBehaviour
    {
        [Header("Combat Configuration")]
        public bool enableAnimation = true;
        public bool forceNoWait = false;
        public float turnWaitTimeSec = 0.0f;
        
        [Header("Map Configuration")]
        public string mapName = "default";
        public Vector3 playerStartPosition = Vector3.zero;
    }
}