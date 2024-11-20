using System;
using MemoryPack;
using UnityEngine;

namespace miniRAID
{
    public class CustomIconScriptableObjectRegister : MonoBehaviour
    {
        private void Awake()
        {
            MemoryPackFormatterProvider.Register<CustomIconScriptableObject>(new CustomIconScriptableObjectFormatter());
        }
    }
}