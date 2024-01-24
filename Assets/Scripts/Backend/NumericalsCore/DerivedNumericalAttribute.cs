using System;
using System.Reflection;

namespace miniRAID.Backend.Numericals
{
    public class PathBellAttribute : Attribute
    {
        // TODO: Cache
        public string tooltip;
        public object cachedValue;
    }
}