using System.Collections.Generic;
using UnityEngine;

namespace miniRAID
{
    [CreateAssetMenu(menuName = "XLSXDefinitionSO")]
    public class XLSXDefinitionSO : ScriptableObject
    {
        public string path;
        public List<CustomIconScriptableObject> objects = new List<CustomIconScriptableObject>();
    }
}