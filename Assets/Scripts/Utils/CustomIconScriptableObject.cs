using System.Collections;
using UnityEngine;

using Sirenix.OdinInspector;
using NuclearBand;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

public class ScriptableObjectIdAttribute : PropertyAttribute { }

public class EffectIdAttribute : PropertyAttribute { }

namespace miniRAID
{
    public class CustomIconScriptableObject : SerializedScriptableObject
    {
        [ScriptableObjectId]
        [PropertyOrder(-100)]
        public string Guid;

        [EffectId]
        [PropertyOrder(-100)]
        public int Id;

        [PropertyOrder(-100)]
        public Sprite icon;

//        protected T AppendChild<T>() where T : ScriptableObject
//        {
//            T v = ScriptableObject.CreateInstance<T>();

//#if UNITY_EDITOR
//            AssetDatabase.AddObjectToAsset(v, this);
//#endif

//            return v;
//        }
    }

    public class RuntimeWrapper<T> where T : CustomIconScriptableObject
    {
        [HideInInspector]
        T data;
    }
}